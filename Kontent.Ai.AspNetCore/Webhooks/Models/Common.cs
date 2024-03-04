using System;

namespace Kontent.Ai.AspNetCore.Webhooks.Models
{
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
