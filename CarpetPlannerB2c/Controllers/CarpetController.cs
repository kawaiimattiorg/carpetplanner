namespace CarpetPlannerB2c.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web;
    using Models;
    using Spire.Pdf;
    using Spire.Pdf.Graphics;

    /// <summary>
    /// Controller for carpet editing related requests.
    /// </summary>
    [Authorize]
    public class CarpetController : Controller
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
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public CarpetController(CarpetDataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Initialize new carpet.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("/carpet")]
        public CarpetEntity NewCarpet()
        {
            var carpet = new CarpetEntity
            {
                Owner = User.GetObjectId() ?? throw new InvalidOperationException(),
                Name = "Uusi matto"
            };

            _context.Carpets.Add(carpet);
            _context.SaveChanges();

            return carpet;
        }

        /// <summary>
        /// View for editing carpet.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpGet]
        [Route("/carpet/{id}")]
        public IActionResult GetCarpet(int id)
        {
            var currentUser = User.GetObjectId() ?? throw new InvalidOperationException();

            var selectedCarpet = _context
                .Carpets
                .FirstOrDefault(carpet => carpet.Id == id && !carpet.Removed);

            if (selectedCarpet == default)
            {
                return NotFound();
            }

            var alias = _context
                .Aliases
                .FirstOrDefault(entity => entity.ObjectId == selectedCarpet.Owner);

            if (alias == default)
            {
                return StatusCode(500);
            }

            var carpets = _context
                .Carpets
                .Where(carpet => carpet.Owner == selectedCarpet.Owner && !carpet.Removed);

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
                Alias = alias.Alias,
                Carpet = selectedCarpet,
                EditAllowed = selectedCarpet.Owner == currentUser,
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
        /// Add stripe to carpet.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/carpet/{id}")]
        public IActionResult UpdateCarpet([FromRoute] int id)
        {
            var owner = User.GetObjectId() ?? throw new InvalidOperationException();

            var carpet = _context
                .Carpets
                .FirstOrDefault(entity => entity.Id == id && !entity.Removed);

            if (carpet == null)
            {
                return NotFound();
            }

            if (carpet.Owner != owner)
            {
                return Unauthorized();
            }

            var lastStripe = _context
                .Stripes
                .Where(entity => entity.CarpetId == id)
                .OrderByDescending(entity => entity.Ordinal)
                .FirstOrDefault();

            var stripe = new StripeEntity
            {
                CarpetId = id,
                ColorString = StripeEntity.DefaultColor,
                Height = 10.0,
                Ordinal = lastStripe?.Ordinal + 1 ?? 0
            };

            _context
                .Stripes
                .Add(stripe);

            _context.SaveChanges();

            return Ok(stripe);
        }

        /// <summary>
        /// Update carpet.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="patch"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("/carpet/{id}")]
        public IActionResult UpdateCarpet([FromRoute] int id, [FromBody] CarpetPatch patch)
        {
            var owner = User.GetObjectId() ?? throw new InvalidOperationException();

            var carpet = _context
                .Carpets
                .FirstOrDefault(entity => entity.Id == id && !entity.Removed);

            if (carpet == null)
            {
                return NotFound();
            }

            if (carpet.Owner != owner)
            {
                return Unauthorized();
            }

            var changes = false;

            if (!string.IsNullOrWhiteSpace(patch.Name))
            {
                carpet.Name = patch.Name;
                changes = true;
            }

            if (patch.Width != null)
            {
                carpet.Width = patch.Width.Value;
                changes = true;
            }

            if (changes)
            {
                _context.SaveChanges();
            }

            return Ok();
        }

        /// <summary>
        /// Update stripes.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="patch"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("/stripe/{id}")]
        public IActionResult UpdateCarpet([FromRoute] int id, [FromBody] StripePatch patch)
        {
            var owner = User.GetObjectId() ?? throw new InvalidOperationException();

            var carpet = _context
                .Carpets
                .FirstOrDefault(entity => entity.Id == id && !entity.Removed);

            if (carpet == null)
            {
                return NotFound();
            }

            if (carpet.Owner != owner)
            {
                return Unauthorized();
            }

            var stripes = _context
                .Stripes
                .Where(stripe => stripe.CarpetId == id && patch.Stripes.Contains(stripe.Id))
                .ToList();

            var colorEntity = patch.Color == null
                ? null
                : _context
                    .Colors
                    .FirstOrDefault(color => color.Id == patch.Color.Value && !color.Removed);

            if (colorEntity == null)
            {
                patch.Color = null;
            }
            else
            {
                patch.Rgb = colorEntity.Rgb;
            }

            switch (patch.MoveDirection)
            {
                case MoveDirection.Up:
                    stripes = stripes
                        .OrderBy(stripe => stripe.Ordinal)
                        .ToList();
                    break;
                case MoveDirection.NoMove:
                    break;
                case MoveDirection.Down:
                    stripes = stripes
                        .OrderByDescending(stripe => stripe.Ordinal)
                        .ToList();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            patch.Stripes = stripes
                .Select(stripe => stripe.Id)
                .ToList();

            foreach (var stripe in stripes)
            {
                if (patch.Color != null)
                {
                    stripe.Color = patch.Color.Value;
                }

                if (patch.Height != null)
                {
                    stripe.Height = patch.Height.Value;
                }

                if (patch.MoveDirection != MoveDirection.NoMove)
                {
                    var other = patch.MoveDirection == MoveDirection.Up
                        ? _context
                            .Stripes
                            .Where(entity => entity.CarpetId == stripe.CarpetId && entity.Ordinal < stripe.Ordinal)
                            .OrderByDescending(entity => entity.Ordinal)
                            .FirstOrDefault()
                        : _context
                            .Stripes
                            .Where(entity => entity.CarpetId == stripe.CarpetId && entity.Ordinal > stripe.Ordinal)
                            .OrderBy(entity => entity.Ordinal)
                            .FirstOrDefault();

                    if (other != null && !patch.Stripes.Contains(other.Id)) // TODO: TÄMÄ EHTO EI TOIMI JOS SIIRRETÄÄN KAHTA VIEREKKÄISTÄ RAITAA
                    {
                        var ordinal = other.Ordinal;
                        other.Ordinal = stripe.Ordinal;
                        stripe.Ordinal = ordinal;

                        if (patch.Moved == null)
                        {
                            patch.Moved = new List<int>();
                        }

                        patch.Moved.Add(stripe.Id);
                    }
                }

                if (patch.Remove)
                {
                    _context.Remove(stripe);
                }

                _context.SaveChanges();
            }

            return Ok(patch);
        }

        /// <summary>
        /// Open pdf version of the carpet.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [Route("/pdf/{id}")]
        public IActionResult GetCarpetPdf(int id)
        {
            var carpet = _context
                .Carpets
                .FirstOrDefault(entity => entity.Id == id && !entity.Removed);

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
