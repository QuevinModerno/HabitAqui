using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TPpweb.Data;
using TPpweb.Models;
using TPpweb.ViewModels;

namespace TPpweb.Controllers
{
    public class HousingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HousingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Housings

        /* public async Task<IActionResult> Index()
         {
               return _context.Houses != null ? 
                           View(await _context.Houses.ToListAsync()) :
                           Problem("Entity set 'ApplicationDbContext.Houses'  is null.");
         }
        */
        public async Task<IActionResult> Index()
        {
            var housesViewModel = await _context.Houses
                .Include(h => h.Landlord) // Include the Landlord entity
                .Select(h => new HousingViewModel
                {
                    landLordName = h.Landlord.Name, // Access the Landlord's Name property
                    AverageStars = h.Landlord.ratings != null && h.Landlord.ratings.Any() ? h.Landlord.ratings.Average(r => r.Stars) : 0,
                    housing = h
                })
                .ToListAsync();

            return View(housesViewModel);
        }

        public async Task<IActionResult> LandlordIndex()
        {
            var properties = TempData["PropertiesData"] as List<HousingViewModel>;
            return View("Index", properties);
        }

        public async Task<IActionResult> ShowSearchForm()
        {
            return View();
        }

        public async Task<IActionResult> ShowSearchResults(string searchWord, string location, string type, DateTime? startDate, DateTime? endDate, string landlordName,String orderBy)
        {
            IQueryable<Housing> query = _context.Houses;
            Console.WriteLine($"searchWord: {searchWord}, location: {location}, type: {type}, startDate: {startDate}, endDate: {endDate}");

            // Adicione condições à consulta com base nos parâmetros fornecidos
            if (!string.IsNullOrEmpty(searchWord))
            {
                query = query.Where(h => EF.Functions.Like(h.Name, $"%{searchWord}%") ||
                                         EF.Functions.Like(h.Description, $"%{searchWord}%"));
            }

            if (!string.IsNullOrEmpty(location))
            {
                query = query.Where(h => EF.Functions.Like(h.Location, $"%{location}%"));
            }

            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(h => h.TipeHabit == type);
            }

            if (startDate.HasValue)
            {
                query = query.Where(h => h.AvailableFrom >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(h => h.AvailableUntil <= endDate.Value);
            }

            if (!string.IsNullOrEmpty(landlordName))
            {
                query = query.Where(h => EF.Functions.Like(h.Landlord.Name, $"%{landlordName}%"));
            }
            if (orderBy != null)
            {
                switch (orderBy)
                {
                    case "PriceAsc":
                        query = query.OrderBy(h => h.TotalPrice);
                        break;
                    case "PriceDesc":
                        query = query.OrderByDescending(h => h.TotalPrice);
                        break;
                    case "RatingAsc":
                        var sortedResultsAsc = await query.ToListAsync();
                        sortedResultsAsc = sortedResultsAsc.OrderBy(h => h.Ratings != null && h.Ratings.Any() ? h.Ratings.Average(r => r.Stars) : 0).ToList();
                        return View("ShowSearchResults", sortedResultsAsc);

                    case "RatingDesc":
                        var sortedResultsDesc = await query.ToListAsync();
                        sortedResultsDesc = sortedResultsDesc.OrderByDescending(h => h.Ratings != null && h.Ratings.Any() ? h.Ratings.Average(r => r.Stars) : 0).ToList();
                        return View("ShowSearchResults", sortedResultsDesc);
                }
            }

            // Execute a consulta e obtenha os resultados
            var searchResults = await query.ToListAsync();

            // Retorne a nova View com os resultados da pesquisa
            return View("ShowSearchResults", searchResults);
        }


        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Houses == null)
            {
                return NotFound();
            }

            var housingViewModel = await _context.Houses
                .Where(m => m.Id == id)
                .Include(h => h.Landlord) // Include the Landlord entity
                .Select(h => new HousingViewModel
                {
                    landLordName = h.Landlord.Name, // Access the Landlord's Name property
                    AverageStars = h.Landlord.ratings != null && h.Landlord.ratings.Any() ? h.Landlord.ratings.Average(r => r.Stars) : 0,
                    housing = h
                })
                .FirstOrDefaultAsync();

            if (housingViewModel == null)
            {
                return NotFound();
            }

            return View(housingViewModel);
        }

        // GET: Housings/Create
        public IActionResult Create(int LandlordId) //Create from Funcionario
        {
            ViewBag.LandLordId = LandlordId;
            return View();
        }

        public IActionResult AdminCreate() //Create from Admin
        {
            ViewBag.LandLordId = 0;
            ViewData["LandlordsId"] = new SelectList(_context.Landlords, "IdLandlord", "Name");
            return View("Create");
        }



        // POST: Housings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,AvailableFrom,AvailableUntil,Location,TotalPrice,Area,NBedroom,Nbathroom,TipeHabit,NPeople,PriceMonth,Description, ImagePath, FotoFile")] Housing housing, int landLordId)
        {

            ModelState.Remove(nameof(housing.FkLandLord));
            ModelState.Remove(nameof(housing.Landlord));
            ModelState.Remove(nameof(housing.Ratings));
            

            if (ModelState.IsValid)
            {
                if (housing.FotoFile != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await housing.FotoFile.CopyToAsync(ms);
                        byte[] imageBytes = ms.ToArray();
                        housing.ImagePath = Convert.ToBase64String(imageBytes);
                    }
                    
                }

                housing.Ratings = new List<Rating>();
                housing.FkLandLord = landLordId;
               
                var landlord = await _context.Landlords.FindAsync(housing.FkLandLord);
                housing.Landlord = landlord;

                _context.Add(housing);
                await _context.SaveChangesAsync();


                // Adicionar a nova propriedade à lista de propriedades do Landlord
                if (landlord != null)
                {
                    if (landlord.Properties == null)
                    {
                        landlord.Properties = new List<Housing>();
                    }

                    landlord.Properties.Add(housing);
                    await _context.SaveChangesAsync(); // Salvar as alterações
                }

                return RedirectToAction(nameof(Index));
            }


            // Imprimir mensagens de erro do ModelState para depuração
            foreach (var modelStateKey in ModelState.Keys)
            {
                var modelStateVal = ModelState[modelStateKey];
                foreach (var error in modelStateVal.Errors)
                {
                    Console.WriteLine($"ModelState Error: {modelStateKey} - {error.ErrorMessage}");
                }
            }

            // Se o ModelState não for válido, retorne a View com os erros
            Console.WriteLine("ModelState inválido");

            return View(housing);
        }

        // GET: Housings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Houses == null)
            {
                return NotFound();
            }

            var housing = await _context.Houses.FindAsync(id);
            if (housing == null)
            {
                return NotFound();
            }
            return View(housing);
        }

        // POST: Housings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,AvailableFrom,AvailableUntil,Location,TotalPrice,FkLandLord,Area,NBedroom,Nbathroom,TipeHabit,NPeople,PriceMonth,Description")] Housing housing)
        {
            if (id != housing.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(housing);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HousingExists(housing.Id))
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
            return View(housing);
        }

        // GET: Housings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Houses == null)
            {
                return NotFound();
            }

            var housing = await _context.Houses
                .FirstOrDefaultAsync(m => m.Id == id);

            
            if (housing == null)
            {
                return NotFound();
            }

            //Verificar se pretence a alguma reserva:
            var reservationsForHousing = await _context.Reservations
                .Where(r => r.FkHousing == id)
                .ToListAsync();

            ViewBag.count = reservationsForHousing.Count;
            return View(housing);

        }


        // POST: Housings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Houses == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Houses'  is null.");
            }
            var housing = await _context.Houses.FindAsync(id);
            if (housing != null)
            {
                _context.Houses.Remove(housing);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HousingExists(int id)
        {
            return (_context.Houses?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReservation([Bind(" Client, BeginDate, EndDate, FkClient, FkHousing")] ReservationViewModel ReservModel)
        {
            if (ModelState.IsValid)
            {
                
                var housing = await _context.Houses.FindAsync(ReservModel.FkHousing);
                var client = User.Identity;
                

                if (housing != null || client != null)
                {
                    // Create a new Reservation object
                    var reservation = new Reservation
                    {
                        Housing = housing,
                        Client = (Person)client, // Make sure to handle this logic to get the correct client
                                         // Set other properties of the reservation here based on your ViewModel
                        DataInicio = ReservModel.BeginDate,
                        DataFim = ReservModel.EndDate,
                        // Set other properties as needed
                        accepted = false // Set the default value as needed
                    };

                    // Add the new reservation to the database context
                    _context.Reservations.Add(reservation);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("A reserva foi adicionada");

                    // Redirect to the Housing details page after creating the reservation
                    return RedirectToAction("Index", "Reservation");
                }
                else
                {
                    // Handle the case where the Housing wasn't found
                    return NotFound(); // Return a 404 error page
                }
                
            }

            // Handle the case where the ModelState is not valid
            return BadRequest(ModelState); // Return a bad request status
        }

    }
}
