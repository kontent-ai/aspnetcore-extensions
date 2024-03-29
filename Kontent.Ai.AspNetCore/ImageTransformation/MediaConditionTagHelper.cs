﻿using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kontent.Ai.AspNetCore.ImageTransformation
{
    /// <summary>
    /// Represents one media condition from image sizes attribute.
    /// </summary>
    [HtmlTargetElement(ParentTag = "img-asset")]
    public class MediaConditionTagHelper : TagHelper
    {
        /// <summary>
        /// Minimum width of the window that should trigger usage of the <see cref="ImageWidth"/>.
        /// </summary>
        [HtmlAttributeName("min-width")]
        public int MinWidth { get; set; }

        /// <summary>
        /// Optional maximum width of the window that should trigger usage of the <see cref="ImageWidth"/>.
        /// </summary>
        [HtmlAttributeName("max-width")]
        public int? MaxWidth { get; set; }

        /// <summary>
        /// The width of an image when the width of the window is between <see cref="MinWidth"/> and <see cref="MaxWidth"/>.
        /// </summary>
        [HtmlAttributeName("image-width")]
        public int ImageWidth { get; set; }

        /// <inheritdoc/>
        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (context.Items.ContainsKey(AssetTagHelper.SizesCollection))
            {
                if (context.Items[AssetTagHelper.SizesCollection] is List<string> sizes)
                {
                    var maxWidth = MaxWidth.HasValue ? $"(max-width: {MaxWidth.Value}px) and " : string.Empty;
                    sizes.Add($"{maxWidth}(min-width: {MinWidth}px) {ImageWidth}px");
                }
            }
            output.SuppressOutput();

            return Task.CompletedTask;
        }
    }
}