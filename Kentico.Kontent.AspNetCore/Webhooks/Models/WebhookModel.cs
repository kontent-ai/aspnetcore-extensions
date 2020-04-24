using System;
using System.Text.Json.Serialization;

namespace Kentico.Kontent.AspNetCore.Webhooks.Models
{
    /// <summary>
    /// Root object of a Kentico Kontent webhook.
    /// </summary>
    public class WebhookModel
    {
        /// <summary>
        /// The Message object contains information about the origin of the notification.
        /// </summary>
        [JsonPropertyName("message")]
        public Message Message { get; set; }

        /// <summary>
        /// Data relevant to the operation that triggered the webhook.
        /// </summary>
        [JsonPropertyName("data")]
        public Data Data { get; set; }
    }

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
        /// Identifier of a Kentico Kontent project.
        /// </summary>
        [JsonPropertyName("project_id")]
        public Guid ProjectId { get; set; }

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
    /// Data relevant to the operation that triggered the webhook.
    /// </summary>
    public class Data
    {
        /// <summary>
        /// A collection of Item objects for each modified content item.
        /// </summary>
        [JsonPropertyName("items")]
        public Item[] Items { get; set; }

        /// <summary>
        /// A collection of Taxonomy group objects.
        /// </summary>
        [JsonPropertyName("taxonomies")]
        public Taxonomy[] Taxonomies { get; set; }
    }

    /// <summary>
    /// A Content item object.
    /// </summary>
    public class Item
    {
        /// <summary>
        /// The content item's internal ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The language's codename.
        /// </summary>
        [JsonPropertyName("language")]
        public string Language { get; set; }

        /// <summary>
        /// The content type's codename.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The content item's codename.
        /// </summary>
        [JsonPropertyName("codename")]
        public string Codename { get; set; }
    }

    /// <summary>
    /// A Taxonomy group object.
    /// </summary>
    public class Taxonomy
    {
        /// <summary>
        /// The taxonomy group's internal ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The taxonomy group's codename.
        /// </summary>
        [JsonPropertyName("codename")]
        public string Codename { get; set; }
    }
}
