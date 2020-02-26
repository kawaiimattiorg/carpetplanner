namespace CarpetPlanner.Models
{
    using System.Collections.Generic;

    public class CarpetSelectionViewModel
    {
        /// <summary>
        /// All current user carpets.
        /// </summary>
        public IList<CarpetEntity> Carpets { get; set; }
    }
}