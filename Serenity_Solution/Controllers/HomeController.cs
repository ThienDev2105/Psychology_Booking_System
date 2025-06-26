using System.Diagnostics;
using System.Threading.Tasks;
using CloudinaryDotNet;
using EXE201.Commons.Data;
using EXE201.Commons.Models;
using EXE201.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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


        public HomeController(ILogger<HomeController> logger, UserManager<User> userManager, IAccountService accountService, 
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
            var allDoctors = await _userManager.GetUsersInRoleAsync("Psychologist");
            var doctors = allDoctors.OrderBy(d => Guid.NewGuid()).Take(5).ToList();

            // Lấy 4 podcast có đánh giá cao nhất từ PodcastController
            var podcasts = PodcastController.GetPodcasts()
                .OrderByDescending(p => p.Rating)
                .Take(10)
                .ToList(); // Giả sử bạn có DbSet<Podcast>

            // Truyền danh sách podcast vào view bằng ViewBag
            ViewBag.Podcasts = podcasts;

            var CurrentUser = await _userManager.GetUserAsync(User);

            /*
            var model = new CombinedViewModel
            {
                Contact = new ContactViewModel(), // Khởi tạo ContactViewModel
                Podcast = new PodcastViewModel
                {
                    // Nếu bạn muốn hiển thị một podcast cụ thể, hãy khởi tạo dữ liệu ở đây
                    // Ví dụ:
                    Title = "Life Update: Cuộc sống của mình sau pobcast",
                    ImageUrl = "/image/Podcast/ThePresent2.png",
                    AudioUrl = "/audio/podcast-audio.mp3",
                    Rating = 4.6,
                    RatingCount = 205,
                    Description = "Giới thiệu: Cuộc đời không có sẵn hướng dẫn, nhưng chúng ta có thể học hỏi từ kinh nghiệm của người khác. Podcast này tập hợp những lời khuyên và hướng dẫn quý giá cho cuộc sống."
                }
            };
            */
            if (User.Identity.IsAuthenticated)
            {
                var viewModelC = new CombinedViewModel
                {
                    Doctors = doctors,
                    Podcast = new PodcastViewModel
                    {
                        // Nếu bạn muốn hiển thị một podcast cụ thể, hãy khởi tạo dữ liệu ở đây
                        // Ví dụ:
                        Title = "Life Update: Cuộc sống của mình sau pobcast",
                        ImageUrl = "/image/Podcast/ThePresent2.png",
                        AudioUrl = "/audio/podcast-audio.mp3",
                        Rating = 4.6,
                        RatingCount = 205,
                        Description = "Giới thiệu: Cuộc đời không có sẵn hướng dẫn, nhưng chúng ta có thể học hỏi từ kinh nghiệm của người khác. Podcast này tập hợp những lời khuyên và hướng dẫn quý giá cho cuộc sống."
                    },
                    Contact = new Contact(),

                    currentUser = CurrentUser.Id
                };
                return View(viewModelC);
            }

                var viewModel = new CombinedViewModel
                {
                Doctors = doctors,
                    Podcast = new PodcastViewModel
                    {
                        // Nếu bạn muốn hiển thị một podcast cụ thể, hãy khởi tạo dữ liệu ở đây
                        // Ví dụ:
                        Title = "Life Update: Cuộc sống của mình sau pobcast",
                        ImageUrl = "/image/Podcast/ThePresent2.png",
                        AudioUrl = "/audio/podcast-audio.mp3",
                        Rating = 4.6,
                        RatingCount = 205,
                        Description = "Giới thiệu: Cuộc đời không có sẵn hướng dẫn, nhưng chúng ta có thể học hỏi từ kinh nghiệm của người khác. Podcast này tập hợp những lời khuyên và hướng dẫn quý giá cho cuộc sống."
                    },
                Contact = new Contact(),
              
                //currentUser = CurrentUser.Id
            };

            return View(viewModel);

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
                return Redirect(returnUrl);
            }
            var EmailUserLogged = await _userManager.GetUserAsync(User);
            if (model.Contact.Email != EmailUserLogged.Email)
            {
                TempData["Error"] = "Vui lòng nhập email bạn đã đăng ký!";
                return Redirect(returnUrl);
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
