namespace CarpetPlanner.Models
{
    using Newtonsoft.Json;
    
    /// <summary>
    /// Class for presenting update values for CarpetEntity.
    /// </summary>
    public class CarpetPatch
    {
        /// <summary>
        /// Carpet name
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Carpet width in centimeters
        /// </summary>
        [JsonProperty(PropertyName = "width")]
        public int? Width { get; set; }
    }
}