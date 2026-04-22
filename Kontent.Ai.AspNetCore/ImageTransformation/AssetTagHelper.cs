using Kontent.Ai.Delivery.Abstractions;
using Kontent.Ai.Urls.ImageTransformation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace Kontent.Ai.AspNetCore.ImageTransformation;

/// <summary>
/// A tag helper that generates img elements based on assets stored in Kontent.ai.
/// </summary>
[RestrictChildren("media-condition")]
[HtmlTargetElement("img-asset", Attributes = "asset")]
public class AssetTagHelper : TagHelper
{
    internal const string SizesCollection = "sizes";

    private int[]? _responsiveWidths;

    /// <summary>
    /// Application settings.
    /// </summary>
    public IOptions<ImageTransformationOptions>? ImageTransformationOptions { get; set; }

    /// <summary>
    /// Represents an asset stored in Kontent.ai. This property is mandatory in order to properly generate an img tag.
    /// </summary>
    [HtmlAttributeName("asset")]
    public IAsset? Asset { get; set; }

    /// <summary>
    /// Allows overriding the alt and title attributes of an image.
    /// </summary>
    [HtmlAttributeName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// The last parameter of the sizes attribute of an image.
    /// </summary>
    [HtmlAttributeName("default-width")]
    public int DefaultWidth { get; set; } = 300;

    /// <summary>
    /// Widths in which a given image is available. This property is used to generate the resulting srcset. This can also be set globally using <see cref="ImageTransformationOptions"/>.
    /// </summary>
    [HtmlAttributeName("responsive-widths")]
    public int[]? ResponsiveWidths
    {
        get => _responsiveWidths ?? ImageTransformationOptions?.Value.ResponsiveWidths;
        set => _responsiveWidths = value;
    }

    /// <summary>
    /// Constructor that allows to set global image transformation behavior.
    /// </summary>
    /// <param name="imageTransformationOption">An instance of a configuration object allowing to adjust the image transformation behavior.</param>
    public AssetTagHelper(IOptions<ImageTransformationOptions>? imageTransformationOption = null)
    {
        ImageTransformationOptions = imageTransformationOption;
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if (Asset == null)
        {
            await base.ProcessAsync(context, output);
            return;
        }

        output.TagName = "img";
        output.TagMode = TagMode.SelfClosing;

        var width = context.AllAttributes["width"];
        var height = context.AllAttributes["height"];
        var imageUrlBuilder = new ImageUrlBuilder(Asset.Url);

        if (width?.Value != null)
        {
            imageUrlBuilder = imageUrlBuilder.WithWidth(Convert.ToDouble(width.Value.ToString()));
        }

        if (height?.Value != null)
        {
            imageUrlBuilder = imageUrlBuilder.WithHeight(Convert.ToDouble(height.Value.ToString()));
        }

        var image = new TagBuilder("img");

        var responsiveWidths = ResponsiveWidths;
        if (responsiveWidths is { Length: > 0 } && width?.Value == null && height?.Value == null)
        {
            var srcSet = string.Join(",", responsiveWidths.Select(w => $"{new ImageUrlBuilder(Asset.Url).WithWidth(Convert.ToDouble(w)).Url} {w}w"));
            image.MergeAttribute("srcset", srcSet);

            var sizes = new List<string>();
            context.Items.Add(SizesCollection, sizes);
            await output.GetChildContentAsync();

            var s = string.Join(", ", sizes.Concat(new[] { $"{DefaultWidth}px" }));
            image.MergeAttribute("sizes", s);

            // Fallback src for clients that don't honor srcset — use the largest declared width.
            imageUrlBuilder = imageUrlBuilder.WithWidth(responsiveWidths.Max());
        }

        image.MergeAttribute("src", $"{imageUrlBuilder.Url}");
        var titleToUse = Title ?? Asset.Description ?? string.Empty;
        image.MergeAttribute("alt", titleToUse);
        image.MergeAttribute("title", titleToUse);
        output.MergeAttributes(image);
    }
}
