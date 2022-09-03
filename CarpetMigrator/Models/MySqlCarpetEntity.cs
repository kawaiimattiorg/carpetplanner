namespace CarpetMigrator.Models
{
    /// <summary>
    /// CarpetEntity
    /// </summary>
    public class MySqlCarpetEntity
    {
        /// <summary>
        /// Identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Carpet name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Reference to stripe that separates every stripe
        /// </summary>
        public int StripeSeparator { get; set; }

        /// <summary>
        /// Carpet width in centimeters
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Is the carpet removed.
        /// </summary>
        public bool Removed { get; set; }
    }
}
