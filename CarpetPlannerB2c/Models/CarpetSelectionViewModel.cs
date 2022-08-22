namespace CarpetPlannerB2c.Models
{
    using System.Collections.Generic;

    public class CarpetSelectionViewModel
    {
        /// <summary>
        /// Current user username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// All current user carpets.
        /// </summary>
        public IList<CarpetEntity> Carpets { get; set; }
    }
}
