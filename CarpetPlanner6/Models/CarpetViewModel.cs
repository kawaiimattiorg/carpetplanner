using System.Collections.Generic;

namespace CarpetPlanner6.Models
{
    /// <summary>
    /// Edit carpet view model.
    /// </summary>
    public class CarpetViewModel
    {
        /// <summary>
        /// Current username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Current carpet with stripes.
        /// </summary>
        public CarpetEntity Carpet { get; set; }

        /// <summary>
        /// List of colors that user can use.
        /// </summary>
        public IList<ColorEntity> Colors { get; set; }

        /// <summary>
        /// Other carpets for current user.
        /// </summary>
        public IList<CarpetEntity> OtherCarpets { get; set; }
    }
}
