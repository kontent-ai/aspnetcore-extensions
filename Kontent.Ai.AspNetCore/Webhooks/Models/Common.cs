using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Kontent.Ai.AspNetCore.Webhooks.Models
{
    /// <summary>
    /// The Message object contains information about the origin of the notification.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Identifier of the webhook.
        /// </summary>
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Identifier of a Kontent project.
        /// </summary>
        [JsonPropertyName("project_id")]
        public Guid? ProjectId { get; set; }

        /// <summary>
        /// Type of the object that triggered the webhook (content_item_variant, taxonomy, ...)
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// Codename of the operation that triggered the webhook.
        /// </summary>
        [JsonPropertyName("operation")]
        public string Operation { get; set; }

        /// <summary>
        /// Name of the API endpoint, e.g. content_management or delivery_production.
        /// </summary>
        [JsonPropertyName("api_name")]
        public string ApiName { get; set; }

        /// <summary>
        /// Timestamp of the webhook.
        /// </summary>
        [JsonPropertyName("created_timestamp")]
        public DateTime CreatedTimestamp { get; set; }

        /// <summary>
        /// Publicly available URL address of your webhook endpoint.
        /// </summary>
        [JsonPropertyName("webhook_url")]
        public string WebhookUrl { get; set; }
    }

    /// <summary>
    /// A Taxonomy group object.
    /// </summary>
    public class Taxonomy
    {
        /// <summary>
        /// The taxonomy group's internal ID.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The taxonomy group's codename.
        /// </summary>
        [JsonPropertyName("codename")]
        public string Codename { get; set; }
    }
}
