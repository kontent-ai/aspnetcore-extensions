using Kontent.Ai.AspNetCore.Webhooks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Kontent.Ai.AspNetCore.Tests
{
    public class SignatureMiddlewareTests
    {
        [Fact]
        public async Task RequestWithoutSignature_ReturnsUnauthorized()
        {
            // Arrange
            var options = Options.Create(new WebhookOptions { });
            var middleware = new SignatureMiddleware(null, options);
            var ctx = new DefaultHttpContext();

            // Act
            await middleware.InvokeAsync(ctx);

            // Assert
            Assert.Equal(ctx.Response.StatusCode, (int)HttpStatusCode.Unauthorized);
        }

        [Theory]
        [InlineData("X-KC-Signature")]
        [InlineData("X-Kontent-ai-Signature")]
        public async Task RequestWithInvalidSignatureEmptyBody_ReturnsUnauthorized(string headerName)
        {
            // Arrange
            var options = Options.Create(new WebhookOptions { });
            var middleware = new SignatureMiddleware(null, options);
            var ctx = new DefaultHttpContext();
            ctx.Request.Headers.Add(headerName, "ABC");

            // Act
            await middleware.InvokeAsync(ctx);

            // Assert
            Assert.Equal(ctx.Response.StatusCode, (int)HttpStatusCode.Unauthorized);
        }

        [Theory]
        [InlineData("X-KC-Signature", "ezKcSmuYrugdCN73QVWDXREaNY7AkUhFWksUFlp9Tnc=", "PublishDeliveryTriggerWebhookBody.json", "fiJ+MiJxiqmzbnlVzWnR+7Rgas7aDSzsqIApflYiZ4o=", HttpStatusCode.NotFound)]
        [InlineData("X-KC-Signature", "8UXIHkZ6KuarukFbZDhzyeVAHcKkFnlLabScv9VyNww=", "PublishManagementTriggerWebhookBody.json", "khBZy02UmiUrp2bl1ooPJdILKUmmL2Q7kx318+arMhM=", HttpStatusCode.NotFound)]
        [InlineData("X-KC-Signature", "8UXIHkZ6KuarukFbZDhzyeVAHcKkFnlLabScv9VyNww=", "PublishDeliveryTriggerWebhookBody.json", "tampered-body-hash", HttpStatusCode.Unauthorized)]
        [InlineData("X-KC-Signature", "8UXIHkZ6KuarukFbZDhzyeVAHcKkFnlLabScv9VyNww=", "PublishManagementTriggerWebhookBody.json", "tampered-body-hash", HttpStatusCode.Unauthorized)]
        [InlineData("X-Kontent-ai-Signature", "ezKcSmuYrugdCN73QVWDXREaNY7AkUhFWksUFlp9Tnc=", "PublishDeliveryTriggerWebhookBody.json", "fiJ+MiJxiqmzbnlVzWnR+7Rgas7aDSzsqIApflYiZ4o=", HttpStatusCode.NotFound)]
        [InlineData("X-Kontent-ai-Signature", "8UXIHkZ6KuarukFbZDhzyeVAHcKkFnlLabScv9VyNww=", "PublishManagementTriggerWebhookBody.json", "khBZy02UmiUrp2bl1ooPJdILKUmmL2Q7kx318+arMhM=", HttpStatusCode.NotFound)]
        [InlineData("X-Kontent-ai-Signature", "8UXIHkZ6KuarukFbZDhzyeVAHcKkFnlLabScv9VyNww=", "PublishDeliveryTriggerWebhookBody.json", "tampered-body-hash", HttpStatusCode.Unauthorized)]
        [InlineData("X-Kontent-ai-Signature", "8UXIHkZ6KuarukFbZDhzyeVAHcKkFnlLabScv9VyNww=", "PublishManagementTriggerWebhookBody.json", "tampered-body-hash", HttpStatusCode.Unauthorized)]
        // https://docs.microsoft.com/en-us/aspnet/core/test/middleware
        public async Task TriggerRequest_WithValidSignature_CorrectStatusSet(string headerName, string authorizationSecret, string bodyFilename, string signature, HttpStatusCode resultStatus)
        {
            using var host = await new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .Configure(app =>
                        {
                            app.UseWebhookSignatureValidator(context =>
                                true,
                                new WebhookOptions
                                {
                                    Secret = authorizationSecret
                                }
                            );
                        });
                })
                .StartAsync();

            var responsePath = Path.Combine(Environment.CurrentDirectory, "Data", "Legacy", bodyFilename);
            var server = host.GetTestServer();
            var context = await server.SendAsync(c =>
            {
                c.Request.Method = HttpMethods.Post;
                c.Request.Headers.Add(headerName, signature);
                c.Request.Body = new StringContent(File.ReadAllText(responsePath)).ReadAsStream();
            });

            Assert.Equal(context.Response.StatusCode, (int)resultStatus);
        }
    }
}
