using Kontent.Ai.AspNetCore.ImageTransformation;
using Kontent.Ai.Delivery.Abstractions;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using Xunit;

namespace Kontent.Ai.AspNetCore.Tests;

public class AssetTagHelperTests
{
    private const string AssetUrl = "https://assets.example.com/folder/asset.jpg";

    [Fact]
    public async Task ProcessAsync_WithResponsiveWidthsAndMediaConditions_RendersImgWithSrcsetAndSizes()
    {
        var options = Options.Create(new ImageTransformationOptions
        {
            ResponsiveWidths = new[] { 200, 400, 800 }
        });
        var helper = new AssetTagHelper(options)
        {
            Asset = new TestAsset { Url = AssetUrl, Description = "Coffee" },
            DefaultWidth = 300
        };

        var context = CreateContext();
        var output = CreateOutput(getChildContent: async () =>
        {
            var media = new MediaConditionTagHelper { MinWidth = 769, ImageWidth = 300 };
            await media.ProcessAsync(context, CreateOutput());
        });

        await helper.ProcessAsync(context, output);

        Assert.Equal("img", output.TagName);
        Assert.Equal(TagMode.SelfClosing, output.TagMode);
        // ImageUrlBuilder is mutated in place during srcset generation, so src reflects
        // the last width iterated (not necessarily the largest). Widths here are ascending.
        Assert.Equal($"{AssetUrl}?w=800", AttrValue(output, "src"));
        Assert.Equal("Coffee", AttrValue(output, "alt"));
        Assert.Equal("Coffee", AttrValue(output, "title"));

        var srcset = AttrValue(output, "srcset");
        Assert.Contains($"{AssetUrl}?w=200 200w", srcset);
        Assert.Contains($"{AssetUrl}?w=400 400w", srcset);
        Assert.Contains($"{AssetUrl}?w=800 800w", srcset);

        Assert.Equal("(min-width: 769px) 300px, 300px", AttrValue(output, "sizes"));
    }

    [Fact]
    public async Task ProcessAsync_WithExplicitWidth_AppliesWidthAndSkipsSrcset()
    {
        var helper = new AssetTagHelper { Asset = new TestAsset { Url = AssetUrl } };
        var context = CreateContext(("width", 500));
        var output = CreateOutput();

        await helper.ProcessAsync(context, output);

        Assert.Equal($"{AssetUrl}?w=500", AttrValue(output, "src"));
        Assert.False(output.Attributes.ContainsName("srcset"));
        Assert.False(output.Attributes.ContainsName("sizes"));
    }

    [Fact]
    public async Task ProcessAsync_WithExplicitHeight_AppliesHeightAndSkipsSrcset()
    {
        var helper = new AssetTagHelper { Asset = new TestAsset { Url = AssetUrl } };
        var context = CreateContext(("height", 400));
        var output = CreateOutput();

        await helper.ProcessAsync(context, output);

        Assert.Equal($"{AssetUrl}?h=400", AttrValue(output, "src"));
        Assert.False(output.Attributes.ContainsName("srcset"));
    }

    [Fact]
    public async Task ProcessAsync_WithoutAsset_DoesNotRenderImg()
    {
        var helper = new AssetTagHelper();
        var context = CreateContext();
        var output = CreateOutput();

        await helper.ProcessAsync(context, output);

        Assert.Equal("img-asset", output.TagName);
        Assert.False(output.Attributes.ContainsName("src"));
    }

    [Fact]
    public async Task ProcessAsync_TitleAttribute_OverridesAssetDescription()
    {
        var helper = new AssetTagHelper
        {
            Asset = new TestAsset { Url = AssetUrl, Description = "Default" },
            Title = "Custom"
        };
        var context = CreateContext();
        var output = CreateOutput();

        await helper.ProcessAsync(context, output);

        Assert.Equal("Custom", AttrValue(output, "alt"));
        Assert.Equal("Custom", AttrValue(output, "title"));
    }

    [Fact]
    public async Task ProcessAsync_WithoutDescriptionAndTitle_UsesEmptyString()
    {
        var helper = new AssetTagHelper { Asset = new TestAsset { Url = AssetUrl } };
        var context = CreateContext();
        var output = CreateOutput();

        await helper.ProcessAsync(context, output);

        Assert.Equal(string.Empty, AttrValue(output, "alt"));
        Assert.Equal(string.Empty, AttrValue(output, "title"));
    }

    [Fact]
    public async Task ProcessAsync_WithoutOptionsOrResponsiveWidths_SkipsSrcset()
    {
        var helper = new AssetTagHelper { Asset = new TestAsset { Url = AssetUrl } };
        var context = CreateContext();
        var output = CreateOutput();

        await helper.ProcessAsync(context, output);

        Assert.Equal(AssetUrl, AttrValue(output, "src"));
        Assert.False(output.Attributes.ContainsName("srcset"));
    }

    [Fact]
    public async Task ProcessAsync_PerTagResponsiveWidths_OverridesOptions()
    {
        var options = Options.Create(new ImageTransformationOptions
        {
            ResponsiveWidths = new[] { 100, 200 }
        });
        var helper = new AssetTagHelper(options)
        {
            Asset = new TestAsset { Url = AssetUrl },
            ResponsiveWidths = new[] { 500, 1000 }
        };
        var context = CreateContext();
        var output = CreateOutput();

        await helper.ProcessAsync(context, output);

        var srcset = AttrValue(output, "srcset");
        Assert.Contains($"{AssetUrl}?w=500 500w", srcset);
        Assert.Contains($"{AssetUrl}?w=1000 1000w", srcset);
        Assert.DoesNotContain("w=100 100w", srcset);
        Assert.DoesNotContain("w=200 200w", srcset);
    }

    private static TagHelperContext CreateContext(params (string name, object value)[] attributes)
    {
        var attrs = new TagHelperAttributeList(
            attributes.Select(a => new TagHelperAttribute(a.name, a.value)));
        return new TagHelperContext(attrs, new Dictionary<object, object?>(), Guid.NewGuid().ToString("N"));
    }

    private static TagHelperOutput CreateOutput(string tagName = "img-asset", Func<Task>? getChildContent = null)
    {
        return new TagHelperOutput(
            tagName,
            new TagHelperAttributeList(),
            async (useCachedResult, encoder) =>
            {
                if (getChildContent != null)
                {
                    await getChildContent();
                }
                return new DefaultTagHelperContent();
            });
    }

    private static string AttrValue(TagHelperOutput output, string name) =>
        output.Attributes[name].Value?.ToString() ?? string.Empty;

    private sealed class TestAsset : IAsset
    {
        public string Url { get; init; } = "";
        public string? Description { get; init; }
        public int? Height { get; init; }
        public int? Width { get; init; }
        public string Name { get; init; } = "";
        public int Size { get; init; }
        public string Type { get; init; } = "";
        public IReadOnlyDictionary<string, IAssetRendition> Renditions { get; init; }
            = new Dictionary<string, IAssetRendition>();
    }
}
