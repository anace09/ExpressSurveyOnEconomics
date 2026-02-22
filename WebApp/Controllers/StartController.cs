using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class StartController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<StartController> _logger;

        public StartController(AppDbContext context, ILogger<StartController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index(string clear = null)
        {

            if (clear == "true")
            {
                HttpContext.Session.Clear();
                TempData["Message"] = "Сессия очищена. Введите данные заново.";
                return RedirectToAction("Index");
            }

            var authError = HttpContext.Session.GetString("AuthError");
            if (!string.IsNullOrEmpty(authError))
            {
                ViewBag.AuthError = authError;
                HttpContext.Session.Remove("AuthError");
            }

            if (HttpContext.Session.GetString("ParticipantId") != null)
            {
                ViewBag.IsRegistered = true;
                ViewBag.ParticipantId = HttpContext.Session.GetString("ParticipantId");
                ViewBag.ParticipantName = HttpContext.Session.GetString("ParticipantName");
                return View(); 
            }

            return View(); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Person person)
        {
            if (!ModelState.IsValid)
            {
                return View(person);   // ← возвращаем модель с заполненными полями + ошибки
            }

            // Проверка дубликата по ФИО + организации
            var existing = await _context.Persons.AnyAsync(p =>
                p.FirstName == person.FirstName &&
                p.LastName == person.LastName &&
                p.MiddleName == person.MiddleName &&
                p.Organization == person.Organization);

            if (existing)
            {
                ModelState.AddModelError(string.Empty,
                    "Пользователь с такими ФИО и учебной организацией уже зарегистрирован. " +
                    "Если это вы — нажмите «Продолжить тест» ниже.");

                ViewBag.IsRegistered = true;

                return View(person);   // ← возвращаем с заполненными данными
            }

            try
            {
                _context.Persons.Add(person);
                await _context.SaveChangesAsync();

                HttpContext.Session.SetString("ParticipantId", person.Id.ToString());
                HttpContext.Session.SetString("ParticipantName", $"{person.FirstName} {person.LastName}");

                return RedirectToAction("Index", "Tests");
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("unique") == true)
            {
                ModelState.AddModelError(string.Empty, "Такая комбинация ФИО и организации уже зарегистрирована.");
                return View(person);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка сохранения участника");
                ModelState.AddModelError(string.Empty, "Произошла ошибка при сохранении. Попробуйте позже.");
                return View(person);
            }
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}