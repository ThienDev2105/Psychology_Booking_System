using CloudinaryDotNet;
using EXE201.Commons.Data;
using EXE201.Commons.Models;
using EXE201.Services.Interfaces;
using EXE201.Services.Models;
using EXE201.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serenity_Solution.Models;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Serenity_Solution.Controllers
{
    public class DoctorController : Controller
    {
        private readonly Cloudinary _cloudinary;
        private readonly IAccountService _accountService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IVnPayServicecs _vpnPayServicecs;
        public DoctorController(UserManager<User> userManager, IAccountService accountService, IEmailService emailService, SignInManager<User> signInManager,
            ApplicationDbContext context, Cloudinary cloudinary, IVnPayServicecs vnPayServicecs)
        {
            _cloudinary = cloudinary;
            _accountService = accountService;
            _userManager = userManager;
            _emailService = emailService;
            _signInManager = signInManager;
            _context = context;
            _vpnPayServicecs = vnPayServicecs;
        }
        public async Task<IActionResult> Index(string searchString, string filterType, int page = 1, int pageSize = 2)
        {
            var doctors = await LoadDoctors(searchString, filterType);

            int total = doctors.Count;
            var pagedDoctors = doctors.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.TotalPages = (int)Math.Ceiling((double)total / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.SearchString = searchString;
            ViewBag.FilterType = filterType;

            return View(pagedDoctors); // Trả về Index.cshtml đầy đủ
        }


        private async Task<List<PsychologistViewModel>> LoadDoctors(string searchString, string filterType)
        {
            var users = await _userManager.GetUsersInRoleAsync("Psychologist");
            var doctors = users.OfType<User>()
                .Where(c => c.CertificateUrl != null && c.ProfilePictureUrl != null && c.Price > 0)
                .ToList();

            if (!string.IsNullOrEmpty(searchString))
            {
                doctors = doctors.Where(d => d.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                                             d.Major.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(filterType))
            {
                switch (filterType)
                {
                    case "psychiatrist":
                        doctors = doctors.Where(d => d.Major?.Contains("Bác sĩ tâm thần", StringComparison.OrdinalIgnoreCase) == true).ToList();
                        break;
                    case "clinicalPsychologist":
                        doctors = doctors.Where(d => d.Major?.Contains("Nhà tâm lý học lâm sàng", StringComparison.OrdinalIgnoreCase) == true).ToList();
                        break;
                    case "counselor":
                        doctors = doctors.Where(d => d.Major?.Contains("Tư vấn viên tâm lý", StringComparison.OrdinalIgnoreCase) == true).ToList();
                        break;
                    case "experience":
                        doctors = doctors.OrderByDescending(d => d.Experience).ToList();
                        break;
                    case "priceAsc":
                        doctors = doctors.OrderBy(d => d.Price).ToList();
                        break;
                }
            }

            return doctors.Select(d => new PsychologistViewModel
            {
                Id = d.Id,
                Name = d.Name,
                Address = d.Address,
                Price = d.Price,
                Description = d.Description,
                Experience = d.Experience,
                Major = d.Major,
                ProfilePictureUrl = d.ProfilePictureUrl
            }).ToList();
        }
        public async Task<IActionResult> GetDoctorList(string searchString, string filterType, int page = 1, int pageSize = 2)
        {
            var doctors = await LoadDoctors(searchString, filterType);

            int total = doctors.Count;
            var pagedDoctors = doctors.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.TotalPages = (int)Math.Ceiling((double)total / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.SearchString = searchString;
            ViewBag.FilterType = filterType;

            return PartialView("_DoctorListPartial", pagedDoctors);
        }


        public async Task<IActionResult> Detail(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null) { return NotFound(); }

            var experienceFormat = string.IsNullOrEmpty(user.Experience)
                ? ""
                : Regex.Replace(user.Experience, @"(?<=• \d{4}-\d{4}:.*?)(?= • \d{4}-\d{4}:)", "\n");

            var psychologist = new PsychologistViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Phone = user.PhoneNumber,
                Address = user.Address,
                Degree = user.CertificateUrl,
                Description = user.Description,
                Experience = experienceFormat,
                Price = user.Price,
                ProfilePictureUrl = user.ProfilePictureUrl,// hoặc null để dùng ảnh mặc định
                Major = user.Major,
                Contact = new Contact()
            };
            if (User.Identity.IsAuthenticated) 
            {
                var currentUser = await _userManager.GetUserAsync(User);
                ViewBag.UserId = currentUser.Id;
            }
            return View(psychologist);
        }

        [Authorize]
        public IActionResult Payment(string Id)
        {
            if (string.IsNullOrEmpty(Id))
                return BadRequest("Doctor ID is required");

            var doctorSelected = _userManager.FindByIdAsync(Id).Result;
            if (doctorSelected == null) { return NotFound(); }

            var userBooking = _userManager.GetUserAsync(User).Result;
            if (userBooking == null)
                return Unauthorized("User not logged in");

            var orderId = new Random().Next(1000, 100000);
            var vnPayModel = new VnPaymentRequestModel
            {
                Amount = (double)doctorSelected.Price,
                CreateDate = DateTime.Now,
                Description = $"Thanh toán tư vấn bác sĩ {doctorSelected.Name}",
                FullName = userBooking.Name,
                OrderId = orderId,
                Psychologist = doctorSelected
            };
            return Redirect(_vpnPayServicecs.CreatePaymentUrl(HttpContext, vnPayModel));
        }


        [Authorize]
        public async Task<IActionResult> PaymentCallBackAsync()
        {
            try
            {
                var response = _vpnPayServicecs.PaymentExecute(Request.Query);
                var code = response.VnPayResponseCode;
                var orderInfo = response.OrderDescription;
                var doctorId = (orderInfo);
                if (string.IsNullOrEmpty(doctorId))
                {
                    TempData["error"] = "Không tìm thấy thông tin bác sĩ";
                    return RedirectToAction("Index", "Doctor");
                }
                var doctorSelected = _userManager.FindByIdAsync(doctorId).Result;
                if (response == null || response.VnPayResponseCode != "00")
                {
                    TempData["error"] = "Lỗi thanh toán";
                    return RedirectToAction("Detail", "Doctor", new { Id = doctorId });

                }
                var currentUser = await _userManager.GetUserAsync(User);


                Appointment invoice = new Appointment();
                invoice.Psychologist_ID = doctorSelected.Id;
                invoice.Client_ID = currentUser.Id;
                invoice.Price = doctorSelected.Price;
                invoice.Status = "Booked";
                invoice.Scheduled_time = DateTime.Now.AddDays(3);
                invoice.Created_at = DateTime.Now;
                invoice.Notes = "Bạn có 3 ngày để phản hồi";

                //await _context.AddAsync(invoice);
                await _context.Appointments.AddAsync(invoice);
                

                var customer = await _userManager.FindByIdAsync(invoice.Client_ID);

                if (doctorSelected != null)
                {
                    await _emailService.SendEmailAsync(doctorSelected.Email, "Đặt lịch hẹn thành công", $"Bạn đã được đặt lịch tư vấn với {customer.Name}. Vui lòng kiểm tra thông tin chi tiết trong hệ thống.");
                }
                doctorSelected.BaBalance  += (double)doctorSelected.Price * 0.9;
                await _userManager.UpdateAsync(doctorSelected);
                await _context.SaveChangesAsync();
                /*
     // 2. create conversation
                var existingConversation = await _context.Conversations
                .FirstOrDefaultAsync(c =>
                    (c.User1Id == currentUser.Id && c.User2Id == doctorSelected.Id) ||
                    (c.User1Id == doctorSelected.Id && c.User2Id == currentUser.Id));

                if (existingConversation == null)
                {
                    // 3. Nếu chưa có -> Tạo Conversation mới
                    var conversation = new Conversation
                    {
                        User1Id = currentUser.Id,
                        User2Id = doctorSelected.Id
                    };

                    _context.Conversations.Add(conversation);
                    await _context.SaveChangesAsync();

                    
                }
                //end
                */
                TempData["success"] = "Thanh toán thành công";
                return RedirectToAction("Detail", "Doctor", new { Id = doctorId });
            }
            catch (Exception ex)
            {
                TempData["error"] = "Có lỗi xảy ra trong quá trình xử lý";
                return RedirectToAction("Index", "Doctor");
            }

        }
        
    }
}
