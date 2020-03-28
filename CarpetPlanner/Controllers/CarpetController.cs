namespace CarpetPlanner.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Models;

    /// <summary>
    /// Controller for carpet editing related requests.
    /// </summary>
    public class CarpetController : Controller
    {
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
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/carpet/{username}")]
        public CarpetEntity NewCarpet([FromRoute] string username)
        {
            var carpet = new CarpetEntity
            {
                Username = username,
                Name = "Uusi matto"
            };

            _context.Carpets.Add(carpet);
            _context.SaveChanges();

            return carpet;
        }

        /// <summary>
        /// Add stripe to carpet.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/carpet/{username}/{id}")]
        public IActionResult UpdateCarpet([FromRoute] string username, [FromRoute] int id)
        {
            var carpet = _context
                .Carpets
                .FirstOrDefault(entity => entity.Username == username && entity.Id == id && !entity.Removed);

            if (carpet == null)
            {
                return NotFound();
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
        /// <param name="username"></param>
        /// <param name="id"></param>
        /// <param name="patch"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("/carpet/{username}/{id}")]
        public IActionResult UpdateCarpet([FromRoute] string username, [FromRoute] int id, [FromBody] CarpetPatch patch)
        {
            var carpet = _context
                .Carpets
                .FirstOrDefault(entity => entity.Username == username && entity.Id == id && !entity.Removed);

            if (carpet == null)
            {
                return NotFound();
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
        /// <param name="username"></param>
        /// <param name="id"></param>
        /// <param name="patch"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("/stripe/{username}/{id}")]
        public IActionResult UpdateCarpet([FromRoute] string username, [FromRoute] int id, [FromBody] StripePatch patch)
        {
            var carpet = _context
                .Carpets
                .FirstOrDefault(entity => entity.Username == username && entity.Id == id && !entity.Removed);

            if (carpet == null)
            {
                return NotFound();
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
    }
}
