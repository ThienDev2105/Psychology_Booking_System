using System.Diagnostics;
using System.Threading.Tasks;
using CloudinaryDotNet;
using EXE201.Commons.Data;
using EXE201.Commons.Models;
using EXE201.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serenity_Solution.Models;

namespace Serenity_Solution.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAccountService _accountService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;


        public HomeController(ILogger<HomeController> logger,UserManager<User> userManager, IAccountService accountService,
            IEmailService emailService,
            SignInManager<User> signInManager,
            ApplicationDbContext context)
        {           
            _logger = logger;

            _accountService = accountService;
            _userManager = userManager;
            _emailService = emailService;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                // Redirect đến trang chính của Manager
                return RedirectToAction("Index", "Manager");
            }

            // Lấy 10 podcast có đánh giá cao nhất từ PodcastController
            var podcasts = PodcastController.GetPodcasts()
                .OrderByDescending(p => p.Rating)
                .Take(10)
                .ToList();

            var allDoctors = await _userManager.GetUsersInRoleAsync("Psychologist");
            var doctors = allDoctors.OrderBy(d => Guid.NewGuid()).Take(2).ToList();

            // Truyền danh sách podcast vào view bằng ViewBag
            ViewBag.Podcasts = podcasts;

            var CurrentUser = await _userManager.GetUserAsync(User);

            if (User.Identity.IsAuthenticated)
            {
                var viewModel = new HomeVM
                {
                    Doctors = doctors,
                    Podcasts = podcasts,
                    Contact = new Contact(),
                    currentUser = CurrentUser.Id
                };
                return View(viewModel);
            }

            var viewModelGuest = new HomeVM
            {
                Doctors = doctors,
                Podcasts = podcasts,
                Contact = new Contact()
            };

            return View(viewModelGuest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitContact(HomeVM model, string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                returnUrl = Url.Action("Index", "Home");
            }

            if (!User.Identity.IsAuthenticated)
            {
                TempData["Error"] = "Vui lòng đăng nhập để gửi yêu cầu!";
                return RedirectToAction("Index");
            }
            var EmailUserLogged = await _userManager.GetUserAsync(User);
            if (model.Contact.Email != EmailUserLogged.Email)
            {
                TempData["Error"] = "Vui lòng nhập email bạn đã đăng ký!";
                return RedirectToAction("Index");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            _context.Contacts.Add(model.Contact);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Cảm ơn bạn đã liên hệ!";
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index");

        }




        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
