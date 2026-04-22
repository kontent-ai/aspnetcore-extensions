using Kontent.Ai.Delivery.Abstractions;
using Kontent.Ai.Delivery.ContentItems.RichText.Resolution;
using Microsoft.AspNetCore.Html;

namespace Kontent.Ai.AspNetCore.RichText;

/// <summary>
/// Razor-friendly extension methods for rendering Kontent.ai rich-text content.
/// </summary>
public static class RichTextExtensions
{
    /// <summary>
    /// Renders structured rich-text content as an <see cref="IHtmlContent"/> suitable for inclusion in a Razor view.
    /// </summary>
    /// <param name="richText">The structured rich-text content.</param>
    /// <param name="resolver">Optional <see cref="IHtmlResolver"/>. When <c>null</c>, a default resolver with SDK-provided built-in behavior is used.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An <see cref="IHtmlContent"/> wrapping the resolver's HTML output.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="richText"/> is <c>null</c>.</exception>
    public static async Task<IHtmlContent> ToHtmlContentAsync(
        this IRichTextContent richText,
        IHtmlResolver? resolver = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(richText);

        resolver ??= new HtmlResolverBuilder().Build();
        var html = await resolver.ResolveAsync(richText, cancellationToken);
        return new HtmlString(html);
    }
}
