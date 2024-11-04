using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TPpweb.Data;
using TPpweb.Models;
using TPpweb.ViewModels;

namespace TPpweb.Controllers
{
    public class DeliveryDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DeliveryDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DeliveryDetails
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Details.Include(d => d.Employer).Include(d => d.Reservation);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: DeliveryDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Details == null)
            {
                return NotFound();
            }

            var deliveryDetails = await _context.Details
                .Include(d => d.Employer)
                .Include(d => d.Reservation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deliveryDetails == null)
            {
                return NotFound();
            }

            return View(deliveryDetails);
        }

        // GET: DeliveryDetails/Create
        public IActionResult Create(int reservId, int employerId)
        {
            ViewBag.reservId = reservId;
            ViewBag.EmployerId = employerId;
            return View();
        }

        public IActionResult Createreturn(int reservId, int employerId)
        {
            ViewBag.reservId = reservId;
            ViewBag.EmployerId = employerId;

            return View("Create");
        }

        // POST: DeliveryDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OptionalEquipment,Damages,DamageImages,Observations,FkEmployer,ReservationId")] DeliveryDetails deliveryDetails)
        {
            ModelState.Remove(nameof(deliveryDetails.Employer));
            ModelState.Remove(nameof(deliveryDetails.Reservation));

            if (ModelState.IsValid)
            {

                var reservation = await _context.Reservations.FindAsync(deliveryDetails.ReservationId);
                var employer = await _context.Employer.FindAsync(deliveryDetails.FkEmployer);
                if (reservation != null && employer != null)
                {
                    deliveryDetails.Reservation = reservation;
                    deliveryDetails.Employer = employer;

                    _context.Add(deliveryDetails);
                    await _context.SaveChangesAsync();

                    if(reservation.Delivery == null)
                    { 
                        reservation.Delivery = deliveryDetails;
                        reservation.accepted = true;
                    }
                    else
                    {
                        reservation.Return = deliveryDetails;
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Reservations", new { id = deliveryDetails.Reservation.FkHousing, employerId = deliveryDetails.FkEmployer });
                }
            }
            ViewData["ReservationId"] = new SelectList(_context.Reservations, "IdReservation", "IdReservation", deliveryDetails.ReservationId);
            return View(deliveryDetails);
        }

        // GET: DeliveryDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Details == null)
            {
                return NotFound();
            }

            var deliveryDetails = await _context.Details.FindAsync(id);
            if (deliveryDetails == null)
            {
                return NotFound();
            }
            ViewData["FkEmployer"] = new SelectList(_context.Employer, "Id", "PersonId", deliveryDetails.FkEmployer);
            ViewData["ReservationId"] = new SelectList(_context.Reservations, "IdReservation", "IdReservation", deliveryDetails.ReservationId);
            return View(deliveryDetails);
        }

        // POST: DeliveryDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OptionalEquipment,Damages,DamageImages,Observations,FkEmployer,ReservationId")] DeliveryDetails deliveryDetails)
        {
            if (id != deliveryDetails.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(deliveryDetails);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeliveryDetailsExists(deliveryDetails.Id))
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
            ViewData["FkEmployer"] = new SelectList(_context.Employer, "Id", "PersonId", deliveryDetails.FkEmployer);
            ViewData["ReservationId"] = new SelectList(_context.Reservations, "IdReservation", "IdReservation", deliveryDetails.ReservationId);
            return View(deliveryDetails);
        }

        // GET: DeliveryDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Details == null)
            {
                return NotFound();
            }

            var deliveryDetails = await _context.Details
                .Include(d => d.Employer)
                .Include(d => d.Reservation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deliveryDetails == null)
            {
                return NotFound();
            }

            return View(deliveryDetails);
        }

        // POST: DeliveryDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Details == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Details'  is null.");
            }
            var deliveryDetails = await _context.Details.FindAsync(id);
            if (deliveryDetails != null)
            {
                _context.Details.Remove(deliveryDetails);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DeliveryDetailsExists(int id)
        {
          return (_context.Details?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
