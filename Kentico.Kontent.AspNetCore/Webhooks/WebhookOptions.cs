namespace Kentico.Kontent.AspNetCore.Webhooks
{
    /// <summary>
    /// A configuration object that allows to adjust the Kentico Kontent webhook behavior.
    /// </summary>
    public class WebhookOptions
    {
        /// <summary>
        /// Webhook secret used for webhook signature validation.
        /// </summary>
        public string Secret { get; set; }
    }
}
