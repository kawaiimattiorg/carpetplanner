namespace CarpetPlannerB2c.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using Newtonsoft.Json;

    /// <summary>
    /// CarpetEntity
    /// </summary>
    public class CarpetEntity
    {
        /// <summary>
        /// Identifier
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        /// <summary>
        /// ObjectId of the user who has created the carpet
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// Carpet name
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Reference to stripe that separates every stripe
        /// </summary>
        public int StripeSeparator { get; set; }

        /// <summary>
        /// Carpet width in centimeters
        /// </summary>
        [JsonProperty(PropertyName = "width")]
        public int Width { get; set; }

        /// <summary>
        /// Is the carpet removed.
        /// </summary>
        [JsonIgnore]
        public bool Removed { get; set; }

        /// <summary>
        /// List of stripes related to this carpet.
        /// </summary>
        [JsonProperty(PropertyName = "stripes")]
        [NotMapped]
        public IList<StripeEntity> Stripes { get; set; }
    }
}
