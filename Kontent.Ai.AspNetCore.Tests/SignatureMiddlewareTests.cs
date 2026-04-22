using System.Net;
using System.Security.Cryptography;
using System.Text;
using Kontent.Ai.AspNetCore.Webhooks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Xunit;

namespace Kontent.Ai.AspNetCore.Tests;

public class SignatureMiddlewareTests
{
    private const string Secret = "test-secret";

    [Fact]
    public async Task RequestWithoutSignature_ReturnsUnauthorized()
    {
        var middleware = new SignatureMiddleware(null!, Options.Create(new WebhookOptions()));
        var ctx = new DefaultHttpContext();

        await middleware.InvokeAsync(ctx);

        Assert.Equal((int)HttpStatusCode.Unauthorized, ctx.Response.StatusCode);
    }

    [Theory]
    [InlineData("X-KC-Signature")]
    [InlineData("X-Kontent-ai-Signature")]
    public async Task RequestWithInvalidSignatureEmptyBody_ReturnsUnauthorized(string headerName)
    {
        var middleware = new SignatureMiddleware(null!, Options.Create(new WebhookOptions()));
        var ctx = new DefaultHttpContext();
        ctx.Request.Headers.Append(headerName, "ABC");

        await middleware.InvokeAsync(ctx);

        Assert.Equal((int)HttpStatusCode.Unauthorized, ctx.Response.StatusCode);
    }

    [Fact]
    public async Task RequestWithInvalidSignatureNonEmptyBody_ReturnsUnauthorized()
    {
        var middleware = new SignatureMiddleware(
            _ => Task.CompletedTask,
            Options.Create(new WebhookOptions { Secret = Secret }));

        var ctx = CreateHttpContext("""{"notifications":[]}""", headerName: "X-Kontent-ai-Signature", headerValue: "not-a-real-signature");

        await middleware.InvokeAsync(ctx);

        Assert.Equal((int)HttpStatusCode.Unauthorized, ctx.Response.StatusCode);
    }

    [Theory]
    [InlineData("X-KC-Signature")]
    [InlineData("X-Kontent-ai-Signature")]
    public async Task RequestWithValidSignature_InvokesNext(string headerName)
    {
        var nextCalled = false;
        RequestDelegate next = _ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        const string body = """{"notifications":[]}""";
        var ctx = CreateHttpContext(body, headerName, ComputeHmacSha256(body, Secret));
        var middleware = new SignatureMiddleware(next, Options.Create(new WebhookOptions { Secret = Secret }));

        await middleware.InvokeAsync(ctx);

        Assert.True(nextCalled);
        Assert.NotEqual((int)HttpStatusCode.Unauthorized, ctx.Response.StatusCode);
    }

    [Fact]
    public async Task RequestWithValidSignature_BodyRemainsReadableDownstream()
    {
        const string body = """{"foo":"bar"}""";
        string? downstreamBody = null;
        RequestDelegate next = async ctx =>
        {
            using var reader = new StreamReader(ctx.Request.Body);
            downstreamBody = await reader.ReadToEndAsync();
        };

        var httpContext = CreateHttpContext(body, "X-Kontent-ai-Signature", ComputeHmacSha256(body, Secret));
        var middleware = new SignatureMiddleware(next, Options.Create(new WebhookOptions { Secret = Secret }));

        await middleware.InvokeAsync(httpContext);

        Assert.Equal(body, downstreamBody);
    }

    [Fact]
    public async Task ModernSignatureHeaderTakesPrecedenceOverLegacy()
    {
        // Middleware prefers X-KC-Signature if present; X-Kontent-ai-Signature is the fallback.
        var nextCalled = false;
        RequestDelegate next = _ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        const string body = "payload";
        var validSignature = ComputeHmacSha256(body, Secret);

        var ctx = CreateHttpContext(body);
        ctx.Request.Headers.Append("X-KC-Signature", validSignature);
        ctx.Request.Headers.Append("X-Kontent-ai-Signature", "invalid-fallback");

        var middleware = new SignatureMiddleware(next, Options.Create(new WebhookOptions { Secret = Secret }));
        await middleware.InvokeAsync(ctx);

        Assert.True(nextCalled);
    }

    private static DefaultHttpContext CreateHttpContext(string body, string? headerName = null, string? headerValue = null)
    {
        var ctx = new DefaultHttpContext();
        var bytes = Encoding.UTF8.GetBytes(body);
        ctx.Request.Body = new MemoryStream(bytes);
        ctx.Request.ContentLength = bytes.Length;
        if (headerName != null && headerValue != null)
        {
            ctx.Request.Headers.Append(headerName, headerValue);
        }
        return ctx;
    }

    private static string ComputeHmacSha256(string message, string secret)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(message)));
    }
}
