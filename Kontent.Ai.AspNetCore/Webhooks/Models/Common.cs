using System;
using System.Text.Json.Serialization;

namespace Kontent.Ai.AspNetCore.Webhooks.Models
{
    /// <summary>
    /// The Legacy Message object contains information about the origin of the notification.
    /// </summary>
    public class LegacyMessage
    {
        /// <summary>
        /// Identifier of the webhook.
        /// </summary>
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Identifier of a Kontent.ai project.
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

    /// <summary>
    /// The Message object contains information about the origin of the notification.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Identifier of the webhook.
        /// </summary>
        [JsonPropertyName("environment_id")]
        public Guid EnvironmentId { get; set; }

        /// <summary>
        /// Type of the object that triggered the webhook (content_item_variant, taxonomy, ...)
        /// </summary>
        [JsonPropertyName("object_type")]
        public string ObjectType { get; set; }

        /// <summary>
        /// Codename of the action that triggered the webhook (published, unpublished, created, deleted, ...).
        /// </summary>
        [JsonPropertyName("action")]
        public string Action { get; set; }

        /// <summary>
        /// Codename of the delivery slot where the webhook was triggered (preview, published).
        /// </summary>
        [JsonPropertyName("delivery_slot")]
        public string DeliverySlot { get; set; }
    }

    /// <summary>
    /// Interface used as base for webhook item.
    /// </summary>
    public interface IWebhookItem
    {
         /// <summary>
        /// The item's ID.
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// The item's codename.
        /// </summary>
        public string Codename { get; set; }

        /// <summary>
        /// The item's collection.
        /// </summary>
        public string Collection { get; set; }

        /// <summary>
        /// Codename of the item's language.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// The item's type.
        /// </summary>
        public string Type { get; set; }
    }
}
