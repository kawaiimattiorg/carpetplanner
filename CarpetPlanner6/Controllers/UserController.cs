namespace CarpetPlanner6.Controllers
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Spire.Pdf;
    using Spire.Pdf.Graphics;

    /// <summary>
    /// Controller for different
    /// </summary>
    public class UserController : Controller
    {
        /// <summary>
        /// Maximum percent of width that should be used for stripes.
        /// </summary>
        private const double PdfMaxStripeWidth = 0.8;

        /// <summary>
        /// Height reserved for header information.
        /// </summary>
        private const double PdfHeaderHeight = 100.0;

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
                .OrderBy(stripe => stripe.Ordinal)
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

        /// <summary>
        /// Open pdf version of the carpet.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [Route("/pdf/{username}/{id}")]
        public IActionResult GetCarpetPdf(string username, int id)
        {
            var carpet = _context
                .Carpets
                .FirstOrDefault(entity => entity.Id == id && entity.Username == username && !entity.Removed);

            if (carpet == null)
            {
                return NotFound();
            }

            var stripes = _context
                .Stripes
                .Where(stripe => stripe.CarpetId == id)
                .OrderBy(stripe => stripe.Ordinal)
                .ToList();

            var colors = _context
                .Colors
                .ToList()
                .ToDictionary(color => color.Id, color => color.Rgb);

            colors.Add(0, StripeEntity.DefaultColor.Substring(1));

            var pdf = new PdfDocument();
            var page = pdf.Pages.Add();

            using (var ms = new MemoryStream())
            {
                // add carpet title
                page.Canvas.DrawString(
                    carpet.Name,
                    new PdfFont(PdfFontFamily.Helvetica, 32f),
                    PdfBrushes.Black,
                    new PointF(0, 0));

                // add carpet information
                var stripeTotalLength = stripes.Sum(stripe => stripe.Height);

                page.Canvas.DrawString(
                    $"Leveys {carpet.Width} cm, pituus {stripeTotalLength} cm.",
                    new PdfFont(PdfFontFamily.Helvetica, 16f),
                    PdfBrushes.Black,
                    new PointF(0, 50));

                // calculate the area that should be used for the carpet stripes
                var currentArea = new SizeF(
                    page.Canvas.ClientSize.Width,
                    page.Canvas.ClientSize.Height - (float)PdfHeaderHeight);

                // calculate cm to pixel conversion factor
                var maxStripeWidth = PdfMaxStripeWidth * currentArea.Width;
                var pdfRatio = maxStripeWidth / currentArea.Height;
                var stripeRatio = carpet.Width / stripeTotalLength;
                var tooHigh = pdfRatio > stripeRatio;

                double cmToPx, width;

                if (tooHigh)
                {
                    cmToPx = currentArea.Height / stripeTotalLength;
                    width = carpet.Width * cmToPx;
                }
                else
                {
                    cmToPx = maxStripeWidth / carpet.Width;
                    width = maxStripeWidth;
                }

                // print stripes
                var start = PdfHeaderHeight;

                foreach (var stripe in stripes)
                {
                    var height = stripe.Height * cmToPx;

                    page.Canvas.DrawRectangle(
                        new PdfSolidBrush(ColorFromRgb(colors[stripe.Color])),
                        0f,
                        (float) start,
                        (float) width,
                        (float) height);

                    // find vertical center for info text
                    var infoStart = start + 0.5 * height - 7.0;

                    page.Canvas.DrawString(
                        $"{stripe.Height} cm",
                        new PdfFont(PdfFontFamily.Helvetica, 12f),
                        PdfBrushes.Black,
                        new PointF((float) (width + 5.0), (float) infoStart));

                    start += height;
                }

                // Save pdf to stream
                pdf.SaveToStream(ms);

                // Send bytes to client as file
                return File(ms.ToArray(), "application/pdf");
            }
        }

        /// <summary>
        /// Convert RGB string to Color object.
        /// </summary>
        /// <param name="rgb"></param>
        /// <returns></returns>
        private static Color ColorFromRgb(string rgb)
        {
            return Color.FromArgb(
                Convert.ToInt32(rgb.Substring(0, 2), 16),
                Convert.ToInt32(rgb.Substring(2, 2), 16),
                Convert.ToInt32(rgb.Substring(4, 2), 16));
        }
    }
}
