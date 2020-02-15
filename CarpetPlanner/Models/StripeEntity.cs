using System.ComponentModel.DataAnnotations.Schema;

namespace CarpetPlanner.Models
{
    /// <summary>
    /// StripeEntity
    /// </summary>
    public class StripeEntity
    {
        /// <summary>
        /// Default RGB color string. 
        /// </summary>
        public const string DefaultColor = "#000000";
        
        /// <summary>
        /// Identifier
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Related carpet identifier
        /// </summary>
        public int CarpetId { get; set; }

        /// <summary>
        /// Reference to color entity
        /// </summary>
        public int Color { get; set; }
        
        /// <summary>
        /// RGB string for Color
        /// </summary>
        [NotMapped]
        public string ColorString { get; set; }
        
        /// <summary>
        /// Stripe height in centimeters
        /// </summary>
        public double Height { get; set; }
    }
}