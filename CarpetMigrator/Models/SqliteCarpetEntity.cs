﻿namespace CarpetMigrator.Models
{
    /// <summary>
    /// CarpetEntity
    /// </summary>
    public class SqliteCarpetEntity
    {
        /// <summary>
        /// Identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ObjectId of the user who has created the carpet
        /// </summary>
        public string Owner { get; set; }

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
