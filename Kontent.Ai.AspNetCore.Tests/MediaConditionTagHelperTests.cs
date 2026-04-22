using Kontent.Ai.AspNetCore.ImageTransformation;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Xunit;

namespace Kontent.Ai.AspNetCore.Tests;

public class MediaConditionTagHelperTests
{
    [Fact]
    public async Task ProcessAsync_MinWidthOnly_AddsSizeEntry()
    {
        var sizes = new List<string>();
        var context = CreateContextWithSizes(sizes);
        var helper = new MediaConditionTagHelper { MinWidth = 769, ImageWidth = 300 };

        await helper.ProcessAsync(context, CreateOutput());

        Assert.Equal("(min-width: 769px) 300px", Assert.Single(sizes));
    }

    [Fact]
    public async Task ProcessAsync_WithMaxWidth_AddsCombinedEntry()
    {
        var sizes = new List<string>();
        var context = CreateContextWithSizes(sizes);
        var helper = new MediaConditionTagHelper { MinWidth = 330, MaxWidth = 768, ImageWidth = 689 };

        await helper.ProcessAsync(context, CreateOutput());

        Assert.Equal("(max-width: 768px) and (min-width: 330px) 689px", Assert.Single(sizes));
    }

    [Fact]
    public async Task ProcessAsync_WithoutSizesCollection_DoesNotThrow()
    {
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object?>(),
            Guid.NewGuid().ToString("N"));
        var helper = new MediaConditionTagHelper { MinWidth = 769, ImageWidth = 300 };
        var output = CreateOutput();

        await helper.ProcessAsync(context, output);

        Assert.Null(output.TagName);
    }

    [Fact]
    public async Task ProcessAsync_SuppressesOutput()
    {
        var sizes = new List<string>();
        var context = CreateContextWithSizes(sizes);
        var helper = new MediaConditionTagHelper { MinWidth = 100, ImageWidth = 100 };
        var output = CreateOutput();

        await helper.ProcessAsync(context, output);

        Assert.Null(output.TagName);
    }

    private static TagHelperContext CreateContextWithSizes(List<string> sizes)
    {
        var items = new Dictionary<object, object?>
        {
            { AssetTagHelper.SizesCollection, sizes }
        };
        return new TagHelperContext(
            new TagHelperAttributeList(),
            items,
            Guid.NewGuid().ToString("N"));
    }

    private static TagHelperOutput CreateOutput() =>
        new(
            "media-condition",
            new TagHelperAttributeList(),
            (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
}
