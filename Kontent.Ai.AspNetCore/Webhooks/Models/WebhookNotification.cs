using System;
using System.Text.Json.Serialization;

namespace Kontent.Ai.AspNetCore.Webhooks.Models
{
    /// <summary>
    /// Root object of a Kontent.ai triggered webhook.
    /// See <see href="https://kontent.ai/learn/docs/webhooks/webhooks/net">webhooks reference documentation</see> for details.
    /// </summary>
    public class WebhookNotification
    {
        /// <summary>
        /// A collection of webhook notifications for each modified object.
        /// </summary>
        [JsonPropertyName("notifications"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public WebhookModel[] Notifications { get; set; }
    }

    /// <summary>
    /// The webhook model that contains data and message.
    /// </summary>
    public class WebhookModel
    {
        /// <summary>
        /// Data relevant to the object that triggered the webhook.
        /// </summary>
        [JsonPropertyName("data"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public WebhookData Data { get; set; }
        /// <summary>
        /// The Message object contains information about the origin of the notification.
        /// </summary>
        [JsonPropertyName("message"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public WebhookMessage Message { get; set; }
    }

    /// <summary>
    /// Data relevant to the object that triggered the webhook.
    /// </summary>
    public class WebhookData
    {
        /// <summary>
        /// Metadata of the modified object.
        /// </summary>
        [JsonPropertyName("system"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public WebhookItem System { get; set; }
    }

    /// <summary>
    /// The Message object contains information about the origin of the notification.
    /// </summary>
    public class WebhookMessage
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
    /// Represents metadata of the modified object.
    /// </summary>
    public class WebhookItem
    {
        /// <summary>
        /// The object's ID.
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
