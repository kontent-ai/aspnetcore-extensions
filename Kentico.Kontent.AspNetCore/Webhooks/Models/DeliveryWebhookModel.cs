using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Kentico.Kontent.AspNetCore.Webhooks.Models
{
    /// <summary>
    /// Root object of a Kontent delivery or preview-delivery API triggered webhook.
    /// See <see href="https://kontent.ai/learn/reference/webhooks-reference/">webhooks reference documentation</see>  for details.
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
        /// A collection of Item objects for each modified content item.
        /// </summary>
        [JsonPropertyName("items"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DeliveryWebhookItem[] Items { get; set; }

        /// <summary>
        /// A collection of  Taxonomy group objects.
        /// </summary>
        [JsonPropertyName("taxonomies"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Taxonomy[] Taxonomies { get; set; }
    }

    /// <summary>
    /// A Content item object.
    /// </summary>
    public class DeliveryWebhookItem
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
