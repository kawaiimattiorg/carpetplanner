namespace CarpetPlanner.Controllers
{
    using System;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Models;

    /// <summary>
    /// Controller for different 
    /// </summary>
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
        [Route("")]
        public IActionResult GetUsers()
        {
            var data = new UserSelectionViewModel
            {
                Usernames = _context
                    .Carpets
                    .Select(carpet => carpet.Username)
                    .Distinct()
                    .OrderBy(username => username)
                    .ToList()
            };

            return View("UserSelection", data);
        }

        /// <summary>
        /// View for selecting carpet to edit.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [Route("/user/{username}")]
        public IActionResult GetUserCarpets(string username)
        {
            var data = new CarpetSelectionViewModel
            {
                Username = username,
                Carpets = _context
                    .Carpets
                    .Where(carpet => carpet.Username == username && !carpet.Removed)
                    .OrderBy(carpet => carpet.Name)
                    .ToList()
            };

            return View("CarpetSelection", data);
        }

        /// <summary>
        /// View for editing carpet.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [Route("/user/{username}/{id}")]
        public IActionResult GetCarpet(string username, int id)
        {
            var carpets = _context
                .Carpets
                .Where(carpet => carpet.Username == username && !carpet.Removed)
                .ToList();
            
            var selectedCarpet = carpets.FirstOrDefault(carpet => carpet.Id == id);

            if (selectedCarpet == null)
            {
                throw new Exception("Carpet not found");
            }
            
            var stripes = _context
                .Stripes
                .Where(stripe => stripe.CarpetId == id)
                .OrderBy(stripe => stripe.Id)
                .ToList();

            var colors = _context
                .Colors
                .ToList();

            var colorDictionary = colors.ToDictionary(color => color.Id, color => $"#{color.Rgb}");

            colorDictionary.Add(0, StripeEntity.DefaultColor);

            foreach (var stripe in stripes)
            {
                stripe.ColorString = colorDictionary[stripe.Color];
            }

            selectedCarpet.Stripes = stripes;

            var data = new CarpetViewModel
            {
                Username = username,
                Carpet = selectedCarpet,
                Colors = colors
                    .Where(color => !color.Removed)
                    .OrderBy(color => color.Ordinal)
                    .ToList(),
                OtherCarpets = carpets
                    .Where(carpet => carpet.Id != id)
                    .OrderBy(carpet => carpet.Name)
                    .ToList()
            };

            return View("Carpet", data);
        }
    }
}