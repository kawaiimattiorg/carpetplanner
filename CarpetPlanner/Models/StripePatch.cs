namespace CarpetPlanner.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    
    /// <summary>
    /// Class for presenting update values for Stripes.
    /// </summary>
    public class StripePatch
    {
        /// <summary>
        /// List of stripes that the change impacts
        /// </summary>
        [JsonProperty(PropertyName = "stripes")]
        public IList<int> Stripes { get; set; }

        /// <summary>
        /// Stripe color id
        /// </summary>
        [JsonProperty(PropertyName = "color")]
        public int? Color { get; set; }
        
        /// <summary>
        /// Stripe color RGB
        /// </summary>
        [JsonProperty(PropertyName = "rgb")]
        public string Rgb { get; set; }

        /// <summary>
        /// Stripe length in centimeters
        /// </summary>
        [JsonProperty(PropertyName = "height")]
        public double? Height { get; set; }

        /// <summary>
        /// Should the stripe be removed
        /// </summary>
        [JsonProperty(PropertyName = "remove")]
        public bool Remove { get; set; }
    }
}