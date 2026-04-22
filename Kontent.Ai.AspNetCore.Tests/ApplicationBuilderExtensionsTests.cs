using System.Net;
using System.Security.Cryptography;
using System.Text;
using Kontent.Ai.AspNetCore.Webhooks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Kontent.Ai.AspNetCore.Tests;

public class ApplicationBuilderExtensionsTests
{
    private const string Secret = "test-secret";
    private const string WebhookPath = "/webhooks";
    private const string SkipPath = "/other";

    [Fact]
    public async Task WithOptions_ValidSignatureOnMatchingPath_ReachesEndpoint()
    {
        using var host = await BuildHost(app => app.UseWebhookSignatureValidator(
            ctx => ctx.Request.Path.StartsWithSegments(WebhookPath, StringComparison.OrdinalIgnoreCase),
            new WebhookOptions { Secret = Secret }));

        var response = await PostWithSignature(host, WebhookPath, """{"notifications":[]}""", validSecret: Secret);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task WithOptions_InvalidSignatureOnMatchingPath_ReturnsUnauthorized()
    {
        using var host = await BuildHost(app => app.UseWebhookSignatureValidator(
            ctx => ctx.Request.Path.StartsWithSegments(WebhookPath, StringComparison.OrdinalIgnoreCase),
            new WebhookOptions { Secret = Secret }));

        var response = await PostWithSignature(host, WebhookPath, "body", validSecret: "wrong-secret");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task NonMatchingPath_SkipsSignatureValidation()
    {
        using var host = await BuildHost(app => app.UseWebhookSignatureValidator(
            ctx => ctx.Request.Path.StartsWithSegments(WebhookPath, StringComparison.OrdinalIgnoreCase),
            new WebhookOptions { Secret = Secret }));

        // No signature header at all — predicate is false → middleware skipped → endpoint responds 200.
        using var client = host.GetTestClient();
        var response = await client.PostAsync(SkipPath, new StringContent("body"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task WithActionConfigurator_BindsOptions()
    {
        using var host = await BuildHost(app => app.UseWebhookSignatureValidator(
            ctx => ctx.Request.Path.StartsWithSegments(WebhookPath, StringComparison.OrdinalIgnoreCase),
            options => options.Secret = Secret));

        var response = await PostWithSignature(host, WebhookPath, """{"ok":true}""", validSecret: Secret);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task WithConfigurationSection_BindsOptions()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"{nameof(WebhookOptions)}:{nameof(WebhookOptions.Secret)}"] = Secret
            })
            .Build();

        using var host = await BuildHost(app => app.UseWebhookSignatureValidator(
            ctx => ctx.Request.Path.StartsWithSegments(WebhookPath, StringComparison.OrdinalIgnoreCase),
            configuration.GetSection(nameof(WebhookOptions))));

        var response = await PostWithSignature(host, WebhookPath, """{"ok":true}""", validSecret: Secret);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private static async Task<IHost> BuildHost(Action<IApplicationBuilder> configureMiddleware)
    {
        var host = await new HostBuilder()
            .ConfigureWebHost(webHost =>
            {
                webHost
                    .UseTestServer()
                    .Configure(app =>
                    {
                        configureMiddleware(app);
                        app.Run(async ctx => await ctx.Response.WriteAsync("ok"));
                    });
            })
            .StartAsync();

        return host;
    }

    private static async Task<HttpResponseMessage> PostWithSignature(IHost host, string path, string body, string validSecret)
    {
        var signature = ComputeHmacSha256(body, validSecret);
        using var client = host.GetTestClient();
        var request = new HttpRequestMessage(HttpMethod.Post, path)
        {
            Content = new StringContent(body)
        };
        request.Headers.Add("X-Kontent-ai-Signature", signature);
        return await client.SendAsync(request);
    }

    private static string ComputeHmacSha256(string message, string secret)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(message)));
    }
}
