using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kentico.Kontent.Delivery.Abstractions;
using Kentico.Kontent.ImageTransformation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace Kentico.Kontent.AspNetCore.ImageTransformation
{
    /// <summary>
    /// A tag helper that generates img elements based on assets stored in Kentico Kontent.
    /// </summary>
    [RestrictChildren("media-condition")]
    [HtmlTargetElement("img-asset", Attributes = "asset")]
    public class AssetTagHelper : TagHelper
    {
        private int[] responsiveWidths;

        /// <summary>
        /// Application settings.
        /// </summary>
        public IOptions<ImageTransformationOptions> ImageTransformationOptions { get; set; }

        /// <summary>
        /// Represents an asset object stored in Kentico Kontent. This property is mandatory in order to properly generate an img tag.
        /// </summary>
        [HtmlAttributeName("asset")]
        public Asset Asset { get; set; }

        /// <summary>
        /// Allows overriding the alt and title attributes of an image.
        /// </summary>
        [HtmlAttributeName("title")]
        public string Title { get; set; }

        /// <summary>
        /// The last parameter of the sizes attribute of an image.
        /// </summary>
        [HtmlAttributeName("default-width")]
        public int DefaultWidth { get; set; }

        /// <summary>
        /// Widths in which a given image is available. This property is used to generate the resulting srcset. This can also be set globally using <see cref="ImageTransformationOptions"/>.
        /// </summary>
        [HtmlAttributeName("responsive-widths")]
        public int[] ResponsiveWidths
        {
            get
            {
                if (responsiveWidths == null)
                {
                    return ImageTransformationOptions.Value.ResponsiveWidths;
                }
                return responsiveWidths;
            }
            set => responsiveWidths = value;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AssetTagHelper()
        {
        }

        /// <summary>
        /// Constructor that allows to set global the image transformation behavior.
        /// </summary>
        /// <param name="imageTransformationOption">An instance of a configuration object allowing to adjust the image transformation behavior.</param>
        public AssetTagHelper(IOptions<ImageTransformationOptions> imageTransformationOption) : this()
        {
            ImageTransformationOptions = imageTransformationOption;
        }

        /// <inheritdoc/>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (Asset == null)
            {
                base.Process(context, output);
                return;
            }

            output.TagName = "img";
            output.TagMode = TagMode.SelfClosing;


            var image = new TagBuilder("img");

            if (ResponsiveWidths != null && ResponsiveWidths.Any() && context.AllAttributes["width"]?.Value == null && context.AllAttributes["height"]?.Value == null)
            {
                image.MergeAttribute("srcset", GenerateSrcsetValue(Asset.Url, ImageTransformationOptions.Value.ResponsiveWidths));

                var sizes = new List<string>();
                context.Items.Add("sizes", sizes);
                await output.GetChildContentAsync();

                if (sizes != null)
                {
                    var s = string.Join(", ", sizes.Concat(new[] { $"{DefaultWidth}px" }));
                    image.MergeAttribute("sizes", s);
                }
            }

            image.MergeAttribute("src", $"{new ImageUrlBuilder(Asset.Url).Url}");
            var titleToUse = Title ?? Asset.Description ?? string.Empty;
            image.MergeAttribute("alt", titleToUse);
            image.MergeAttribute("title", titleToUse);
            output.MergeAttributes(image);
        }

        private static string GenerateSrcsetValue(string imageUrl, int[] widths)
        {
            return string.Join(",", widths.Select(w => $"{new ImageUrlBuilder(imageUrl).WithWidth(Convert.ToDouble(w)).Url} {w}w"));
        }
    }
}