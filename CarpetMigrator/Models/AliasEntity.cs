namespace CarpetMigrator.Models
{
    /// <summary>
    /// AliasEntity
    /// </summary>
    public class AliasEntity
    {
        /// <summary>
        /// Identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// User's object id in B2C
        /// </summary>
        public string ObjectId { get; set; }

        /// <summary>
        /// Display name
        /// </summary>
        public string Alias { get; set; }
    }
}
