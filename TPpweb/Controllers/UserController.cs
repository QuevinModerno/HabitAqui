using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TPpweb.Data;

using System.Security.Claims;

using TPpweb.Data;

using TPpweb.Models;
using TPpweb.ViewModels;
using NuGet.Protocol.Plugins;

[Authorize(Roles = "Cliente, Funcionario, Gestor, Administrador")]
public class UserController : Controller
{
    private readonly ApplicationDbContext _context;

    private readonly UserManager<Person> _userManager; 

    public UserController(UserManager<Person> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    [Authorize(Roles = "Cliente")]
    public IActionResult AreaCliente()
    {
        return View();
    }

    [Authorize(Roles = "Funcionario")]
    public IActionResult AreaFuncionario()
    {
        var id = _userManager.GetUserId(User);
        // var employerDetails = _context.Employers.Include(e => e.Landlord).Include(e => e.Person).FirstOrDefault(e => e.Id == id);

        ViewBag.EmployerId = id;
        return View();
    }

    [Authorize(Roles = "Gestor")]
    public IActionResult AreaGestor()
    {
        var id = _userManager.GetUserId(User);
        // var employerDetails = _context.Employers.Include(e => e.Landlord).Include(e => e.Person).FirstOrDefault(e => e.Id == id);

        ViewBag.EmployerId = id;
        return View();
    }

    [Authorize(Roles = "Administrador")]
    public IActionResult AreaAdministrador()
    {
        return View();
    }
    [Authorize(Roles = "Administrador, Gestor")]

    [HttpGet]
    public IActionResult CreateGestor()
    {

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateGestor(Person user)
    {
        user.UserName = user.Email;
        if (ModelState.IsValid)
        {
            var result = await _userManager.CreateAsync(user, user.Passe);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Gestor");

                return RedirectToAction("Create", "Employers", new { role = "Gestor", personId = user.Id });
            }
        }
        return RedirectToAction("AreaGestor", "User");
    }


    public async Task<IActionResult> CreateGestorFromAdmin(int landlordId)
    {
        var maxGestorNumber = _context.Users
                     .Where(u => u.UserName.StartsWith("gestor"))
                     .Select(u => u.UserName.Substring(6)) // Obtém a parte do número do username
                     .AsEnumerable() // Traga os dados para a memória para continuar a operação
                     .Select(numStr => int.TryParse(numStr, out int number) ? number : 0)
                     .DefaultIfEmpty(0)
                     .Max();

        // Incrementar o número para criar o próximo username do gestor
        var nextGestorNumber = maxGestorNumber + 1;
        var novoUsernameGestor = $"gestor{nextGestorNumber}@isec.pt";


        var defaultUser = new Person
        {
            UserName = novoUsernameGestor,
            Email = novoUsernameGestor,
            Nome = "Gestor",
            Morada = "ISEC",
            Passe = "Pweb_123",
            DataNascimento = new DateTime(2010, 12, 12),
        };
        var user = await _userManager.FindByEmailAsync(defaultUser.Email);
        if (user == null)
        {
            var sw = await _userManager.CreateAsync(defaultUser, defaultUser.Passe);

            await _userManager.AddToRoleAsync(defaultUser,
             Roles.Gestor.ToString());
            return RedirectToAction("CreateGestorFromAdmin", "Employers", new { landlordId, username = novoUsernameGestor });
        }

        return View("Index");
    
    }

    public IActionResult ListUsers()
    {
        var users = _userManager.Users.ToList();

        var viewModelList = users.Select(user => new UserViewModel
        {
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Roles = _userManager.GetRolesAsync(user).Result.ToList(),
            IsAdmin = _userManager.IsInRoleAsync(user, "Administrador").Result,
            HasAssociatedHouse = _context.Houses.Any(h => h.FkLandLord.Equals(user.Id))
        }).ToList();


        return View(viewModelList);
    }

    public IActionResult EditUser(string userId)
    {
        var user = _userManager.FindByIdAsync(userId).Result;

        if (user == null)
        {
            // Trate o caso em que o usuário não é encontrado, por exemplo, redirecione para uma página de erro
            return RedirectToAction("UserNotFound");
        }

        return View("EditUser", user);
    }

    public IActionResult HistoricoArrendamentos()
    {
        try
        {
            var userId = _userManager.GetUserId(User);

            var reservations = _context.Reservations
                .Include(r => r.Housing)
                .Where(r => r.FkClient == userId)
                .ToList();

            if (reservations != null && reservations.Any())
            {
                var viewModel = reservations.Select(reservation => new ReservationViewModel
                {
                    Id = reservation.IdReservation,
                    Client = reservation.FkClient,
                    BeginDate = reservation.DataInicio,
                    EndDate = reservation.DataFim,
                    FkHousing = reservation.FkHousing,
                    AvaliacaoDada = reservation.AvaliacaoDada
                }).ToList();

                // Adicione esta linha para debug
                Console.WriteLine($"ViewModel Count: {viewModel.Count}");

                return View(viewModel);
            }
            else
            {
                return View(); // Se não houver reservas, retorna a view sem modelo
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return View(); // Trate a exceção conforme necessário
        }
    }

    public ActionResult MinhasAvaliacoes()
    {
        // Recupere o ID do usuário atualmente logado
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Consulte o banco de dados para recuperar as avaliações feitas pelo usuário
        var avaliacoes = _context.Rates
            .Include(r => r.Reservation)
            .Where(r => r.Reservation.Client.Id == userId)
            .ToList();

        // Mapeie as avaliações para uma ViewModel se necessário

        return View(avaliacoes);
    }


    public ActionResult AvaliarHabitacao(int arrendamentoId)
    {

        try
        {
            // Lógica para recuperar informações necessárias e criar a ReservationViewModel
            var reservation = _context.Reservations
                .Include(r => r.Housing) // Certifique-se de incluir as entidades relacionadas necessárias
                .SingleOrDefault(r => r.IdReservation == arrendamentoId);

            if (reservation == null)
            {
                // Se a reserva não for encontrada, você pode redirecionar para uma página de erro ou lidar de outra forma
                return RedirectToAction("ReservationNotFound");
            }

            // Verifica se a avaliação já foi dada para esta reserva
            if (reservation.AvaliacaoDada)
            {
                return RedirectToAction("ReservationAlreadyRated");
            }

            var viewModel = new ReservationViewModel
            {
                Id = reservation.IdReservation,
                Client = reservation.FkClient,
                BeginDate = reservation.DataInicio,
                EndDate = reservation.DataFim,
                FkHousing = reservation.FkHousing,
                AvaliacaoDada = reservation.AvaliacaoDada,
                RatingStars = 0.0, // Defina um valor padrão, se necessário
                RatingDate = DateTime.Now, // Defina a data atual ou outro valor padrão, se necessário
                RatingComment = string.Empty // Defina uma string vazia ou outro valor padrão, se necessário
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            // Trate qualquer exceção que possa ocorrer durante a interação com o banco de dados
            ModelState.AddModelError(string.Empty, "Ocorreu um erro ao recuperar as informações. Por favor, tente novamente mais tarde.");
            // Se necessário, redirecione para uma página de erro
            return RedirectToAction("Error");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult AvaliarHabitacao(ReservationViewModel viewModel)
    {
            if (ModelState.IsValid)
            {
                try
                {

                    var rating = new Rating
                    {
                        FkReservation = viewModel.Id,
                        Stars = viewModel.RatingStars,
                        DateRating = viewModel.RatingDate,
                        Comment = viewModel.RatingComment
                    };

                    _context.Rates.Add(rating);
                    _context.SaveChanges();

                    Console.WriteLine("Avaliação salva na tabela Rates");

                    // Atualiza a flag de avaliação na reserva para evitar avaliação duplicada
                    var reservation = _context.Reservations.Find(viewModel.Id);
                    reservation.AvaliacaoDada = true;
                    _context.Update(reservation);
                    _context.SaveChanges();


                    return RedirectToAction("AreaCliente");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Erro ao salvar a avaliação: " + ex.Message);
                }
            }
        // Se a validação falhar, retorna à View com mensagens de erro
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            // Trate o caso em que o ID do usuário não é fornecido, por exemplo, redirecione para uma página de erro
            return RedirectToAction("UserNotFound");
        }

        var userToDelete = await _userManager.FindByIdAsync(userId);

        if (userToDelete == null)
        {
            // Trate o caso em que o usuário não é encontrado, por exemplo, redirecione para uma página de erro
            return RedirectToAction("UserNotFound");
        }

        var result = await _userManager.DeleteAsync(userToDelete);

        if (result.Succeeded)
        {
            // O usuário foi excluído com sucesso, redirecione para a lista de usuários
            return RedirectToAction("ListUsers");
        }
        else
        {
            // Ocorreu um erro ao excluir o usuário, adicione mensagens de erro ao ModelState
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            // Retorne para a view de detalhes do usuário para exibir mensagens de erro
            return RedirectToAction("EditUser", new { userId = userId });
        }
    }




    [HttpPost]
    public async Task<IActionResult> UpdateUser(Person updatedUser)
    {
        if (ModelState.IsValid)
        {
            var existingUser = await _userManager.FindByIdAsync(updatedUser.Id);

            if (existingUser == null)
            {
                return RedirectToAction("UserNotFound");
            }

            existingUser.UserName = !string.IsNullOrWhiteSpace(updatedUser.UserName) ? updatedUser.UserName : updatedUser.Email;
            existingUser.Email = updatedUser.Email;
            existingUser.Nome = updatedUser.Nome;
            existingUser.Morada = updatedUser.Morada;
            existingUser.DataNascimento = updatedUser.DataNascimento;
            existingUser.Contacto = updatedUser.Contacto;
            existingUser.BI = updatedUser.BI;
            existingUser.NIF = updatedUser.NIF;
            existingUser.Passe = updatedUser.Passe;

            var result = await _userManager.UpdateAsync(existingUser);

            if (result.Succeeded)
            {
                return RedirectToAction("ListUsers");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View("EditUser", updatedUser);
    }

}
