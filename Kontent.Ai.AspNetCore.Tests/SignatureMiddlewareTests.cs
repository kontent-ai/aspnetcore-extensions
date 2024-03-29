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

        [Fact]
        public async Task RequestWithInvalidSignatureEmptyBody_ReturnsUnauthorized()
        {
            // Arrange
            var options = Options.Create(new WebhookOptions { });
            var middleware = new SignatureMiddleware(null, options);
            var ctx = new DefaultHttpContext();
            ctx.Request.Headers.Add("X-KC-Signature", "ABC");

            // Act
            await middleware.InvokeAsync(ctx);

            // Assert
            Assert.Equal(ctx.Response.StatusCode, (int)HttpStatusCode.Unauthorized);
        }

        [Theory]
        [InlineData("ezKcSmuYrugdCN73QVWDXREaNY7AkUhFWksUFlp9Tnc=", "PublishDeliveryTriggerWebhookBody.json", "fiJ+MiJxiqmzbnlVzWnR+7Rgas7aDSzsqIApflYiZ4o=", HttpStatusCode.NotFound)]
        [InlineData("8UXIHkZ6KuarukFbZDhzyeVAHcKkFnlLabScv9VyNww=", "PublishManagementTriggerWebhookBody.json", "khBZy02UmiUrp2bl1ooPJdILKUmmL2Q7kx318+arMhM=", HttpStatusCode.NotFound)]
        [InlineData("8UXIHkZ6KuarukFbZDhzyeVAHcKkFnlLabScv9VyNww=", "PublishDeliveryTriggerWebhookBody.json", "tampered-body-hash", HttpStatusCode.Unauthorized)]
        [InlineData("8UXIHkZ6KuarukFbZDhzyeVAHcKkFnlLabScv9VyNww=", "PublishManagementTriggerWebhookBody.json", "tampered-body-hash", HttpStatusCode.Unauthorized)]
        // https://docs.microsoft.com/en-us/aspnet/core/test/middleware
        public async Task TriggerRequest_WithValidSignature_CorrectStatusSet(string authorizationSecret, string bodyFilename, string signature, HttpStatusCode resultStatus)
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

            var responsePath = Path.Combine(Environment.CurrentDirectory, "Data", bodyFilename);
            var server = host.GetTestServer();
            var context = await server.SendAsync(c =>
            {
                c.Request.Method = HttpMethods.Post;
                c.Request.Headers.Add("X-KC-Signature", signature);
                c.Request.Body = new StringContent(File.ReadAllText(responsePath)).ReadAsStream();
            });

            Assert.Equal(context.Response.StatusCode, (int)resultStatus);
        }
    }
}
