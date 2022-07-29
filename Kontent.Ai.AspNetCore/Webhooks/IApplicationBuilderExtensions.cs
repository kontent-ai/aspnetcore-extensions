using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Kontent.Ai.AspNetCore.Webhooks
{
    /// <summary>
    /// Provides webhook validation-related extension methods for the <see cref="IApplicationBuilder"/> interface.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {

        /// <summary>
        /// Applies the webhook signature validation to a path given by the <paramref name="predicate"/>.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder">application</see> to configure.</param>
        /// <param name="predicate">Invoked with the request environment to determine if the branch should be taken</param>
        /// <param name="options">A configuration object that allows to adjust the Kontent.ai webhook behavior.</param>
        /// <returns>The original <see cref="IApplicationBuilder"/>.</returns>
        public static IApplicationBuilder UseWebhookSignatureValidator(this IApplicationBuilder app, Func<HttpContext, bool> predicate, WebhookOptions options = null)
        {
            app.UseWhen(predicate, appBuilder =>
            {
                if (options != null)
                {
                    appBuilder.UseMiddleware<SignatureMiddleware>(Options.Create(options));
                }
                else
                {
                    appBuilder.UseMiddleware<SignatureMiddleware>();
                }
            });

            return app;
        }


        /// <summary>
        /// Applies the webhook signature validation to a path given by the <paramref name="predicate"/>.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder">application</see> to configure.</param>
        /// <param name="predicate">Invoked with the request environment to determine if the branch should be taken</param>
        /// <param name="configureOptions">Allows to configure the <see cref="WebhookOptions"/></param>
        /// <returns>The original <see cref="IApplicationBuilder"/>.</returns>
        public static IApplicationBuilder UseWebhookSignatureValidator(this IApplicationBuilder app, Func<HttpContext, bool> predicate, Action<WebhookOptions> configureOptions)
        {
            var options = new WebhookOptions();
            configureOptions(options);

            return app.UseWebhookSignatureValidator(predicate, options);
        }

        /// <summary>
        /// Applies the webhook signature validation to a path given by the <paramref name="predicate"/>.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder">application</see> to configure.</param>
        /// <param name="predicate">Invoked with the request environment to determine if the branch should be taken</param>
        /// <param name="configurationSection">Configuration section with <see cref="WebhookOptions"/></param>
        /// <returns>The original <see cref="IApplicationBuilder"/>.</returns>
        public static IApplicationBuilder UseWebhookSignatureValidator(this IApplicationBuilder app, Func<HttpContext, bool> predicate, IConfigurationSection configurationSection)
        {
            var options = new WebhookOptions();
            configurationSection.Bind(options);

            return app.UseWebhookSignatureValidator(predicate, options);
        }
    }
}
