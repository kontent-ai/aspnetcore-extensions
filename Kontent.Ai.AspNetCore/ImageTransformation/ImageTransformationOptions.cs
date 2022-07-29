namespace Kontent.Ai.AspNetCore.ImageTransformation
{
    /// <summary>
    /// A configuration object allowing to adjust the image transformation behavior.
    /// </summary>
    public class ImageTransformationOptions
    {
        /// <summary>
        /// Widths in which a given image is available. This property is used to generate the resulting srcset.
        /// </summary>
        public int[] ResponsiveWidths { get; set; }
    }
}
