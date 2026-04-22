using System.Text.Encodings.Web;
using Kontent.Ai.AspNetCore.RichText;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Xunit;

namespace Kontent.Ai.AspNetCore.Tests;

public class RichTextTagHelperTests
{
    [Fact]
    public async Task ProcessAsync_WithContent_EmitsResolvedHtml()
    {
        var resolver = new RecordingHtmlResolver { ReturnValue = "<p>body</p>" };
        var helper = new RichTextTagHelper(resolver) { Content = new EmptyRichTextContent() };
        var output = CreateOutput();

        await helper.ProcessAsync(CreateContext(), output);

        Assert.Equal("<p>body</p>", RenderContent(output));
    }

    [Fact]
    public async Task ProcessAsync_StripsWrapperTag()
    {
        var helper = new RichTextTagHelper { Content = new EmptyRichTextContent() };
        var output = CreateOutput();

        await helper.ProcessAsync(CreateContext(), output);

        Assert.Null(output.TagName);
    }

    [Fact]
    public async Task ProcessAsync_WithoutContent_IsNoOp()
    {
        var resolver = new RecordingHtmlResolver();
        var helper = new RichTextTagHelper(resolver); // Content not set
        var output = CreateOutput();

        await helper.ProcessAsync(CreateContext(), output);

        Assert.Equal(0, resolver.CallCount);
        Assert.Equal(string.Empty, RenderContent(output));
    }

    [Fact]
    public async Task ProcessAsync_ExplicitResolverAttribute_BeatsConstructorResolver()
    {
        var constructorResolver = new RecordingHtmlResolver { ReturnValue = "constructor" };
        var attributeResolver = new RecordingHtmlResolver { ReturnValue = "attribute" };
        var helper = new RichTextTagHelper(constructorResolver)
        {
            Content = new EmptyRichTextContent(),
            Resolver = attributeResolver
        };
        var output = CreateOutput();

        await helper.ProcessAsync(CreateContext(), output);

        Assert.Equal(1, attributeResolver.CallCount);
        Assert.Equal(0, constructorResolver.CallCount);
        Assert.Equal("attribute", RenderContent(output));
    }

    [Fact]
    public async Task ProcessAsync_ConstructorResolver_UsedWhenNoAttribute()
    {
        var constructorResolver = new RecordingHtmlResolver { ReturnValue = "constructor" };
        var helper = new RichTextTagHelper(constructorResolver) { Content = new EmptyRichTextContent() };
        var output = CreateOutput();

        await helper.ProcessAsync(CreateContext(), output);

        Assert.Equal(1, constructorResolver.CallCount);
        Assert.Equal("constructor", RenderContent(output));
    }

    [Fact]
    public async Task ProcessAsync_NoResolverAnywhere_UsesSdkBuiltInDefaults()
    {
        // No DI-injected resolver, no attribute resolver — falls back to new HtmlResolverBuilder().Build().
        var helper = new RichTextTagHelper { Content = new EmptyRichTextContent() };
        var output = CreateOutput();

        await helper.ProcessAsync(CreateContext(), output);

        Assert.Equal(string.Empty, RenderContent(output));
    }

    private static TagHelperContext CreateContext() =>
        new(new TagHelperAttributeList(), new Dictionary<object, object?>(), Guid.NewGuid().ToString("N"));

    private static TagHelperOutput CreateOutput() =>
        new("rich-text",
            new TagHelperAttributeList(),
            (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

    private static string RenderContent(TagHelperOutput output)
    {
        using var writer = new StringWriter();
        output.Content.WriteTo(writer, HtmlEncoder.Default);
        return writer.ToString();
    }
}
