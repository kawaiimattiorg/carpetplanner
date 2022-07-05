namespace CarpetPlanner6.Models
{
    /// <summary>
    /// ColorEntity
    /// </summary>
    public class ColorEntity
    {
        /// <summary>
        /// Identifier
        /// </summary>]
        public int Id { get; set; }

        /// <summary>
        /// Position in UI
        /// </summary>
        public int Ordinal { get; set; }

        /// <summary>
        /// Color RGB string
        /// </summary>
        public string Rgb { get; set; }

        /// <summary>
        /// Enabled
        /// </summary>
        public bool Removed { get; set; }
    }
}
