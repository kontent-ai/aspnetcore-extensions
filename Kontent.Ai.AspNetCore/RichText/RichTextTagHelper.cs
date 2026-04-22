using Kontent.Ai.Delivery.Abstractions;
using Kontent.Ai.Delivery.ContentItems.RichText.Resolution;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Kontent.Ai.AspNetCore.RichText;

/// <summary>
/// Renders Kontent.ai structured rich-text content as HTML inside a Razor view.
/// </summary>
/// <remarks>
/// The tag helper selects its <see cref="IHtmlResolver"/> in the following order:
/// <list type="number">
///   <item><description>The <c>resolver</c> attribute on the tag, when set.</description></item>
///   <item><description>An <see cref="IHtmlResolver"/> registered via <c>AddKontentRichText</c>.</description></item>
///   <item><description>A built-in resolver with the SDK's default behavior.</description></item>
/// </list>
/// The <c>&lt;rich-text&gt;</c> element itself is not emitted — the resolver's HTML is rendered in its place.
/// </remarks>
[HtmlTargetElement("rich-text", Attributes = "content")]
public class RichTextTagHelper : TagHelper
{
    private readonly IHtmlResolver? _defaultResolver;

    /// <summary>
    /// The structured rich-text content to render.
    /// </summary>
    [HtmlAttributeName("content")]
    public IRichTextContent? Content { get; set; }

    /// <summary>
    /// Optional <see cref="IHtmlResolver"/> for this rendering. Overrides the DI-registered resolver.
    /// </summary>
    [HtmlAttributeName("resolver")]
    public IHtmlResolver? Resolver { get; set; }

    /// <summary>
    /// Creates an instance of the <see cref="RichTextTagHelper"/>.
    /// </summary>
    /// <param name="defaultResolver">Optional resolver injected from the DI container via <c>AddKontentRichText</c>.</param>
    public RichTextTagHelper(IHtmlResolver? defaultResolver = null)
    {
        _defaultResolver = defaultResolver;
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = null;

        if (Content == null)
        {
            return;
        }

        var resolver = Resolver ?? _defaultResolver ?? new HtmlResolverBuilder().Build();
        var html = await resolver.ResolveAsync(Content);
        output.Content.SetHtmlContent(html);
    }
}
