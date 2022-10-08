using System.Collections.Generic;

namespace CarpetPlanner.Models
{
    /// <summary>
    /// Edit carpet view model.
    /// </summary>
    public class CarpetViewModel
    {
        /// <summary>
        /// Current username.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Current carpet with stripes.
        /// </summary>
        public CarpetEntity Carpet { get; set; }

        /// <summary>
        /// Whether current user is allowed to edit the carpet. Buttons that would
        /// make changes to the carpet are only shown when this is true.
        /// </summary>
        public bool EditAllowed { get; set; }

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
