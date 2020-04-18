namespace CarpetPlanner.Controllers
{
    using System;
    using System.IO;
    using System.Linq;
    using iText.Kernel.Colors;
    using iText.Kernel.Geom;
    using iText.Kernel.Pdf;
    using iText.Kernel.Pdf.Canvas;
    using iText.Layout;
    using iText.Layout.Element;
    using Microsoft.AspNetCore.Mvc;
    using Models;

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

            using (var ms = new MemoryStream())
            {
                var props = new WriterProperties();

                var pdf = new PdfDocument(new PdfWriter(ms, props));
                var document = new Document(pdf, PageSize.A4, true);

                // add carpet title
                var title = new Paragraph(carpet.Name);
                title.SetFontSize(36);
                title.SetMultipliedLeading(1.0f);
                document.Add(title);

                // add carpet information
                var stripeTotalLength = stripes.Sum(stripe => stripe.Height);
                var info = new Paragraph($"Leveys {carpet.Width} cm, pituus {stripeTotalLength} cm.");
                info.SetMultipliedLeading(1.0f);
                document.Add(info);

                // calculate the area that should be used for the carpet stripes
                var currentArea = document
                    .GetRenderer()
                    .GetCurrentArea()
                    .GetBBox();

                var bottomMargin = document.GetBottomMargin();
                var leftMargin = document.GetLeftMargin();

                currentArea.ApplyMargins(
                    0,
                    document.GetRightMargin(),
                    bottomMargin,
                    leftMargin,
                    false);

                // calculate cm to pixel conversion factor
                var totalHeight = currentArea.GetHeight();
                var totalWidth = currentArea.GetWidth();

                var maxStripeWidth = PdfMaxStripeWidth * totalWidth;
                var pdfRatio = maxStripeWidth / totalHeight;

                var stripeRatio = carpet.Width / stripeTotalLength;

                var tooHigh = pdfRatio > stripeRatio;

                double cmToPx, width;

                if (tooHigh)
                {
                    cmToPx = totalHeight / stripeTotalLength;
                    width = carpet.Width * cmToPx;
                }
                else
                {
                    cmToPx = maxStripeWidth / carpet.Width;
                    width = maxStripeWidth;
                }

                var infoWidth = (float) (totalWidth - width + 2 * leftMargin);

                // print stripes
                var canvas = new PdfCanvas(pdf.GetFirstPage());

                var start = (double) totalHeight + bottomMargin;

                foreach (var stripe in stripes)
                {
                    var height = stripe.Height * cmToPx;
                    start -= height;

                    // draw stripe
                    canvas
                        .SetColor(ColorFromRgb(colors[stripe.Color]), true)
                        .Rectangle(leftMargin, (float) start, (float) width, (float) height)
                        .Fill();
                    
                    // add stripe info box
                    var stripeInfo = new Paragraph($"{stripe.Height} cm")
                        .SetFontColor(ColorConstants.BLACK)
                        .SetFixedPosition(leftMargin + (float) width, (float) start, infoWidth)
                        .SetHeight((float)height);
                    
                    // TODO: SETFONTSIZE ON VARMAANKIN PT, MUT HEIGHT ON PX??
                    
                    if (height < 12)
                    {
                        stripeInfo.SetFontSize((float) Math.Ceiling(height - 2f));
                    }
                    
                    document.Add(stripeInfo);
                }

                document.Close();

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
            const float max = 255f;

            var r = Convert.ToUInt32(rgb.Substring(0, 2), 16) / max;
            var g = Convert.ToUInt32(rgb.Substring(2, 2), 16) / max;
            var b = Convert.ToUInt32(rgb.Substring(4, 2), 16) / max;

            return new DeviceRgb(r, g, b);
        }
    }
}
