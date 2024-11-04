using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TPpweb.Data;
using TPpweb.Models;
using TPpweb.ViewModels;

namespace TPpweb.Controllers
{
    public class LandlordsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LandlordsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Landlords
        public async Task<IActionResult> Index()
        {
            return _context.Landlords != null ?
                        View(await _context.Landlords.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Landlords'  is null.");
        }

        // GET: Landlords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Landlords == null)
            {
                return NotFound();
            }

            var landlord = await _context.Landlords
                .FirstOrDefaultAsync(m => m.IdLandlord == id);
            if (landlord == null)
            {
                return NotFound();
            }

            return View(landlord);
        }

        // GET: Landlords/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Landlords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdLandlord,Name,CompanyName,ContactInformation,active")] Landlord landlord)
        {
            ModelState.Remove(nameof(landlord.Properties));
            ModelState.Remove(nameof(landlord.ratings));
            if (ModelState.IsValid)
            {
                landlord.Properties = new List<Housing>();
                landlord.ratings = new List<Rating>();

                _context.Add(landlord);
                await _context.SaveChangesAsync();
                return RedirectToAction("CreateGestorFromAdmin", "User", new {landlordId = landlord.IdLandlord});
            }
            return View(landlord);
        }


        // GET: Landlords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Landlords == null)
            {
                return NotFound();
            }

            var landlord = await _context.Landlords.FindAsync(id);
            if (landlord == null)
            {
                return NotFound();
            }
            return View(landlord);
        }

        // POST: Landlords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdLandlord,Name,CompanyName,ContactInformation,active")] Landlord landlord)
        {
            if (id != landlord.IdLandlord)
            {
                return NotFound();
            }
            ModelState.Remove(nameof(landlord.Properties));
            ModelState.Remove(nameof(landlord.ratings));

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(landlord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LandlordExists(landlord.IdLandlord))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(landlord);
        }

        // GET: Landlords/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Landlords == null)
            {
                return NotFound();
            }

            var landlord = await _context.Landlords
                .FirstOrDefaultAsync(m => m.IdLandlord == id);
            if (landlord == null)
            {
                return NotFound();
            }

            return View(landlord);
        }

        // POST: Landlords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Landlords == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Landlords'  is null.");
            }
            var landlord = await _context.Landlords.FindAsync(id);
            if (landlord != null)
            {
                _context.Landlords.Remove(landlord);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LandlordExists(int id)
        {
            return (_context.Landlords?.Any(e => e.IdLandlord == id)).GetValueOrDefault();
        }


        public async Task<IActionResult> AddPropertie(int? id)
        {
            return View(Index);
        }
    }
}
