using System;
using System.Text.Json.Serialization;

namespace Kontent.Ai.AspNetCore.Webhooks.Models
{
    /// <summary>
    /// Root object of a Kontent.ai delivery or preview-delivery API triggered legacy webhook.
    /// See <see href="https://kontent.ai/learn/reference/webhooks-reference/">legacy webhooks reference documentation</see> for details.
    /// </summary>
    public class LegacyWebhookModel
    {
        /// <summary>
        /// Data relevant to the operation that triggered the webhook.
        /// </summary>
        [JsonPropertyName("data"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public LegacyWebhookData Data { get; set; }
        /// <summary>
        /// The Message object contains information about the origin of the notification.
        /// </summary>
        [JsonPropertyName("message"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public LegacyMessage Message { get; set; }
    }

    /// <summary>
    /// Data relevant to the operation that triggered the webhook.
    /// </summary>
    public class LegacyWebhookData
    {
        /// <summary>
        /// A collection of Item objects for each modified content item.
        /// </summary>
        [JsonPropertyName("items"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public LegacyWebhookItem[] Items { get; set; }

        /// <summary>
        /// A collection of  Taxonomy group objects.
        /// </summary>
        [JsonPropertyName("taxonomies"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Taxonomy[] Taxonomies { get; set; }
    }

    /// <summary>
    /// A Content item object.
    /// </summary>
    public class LegacyWebhookItem : IWebhookItem
    {
        /// <summary>
        /// The item's ID.
        /// </summary>
        [JsonPropertyName("id"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Guid? Id { get; set; }

        /// <summary>
        /// The item's codename.
        /// </summary>
        [JsonPropertyName("codename"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Codename { get; set; }

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
        /// Codename of the collection the item belongs to.
        /// </summary>
        [JsonPropertyName("collection"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Collection { get; set; }
    }
}
