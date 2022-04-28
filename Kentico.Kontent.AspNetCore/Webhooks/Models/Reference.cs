using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Kentico.Kontent.AspNetCore.Webhooks.Models
{
    /// <summary>
    /// A reference object
    /// </summary>
    public class Reference
    {
        /// <summary>
        /// ID reference to an object.
        /// </summary>
        [JsonPropertyName("id"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Guid? Id { get; set; }

        /// <summary>
        /// Codename reference to an object.
        /// </summary>
        [JsonPropertyName("codename"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Codename { get; set; }

        /// <summary>
        /// External ID reference to an object.
        /// </summary>
        [JsonPropertyName("external_id"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ExternalId { get; set; }

        /// <summary>
        /// Creates the reference by id.
        /// </summary>
        /// <param name="id">The id of the identifier.</param>
        public static Reference ById(Guid id) => new() { Id = id };

        /// <summary>
        /// Creates the reference by codename.
        /// </summary>
        /// <param name="codename">Codename of the identifier.</param>
        public static Reference ByCodename(string codename) => new() { Codename = codename };

        /// <summary>
        /// Creates the reference by external ID.
        /// </summary>
        /// <param name="externalId">External ID of the identifier.</param>
        public static Reference ByExternalId(string externalId) => new() { ExternalId = externalId };
    }
}
