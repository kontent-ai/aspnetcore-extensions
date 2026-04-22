using Kontent.Ai.AspNetCore.RichText;
using Kontent.Ai.Delivery.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kontent.Ai.AspNetCore.Tests;

public class RichTextServiceCollectionExtensionsTests
{
    [Fact]
    public void AddKontentRichText_RegistersIHtmlResolver()
    {
        var services = new ServiceCollection();
        services.AddKontentRichText();

        using var provider = services.BuildServiceProvider();
        var resolver = provider.GetRequiredService<IHtmlResolver>();

        Assert.NotNull(resolver);
    }

    [Fact]
    public void AddKontentRichText_RegistersAsSingleton()
    {
        var services = new ServiceCollection();
        services.AddKontentRichText();

        using var provider = services.BuildServiceProvider();
        var first = provider.GetRequiredService<IHtmlResolver>();
        var second = provider.GetRequiredService<IHtmlResolver>();

        Assert.Same(first, second);
    }

    [Fact]
    public async Task AddKontentRichText_WithoutConfigure_ProducesBuiltInResolver()
    {
        var services = new ServiceCollection();
        services.AddKontentRichText();

        using var provider = services.BuildServiceProvider();
        var resolver = provider.GetRequiredService<IHtmlResolver>();

        var result = await resolver.ResolveAsync(new EmptyRichTextContent());
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void AddKontentRichText_WithConfigure_InvokesConfigurator()
    {
        var configCalled = false;
        var services = new ServiceCollection();
        services.AddKontentRichText(_ => configCalled = true);

        using var provider = services.BuildServiceProvider();
        _ = provider.GetRequiredService<IHtmlResolver>();

        Assert.True(configCalled);
    }

    [Fact]
    public void AddKontentRichText_CalledTwice_LastConfigurationWins()
    {
        var firstCalled = false;
        var secondCalled = false;
        var services = new ServiceCollection();

        services.AddKontentRichText(_ => firstCalled = true);
        services.AddKontentRichText(_ => secondCalled = true);

        using var provider = services.BuildServiceProvider();
        _ = provider.GetRequiredService<IHtmlResolver>();

        Assert.False(firstCalled);
        Assert.True(secondCalled);
    }
}
