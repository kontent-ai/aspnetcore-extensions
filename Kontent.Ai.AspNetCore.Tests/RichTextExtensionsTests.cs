using Kontent.Ai.AspNetCore.RichText;
using Kontent.Ai.Delivery.Abstractions;
using Microsoft.AspNetCore.Html;
using Xunit;

namespace Kontent.Ai.AspNetCore.Tests;

public class RichTextExtensionsTests
{
    [Fact]
    public async Task ToHtmlContentAsync_WithNullRichText_Throws()
    {
        IRichTextContent? richText = null;

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => richText!.ToHtmlContentAsync());
    }

    [Fact]
    public async Task ToHtmlContentAsync_WithoutResolver_UsesDefaultResolver()
    {
        var richText = new EmptyRichTextContent();

        var result = await richText.ToHtmlContentAsync();

        Assert.IsType<HtmlString>(result);
        Assert.Equal(string.Empty, result.ToString());
    }

    [Fact]
    public async Task ToHtmlContentAsync_WithExplicitResolver_UsesProvidedResolver()
    {
        var richText = new EmptyRichTextContent();
        var resolver = new RecordingHtmlResolver { ReturnValue = "<p>hello</p>" };

        var result = await richText.ToHtmlContentAsync(resolver);

        Assert.Equal(1, resolver.CallCount);
        Assert.Same(richText, resolver.LastContent);
        Assert.Equal("<p>hello</p>", result.ToString());
    }

    [Fact]
    public async Task ToHtmlContentAsync_PropagatesCancellationToken()
    {
        var richText = new EmptyRichTextContent();
        var resolver = new RecordingHtmlResolver();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => richText.ToHtmlContentAsync(resolver, cts.Token));
    }
}
