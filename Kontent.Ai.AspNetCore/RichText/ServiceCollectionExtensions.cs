using Kontent.Ai.Delivery.Abstractions;
using Kontent.Ai.Delivery.ContentItems.RichText.Resolution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kontent.Ai.AspNetCore.RichText;

/// <summary>
/// Extension methods on <see cref="IServiceCollection"/> for registering Kontent.ai rich-text services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers an <see cref="IHtmlResolver"/> singleton used by the <c>&lt;rich-text&gt;</c> tag helper and <see cref="RichTextExtensions.ToHtmlContentAsync"/>.
    /// </summary>
    /// <remarks>
    /// Calling this method replaces any prior <see cref="IHtmlResolver"/> registration. The resolver is built lazily on
    /// first resolution; the <paramref name="configure"/> callback runs at that time.
    /// </remarks>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Optional configuration callback that receives the builder used to assemble the resolver.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddKontentRichText(
        this IServiceCollection services,
        Action<IHtmlResolverBuilder>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.RemoveAll<IHtmlResolver>();
        services.AddSingleton<IHtmlResolver>(_ =>
        {
            var builder = new HtmlResolverBuilder();
            configure?.Invoke(builder);
            return builder.Build();
        });
        return services;
    }
}
