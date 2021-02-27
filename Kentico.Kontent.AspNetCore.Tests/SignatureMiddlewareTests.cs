using Kentico.Kontent.AspNetCore.Webhooks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Kentico.Kontent.AspNetCore.Tests
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
        public async Task RequestWithInvalidSignature_ReturnsUnauthorized()
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
    }
}
