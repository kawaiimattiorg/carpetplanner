namespace CarpetPlannerB2c.Controllers
{
    using System.Linq;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web;
    using Models;

    /// <summary>
    /// Controller for different
    /// </summary>
    [Authorize]
    public class UserController : Controller
    {
        /// <summary>
        /// Database handle.
        /// </summary>
        private readonly CarpetDataContext _context;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context"></param>
        public UserController(CarpetDataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// View for selecting user.
        /// </summary>
        /// <returns></returns>
        [Route("/user")]
        public IActionResult GetUsers()
        {
            var data = new UserSelectionViewModel
            {
                Aliases = _context
                    .Aliases
                    .OrderBy(alias => alias.Alias)
                    .Select(alias => alias.Alias)
                    .ToList()
            };

            return View("UserSelection", data);
        }

        /// <summary>
        /// View for selecting carpet to edit.
        /// </summary>
        /// <returns></returns>
        [Route("")]
        public IActionResult GetUserCarpets()
        {
            var objectId = User.GetObjectId() ?? "";

            var data = new CarpetSelectionViewModel
            {
                Alias = objectId,
                Carpets = _context
                    .Carpets
                    .Where(carpet => carpet.Owner == objectId && !carpet.Removed)
                    .OrderBy(carpet => carpet.Name)
                    .ToList()
            };

            return View("CarpetSelection", data);
        }

        /// <summary>
        /// View for selecting carpet to edit.
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        [Route("/user/{alias}")]
        public IActionResult GetUserCarpets(string alias)
        {
            var owner = _context
                .Aliases
                .FirstOrDefault(entity => entity.Alias == alias);

            if (owner == default)
            {
                return BadRequest();
            }

            var data = new CarpetSelectionViewModel
            {
                Alias = alias,
                Carpets = _context
                    .Carpets
                    .Where(carpet => carpet.Owner == owner.ObjectId && !carpet.Removed)
                    .OrderBy(carpet => carpet.Name)
                    .ToList()
            };

            return View("CarpetSelection", data);
        }
    }
}
