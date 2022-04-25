using System;
using System.Text.Json.Serialization;

namespace Kentico.Kontent.AspNetCore.Webhooks.Models
{
    /// <summary>
    /// Root object of a Kontent management API triggered webhook.
    /// </summary>
    public class ManagementWebhookModel
    {
        /// <summary>
        /// Data relevant to the operation that triggered the webhook.
        /// </summary>
        [JsonPropertyName("data"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), JsonPropertyOrder(1)]
        public ManagementWebhookData Data { get; set; }

        /// <summary>
        /// The Message object contains information about the origin of the notification.
        /// </summary>
        [JsonPropertyName("message"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), JsonPropertyOrder(2)]
        public Message Message { get; set; }
    }

    /// <summary>
    /// Data relevant to the operation that triggered the webhook.
    /// </summary>
    public class ManagementWebhookData
    {
        /// <summary>
        /// A collection of Item objects for each modified content item.
        /// </summary>
        [JsonPropertyName("items"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), JsonPropertyOrder(1)]
        public ManagementWebhookItem[] Items { get; set; }

        /// <summary>
        /// A collection of Taxonomy group objects.
        /// </summary>
        [JsonPropertyName("taxonomies"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), JsonPropertyOrder(2)]
        public Taxonomy[] Taxonomies { get; set; }
    }

    /// <summary>
    /// A Content item object.
    /// </summary>
    public class ManagementWebhookItem
    {
        /// <summary>
        /// Reference to an item.
        /// </summary>
        [JsonPropertyName("item"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), JsonPropertyOrder(1)]
        public Reference Item { get; set; }

        /// <summary>
        /// Reference to a language.
        /// </summary>
        [JsonPropertyName("language"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), JsonPropertyOrder(2)]
        public Reference Language { get; set; }

        /// <summary>
        /// Reference to the workflow step being transitioned from.
        /// </summary>
        [JsonPropertyName("transition_from"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), JsonPropertyOrder(3)]
        public Reference TransitionFrom { get; set; }

        /// <summary>
        /// Reference to the workflow step being transitioned to.
        /// </summary>
        [JsonPropertyName("transition_to"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), JsonPropertyOrder(4)]
        public Reference TransitionTo { get; set; }
    }
}
