using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Kentico.Kontent.AspNetCore.Middleware.Webhook
{
    /// <summary>
    /// Verifies signatures of Kentico Kontent webhooks.
    /// </summary>
    public class SignatureMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// A configuration object that allows to adjust the Kentico Kontent webhook behavior.
        /// It contains a webhook secret used for signature validation.
        /// </summary>
        public IOptions<WebhookOptions> WebhookOptions { get; }

        /// <summary>
        /// Creates an instance of the <see cref="SignatureMiddleware"/>.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="webhookOptions">A configuration object that allows to adjust the Kentico Kontent webhook behavior.</param>
        public SignatureMiddleware(RequestDelegate next, IOptions<WebhookOptions> webhookOptions)
        {
            _next = next;
            WebhookOptions = webhookOptions;
        }

        /// <summary>
        /// Processes the request to validate the webhook signature.
        /// </summary>
        /// <param name="httpContext">HTTP context whose request to inspect.</param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext httpContext)
        {
            var request = httpContext.Request;
            request.EnableBuffering();

            using var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true);
            var content = await reader.ReadToEndAsync();
            request.Body.Seek(0, SeekOrigin.Begin);

            var generatedSignature = GenerateHash(content, WebhookOptions.Value.Secret);
            var signature = request.Headers["X-KC-Signature"].FirstOrDefault();

            if (generatedSignature != signature)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            await _next(httpContext);
        }

        private static string GenerateHash(string message, string secret)
        {
            secret ??= "";
            var safeUtf8 = new UTF8Encoding(false, true);
            var keyBytes = safeUtf8.GetBytes(secret);
            var messageBytes = safeUtf8.GetBytes(message);

            using var hmacsha256 = new HMACSHA256(keyBytes);
            var hashMessage = hmacsha256.ComputeHash(messageBytes);

            return Convert.ToBase64String(hashMessage);
        }
    }
}