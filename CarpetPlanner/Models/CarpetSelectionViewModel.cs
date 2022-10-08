namespace CarpetPlanner.Models
{
    using System.Collections.Generic;

    public class CarpetSelectionViewModel
    {
        /// <summary>
        /// Current user username.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Whether New Carpet button is visible
        /// </summary>
        public bool NewCarpetVisible { get; set; }

        /// <summary>
        /// All current user carpets.
        /// </summary>
        public IList<CarpetEntity> Carpets { get; set; }
    }
}
