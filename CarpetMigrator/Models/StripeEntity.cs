using System.ComponentModel.DataAnnotations.Schema;

namespace CarpetMigrator.Models
{
    /// <summary>
    /// StripeEntity
    /// </summary>
    public class StripeEntity
    {
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
        /// Stripe height in centimeters
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Stripe place in carpet.
        /// </summary>
        public int Ordinal { get; set; }
    }
}
