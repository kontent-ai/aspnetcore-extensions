using System.Net;
using Kontent.Ai.AspNetCore.Webhooks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Xunit;

namespace Kontent.Ai.AspNetCore.Tests;

public class SignatureMiddlewareTests
{
    [Fact]
    public async Task RequestWithoutSignature_ReturnsUnauthorized()
    {
        // Arrange
        var options = Options.Create(new WebhookOptions { });
        var middleware = new SignatureMiddleware(null!, options);
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
        var middleware = new SignatureMiddleware(null!, options);
        var ctx = new DefaultHttpContext();
        ctx.Request.Headers.Append(headerName, "ABC");

        // Act
        await middleware.InvokeAsync(ctx);

        // Assert
        Assert.Equal(ctx.Response.StatusCode, (int)HttpStatusCode.Unauthorized);
    }
}
