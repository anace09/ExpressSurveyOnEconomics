using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using WebApp.Controllers.Config;
using WebApp.Data;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class AdminController : Controller
    {

        private readonly AppDbContext _context;
        private readonly AdminSettings _settings;

        public AdminController(AppDbContext context, IOptions<AdminSettings> settings) {
            this._context = context;
            this._settings = settings.Value;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string login, string password) {

            Console.WriteLine($"login='{login}' password='{password}'");
            Console.WriteLine($"settings.Login='{_settings.Login}'");
            Console.WriteLine($"settings.Hash='{_settings.PasswordHash}'");
            bool verify = BCrypt.Net.BCrypt.Verify(password, _settings.PasswordHash);
            Console.WriteLine($"Verify={verify}");

            if (login == _settings.Login &&
                BCrypt.Net.BCrypt.Verify(password, _settings.PasswordHash))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, login),
                    new Claim(ClaimTypes.Role, "Admin")
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principical = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principical,
                    new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8) }
                    );

                return RedirectToAction(nameof(Results));
            }

            ViewBag.Error = "Неверный логин или пароль";
            return View();

        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Logout()
        {

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));

        }

        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Results(string? search, int? taskId, int page = 1) {

            var tasks = await _context.TestTasks.OrderBy(t => t.Id).ToListAsync();

            var grouped = await _context.UserAnswers
                .GroupBy(a => new { a.ParticipantId, a.TaskId })
                .Select(g => new
                {
                    g.Key.ParticipantId,
                    g.Key.TaskId,
                    EarnedPoints = g.Where(a => a.IsCorrect).Sum(a => a.Points),
                    AnsweredAt = g.Max(a => a.AnsweredAt)
                })
                .ToListAsync();

            var participantIds = grouped.Select(g => g.ParticipantId).Distinct().ToList();
            var persons = await _context.Persons
                .Where(p => participantIds.Contains(p.Id.ToString()))
                .ToDictionaryAsync(p => p.Id.ToString());

            var rows = participantIds.Select(pid =>
            {
                persons.TryGetValue(pid, out var person);
                var fullName = person != null
                    ? $"{person.LastName} {person.FirstName} {person.MiddleName}".Trim()
                    : pid;

                var scoreByTask = grouped
                    .Where(g => g.ParticipantId == pid)
                    .ToDictionary(g => g.TaskId, g => g.EarnedPoints);

                return new StudentResultRowViewModel
                {
                    ParticipantId = pid,
                    FullName = fullName,
                    Organization = person?.Organization ?? "—",
                    ScoreByTask = scoreByTask,
                    TotalPoints = scoreByTask.Values.Sum(),
                    LastAnsweredAt = grouped.Where(g => g.ParticipantId == pid).Max(g => g.AnsweredAt)
                };
            }).OrderByDescending(r => r.TotalPoints).ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                rows = rows.Where(r =>
                    r.FullName.ToLower().Contains(s) ||
                    r.Organization.ToLower().Contains(s)
                ).ToList();
            }

            ViewBag.Tasks = tasks;
            ViewBag.Search = search;

            return View(rows);
        }

        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Details(string participantId)
        {
            var person = await _context.Persons
                .FirstOrDefaultAsync(p => p.Id.ToString() == participantId);

            var tasks = await _context.TestTasks.OrderBy(t => t.Id).ToListAsync();

            var answers = await _context.UserAnswers
                .Where(a => a.ParticipantId == participantId)
                .OrderBy(a => a.TaskId).ThenBy(a => a.AnsweredAt)
                .ToListAsync();

            var byTask = tasks.Select(t => new TaskDetailGroupViewModel
            {
                Task = t,
                Answers = answers.Where(a => a.TaskId == t.Id).ToList(),
                EarnedPoints = answers.Where(a => a.TaskId == t.Id && a.IsCorrect).Sum(a => a.Points)
            }).Where(g => g.Answers.Any()).ToList();

            ViewBag.Person = person;
            ViewBag.Total = byTask.Sum(g => g.EarnedPoints);

            return View(byTask);
        }
    }

}
