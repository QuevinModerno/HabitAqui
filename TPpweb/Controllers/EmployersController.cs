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
    public class EmployersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Employers
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Employer.Include(e => e.Landlord).Include(e => e.Person);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> ShowHouses(string id)
        {

            var employer = await _context.Employer
            .Include(e => e.Landlord) // Include the Landlord entity
            .ThenInclude(l => l.Properties) // Include the Properties collection of Landlord
            .FirstOrDefaultAsync(e => e.PersonId == id);

            if (employer == null || employer.Landlord == null)
            {
                return NotFound(); // Handle the case where employer or landlord is not found
            }

            var propertiesViewModel = employer.Landlord.Properties.Select(p => new HousingViewModel
            {
                landLordName = employer.Landlord.Name, // Access the Landlord's Name property
                AverageStars = employer.Landlord.ratings != null && employer.Landlord.ratings.Any() ?
                    employer.Landlord.ratings.Average(r => r.Stars) : 0,
                housing = p
            }).ToList();

            ViewBag.EmployerId = employer.Id;

            return View(propertiesViewModel);
        }

        public async Task<IActionResult> CreateHouse(string id)
        {
            var employer = await _context.Employer
            .FirstOrDefaultAsync(e => e.PersonId == id);

            if (employer == null)
            {
                return NotFound(); // Handle the case where employer or landlord is not found
            }
            var LandlordId = employer.LandlordId;
            return RedirectToAction("Create", "Housings", new { LandlordId = LandlordId });

        }

        public async Task<IActionResult> EditHouse(string id)
        {
            var employer = await _context.Employer
            .FirstOrDefaultAsync(e => e.PersonId == id);

            return NotFound();
        }

        public async Task<IActionResult> DeleteHouse(string id)
        {
            var employer = await _context.Employer
            .FirstOrDefaultAsync(e => e.PersonId == id);

            return NotFound();
        }

            // GET: Employers/Details/5
            public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Employer == null)
            {
                return NotFound();
            }

            var employer = await _context.Employer
                .Include(e => e.Landlord)
                .Include(e => e.Person)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employer == null)
            {
                return NotFound();
            }

            return View(employer);
        }


        public async Task<IActionResult> CreateGestorFromAdmin(int landlordId, string username)
        {
            var locador = await _context.Landlords.FindAsync(landlordId);
            var user = _context.Users.FirstOrDefault(u => u.UserName == username);

            if (locador != null && user != null)
            {
                // Cria um novo Employer
                var novoEmployer = new Employer
                {
                    LandlordId = locador.IdLandlord, // Define o ID do locador
                    Landlord = locador, // Define o objeto locador

                    PersonId = user.Id, // Define o ID do usuário
                    Person = user // Define o objeto usuário
                                  // Pode atribuir outros campos do Employer conforme necessário
                };

                // Adiciona o novo Employer ao contexto e salva as mudanças no banco de dados
                _context.Employer.Add(novoEmployer);
                await _context.SaveChangesAsync();
            }


            return RedirectToAction("AreaAdministrador", "User");

        }



        public IActionResult CreateGestor() //Create Funcionario (role = null) && Gestor = (role = Gestor)
        {
            ViewData["LandlordId"] = new SelectList(_context.Landlords, "IdLandlord", "Name");
            ViewData["PersonId"] = new SelectList(_context.Persons, "Id", "UserName");
            ViewBag.role = "Gestor";
            return View();
        }


        // GET: Employers/Create
        public IActionResult Create(string? role, string? personId) //Create Funcionario (role = null) && Gestor = (role = Gestor)
        {
            ViewData["LandlordId"] = new SelectList(_context.Landlords, "IdLandlord", "Name");
            ViewData["PersonId"] = new SelectList(_context.Persons, "Id", "UserName");
            ViewBag.role = role;
            ViewBag.PersonId = personId;
            return View();
        }


        // POST: Employers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LandlordId,PersonId")] Employer employer, string? role)
        {
            ModelState.Remove(nameof(employer.Person));
            ModelState.Remove(nameof(employer.Landlord));
            
            if (ModelState.IsValid)
            {

                _context.Add(employer);
                await _context.SaveChangesAsync();

                var funcionarioRoleId = _context.Roles
                 .Where(r => r.Name == "Funcionario")
                 .Select(r => r.Id)
                 .FirstOrDefault();
              

                if (funcionarioRoleId != null && role == null)
                {
                    var userRole = new IdentityUserRole<string>
                    {
                        UserId = employer.PersonId, 
                        RoleId = funcionarioRoleId   
                    };

                    // Adiciona o usuário ao papel (role) "Funcionario"
                    _context.UserRoles.Add(userRole);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["LandlordId"] = new SelectList(_context.Landlords, "IdLandlord", "Name", employer.LandlordId);
            ViewData["PersonId"] = new SelectList(_context.Persons, "Id", "UserName", employer.PersonId);
            return View(employer);
        }

        // GET: Employers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Employer == null)
            {
                return NotFound();
            }

            var employer = await _context.Employer.FindAsync(id);
            if (employer == null)
            {
                return NotFound();
            }
            ViewData["LandlordId"] = new SelectList(_context.Landlords, "IdLandlord", "Name", employer.LandlordId);
            ViewData["PersonId"] = new SelectList(_context.Persons, "Id", "Id", employer.PersonId);
            return View(employer);
        }

        // POST: Employers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LandlordId,PersonId")] Employer employer)
        {
            if (id != employer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployerExists(employer.Id))
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
            ViewData["LandlordId"] = new SelectList(_context.Landlords, "IdLandlord", "Name", employer.LandlordId);
            ViewData["PersonId"] = new SelectList(_context.Persons, "Id", "Id", employer.PersonId);
            return View(employer);
        }

        // GET: Employers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Employer == null)
            {
                return NotFound();
            }

            var employer = await _context.Employer
                .Include(e => e.Landlord)
                .Include(e => e.Person)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employer == null)
            {
                return NotFound();
            }

            return View(employer);
        }

        // POST: Employers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Employer == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Employer'  is null.");
            }
            var employer = await _context.Employer.FindAsync(id);
            if (employer != null)
            {
                _context.Employer.Remove(employer);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployerExists(int id)
        {
          return (_context.Employer?.Any(e => e.Id == id)).GetValueOrDefault();
        }

    }
}
