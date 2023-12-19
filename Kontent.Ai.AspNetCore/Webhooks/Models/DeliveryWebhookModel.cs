using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Kontent.Ai.AspNetCore.Webhooks.Models
{
    /// <summary>
    /// Root object of a Kontent.ai delivery or preview-delivery API triggered webhook.
    /// See <see href="https://kontent.ai/learn/docs/webhooks/webhooks/net">webhooks reference documentation</see>  for details.
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// A collection of webhook model objects for each modified content item.
        /// </summary>
        [JsonPropertyName("notifications"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<DeliveryWebhookData> Notifications { get; set; }
    }

    /// <summary>
    /// The webhook model that contains data and message.
    /// </summary>
    public class DeliveryWebhookModel
    {
        /// <summary>
        /// Data relevant to the operation that triggered the webhook.
        /// </summary>
        [JsonPropertyName("data"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DeliveryWebhookData Data { get; set; }
        /// <summary>
        /// The Message object contains information about the origin of the notification.
        /// </summary>
        [JsonPropertyName("message"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Message Message { get; set; }
    }

    /// <summary>
    /// Data relevant to the operation that triggered the webhook.
    /// </summary>
    public class DeliveryWebhookData
    {
        /// <summary>
        /// The modified content item.
        /// </summary>
        [JsonPropertyName("system"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DeliveryWebhookItem System { get; set; }
    }

    /// <summary>
    /// A Content item object.
    /// </summary>
    public class DeliveryWebhookItem : IWebhookItem
    {
        /// <summary>
        /// The item's ID.
        /// </summary>
        [JsonPropertyName("id"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Guid? Id { get; set; }

        /// <summary>
        /// The item's name.
        /// </summary>
        [JsonPropertyName("name"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Name { get; set; }

        /// <summary>
        /// The item's codename.
        /// </summary>
        [JsonPropertyName("codename"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Codename { get; set; }

        /// <summary>
        /// The item's collection.
        /// </summary>
        [JsonPropertyName("collection"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Collection { get; set; }

        /// <summary>
        /// The item's workflow.
        /// </summary>
        [JsonPropertyName("workflow"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Workflow { get; set; }

        /// <summary>
        /// The item's workflow step.
        /// </summary>
        [JsonPropertyName("workflow_step"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string WorkflowStep { get; set; }

        /// <summary>
        /// Codename of the item's language.
        /// </summary>
        [JsonPropertyName("language"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Language { get; set; }

        /// <summary>
        /// The item's type.
        /// </summary>
        [JsonPropertyName("type"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Type { get; set; }

        /// <summary>
        /// Timestamp of when the item was modified.
        /// </summary>
        [JsonPropertyName("last_modified")]
        public DateTime LastModified { get; set; }
    }
}
