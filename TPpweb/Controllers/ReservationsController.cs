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
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private static int EmployerId;

        public ReservationsController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: Reservations
        [HttpGet]
        public async Task<IActionResult> Index(int id, int employerId)
        {
            var reservationsForHousing = await _context.Reservations
                .Where(r => r.FkHousing == id)
                .Include(r => r.Housing) // Se necessário, carrega informações da habitação associada
                .Include(r =>r.Client)
                .ToListAsync();

            EmployerId = employerId;
            return View(reservationsForHousing);
        }
        public async Task<IActionResult> IndexAll(string? employerId, string? status)
        {
            if (EmployerId == 0)
            {
               
                    
                    var i = employerId; var employer = await _context.Employer
                        .Include(e => e.Landlord) // Include the Landlord entity
                        .ThenInclude(l => l.Properties) // Include the Properties collection of Landlord
                        .FirstOrDefaultAsync(e => e.PersonId == employerId);

                    EmployerId = employer.Id;

                

            }


            var reservations = await _context.Reservations
                .Include(r => r.Housing) // Se necessário, carrega informações da habitação associada
                .Include(r => r.Client)
                .ToListAsync();
            ViewBag.status = status;
            ViewBag.EmployerId = EmployerId;

            if (status == "accepted")
            {
                reservations = reservations.Where(r => r.accepted).ToList();
            }
            else if (status == "pending")
            {
                reservations = reservations.Where(r => !r.accepted).ToList();
            }
            else if (status == "timed")
            {
                DateTime dataAtual = DateTime.Now;
                // Comparando a data atual com a data fornecida
                reservations = reservations.Where(r => dataAtual > r.DataFim).ToList();
            }

            return View("IndexAll", reservations);
        }
      

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Reservations == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Housing)
                .FirstOrDefaultAsync(m => m.IdReservation == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        public IActionResult Create(int id)
        {
            /*if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Só utilizadores autenticados é que podem efetuar agendamentos. Por favor, autentique-se.";
                return RedirectToAction("Login", "Conta");
            }
            */

            ViewBag.Id = id;
            
            return View();
        }


        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DataInicio,DataFim")] Reservation reservation, int id)
        {
            ModelState.Remove(nameof(reservation.Client));
            ModelState.Remove(nameof(reservation.FkClient));

            ModelState.Remove(nameof(reservation.Housing));
            ModelState.Remove(nameof(reservation.FkHousing));


            if (ModelState.IsValid && id != 0)
            {

                var user = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
                if (user != null)
                {
                    reservation.FkClient = user.Id;
                }


                if (reservation.FkClient == null)
                {
                    Console.WriteLine(User.Identity.Name + "\n");
                    
                    Console.WriteLine("Estou nulo!\n\n\n\n\n");
                }

                Console.WriteLine(User.Identity.Name + "\n\n\n\n");
                Console.WriteLine(reservation.FkClient + "\n\n\n\n");
                reservation.FkHousing = id;

                var housing = _context.Houses.FirstOrDefault(h => h.Id == id);
                reservation.Housing = housing;




                TimeSpan duration = reservation.DataFim - reservation.DataInicio;
                reservation.Price = CalculatePrice(duration.Days, housing.PriceMonth);
                reservation.accepted = false;

                _context.Reservations.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FkHousing"] = new SelectList(_context.Houses, "Id", "Location", reservation.FkHousing);
            return View(reservation);
        }

        private float CalculatePrice(int days, int price)
        {
            decimal monthsCompletos = Math.Ceiling((decimal)days / price);

            // Calcular o valor total
            return (int) ( price * monthsCompletos );
           
        }


        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Reservations == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FindAsync(id);
            ViewBag.housingId = reservation.FkHousing;
            if (reservation == null)
            {
                return NotFound();
            }
            return View(reservation); 
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdReservation,FkHousing,FkClient,DataInicio,DataFim,accepted")] Reservation reservation)
        {
            if (id != reservation.IdReservation)
            {
                return NotFound();
            }
            ModelState.Remove(nameof(reservation.Client));
            ModelState.Remove(nameof(reservation.Housing));


            if (ModelState.IsValid)
            {
                var housing = _context.Houses.FirstOrDefault(h => h.Id == reservation.FkHousing);
                reservation.Housing = housing;

                var client = _context.Persons.FirstOrDefault(h => h.Id == reservation.FkClient);
                reservation.Client = client;

                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.IdReservation))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                
                return RedirectToAction("Create", "DeliveryDetails", new { reservId = reservation.IdReservation, employerId = EmployerId } );
            }
            ViewData["FkHousing"] = new SelectList(_context.Houses, "Id", "Location", reservation.FkHousing);
            return View(reservation); 
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Reservations == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Housing)
                .FirstOrDefaultAsync(m => m.IdReservation == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Reservations == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Reservations'  is null.");
            }
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
          return (_context.Reservations?.Any(e => e.IdReservation == id)).GetValueOrDefault();
        }
    }
}
