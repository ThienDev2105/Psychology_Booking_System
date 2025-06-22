using CloudinaryDotNet;
using EXE201.Commons.Data;
using EXE201.Commons.Models;
using EXE201.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serenity_Solution.Models;

namespace Serenity_Solution.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManagerController : Controller
    {
        private readonly Cloudinary _cloudinary;
        private readonly IAccountService _accountService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        public ManagerController(UserManager<User> userManager, IAccountService accountService, IEmailService emailService, SignInManager<User> signInManager,
            ApplicationDbContext context, Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
            _accountService = accountService;
            _userManager = userManager;
            _emailService = emailService;
            _signInManager = signInManager;
            _context = context;
        }
        public IActionResult Index()
        {
            bool noDetails = TempData["NoWSDetail"] as bool? ?? false;
            ViewBag.NoWSDetail = noDetails;
            return View();
        }
        public async Task<IActionResult> UpgradeRequest(int page = 1, int pageSize = 5)
        {
            var users = await _userManager.GetUsersInRoleAsync("Customer");

            var customers = users.OfType<User>() // Lọc ra danh sách Customer
                .Where(c => c.CertificateUrl != null) // Lọc ra những người có yêu cầu nâng cấp
                .ToList();

            if(customers.Count == 0)
            {
                TempData["NoWSDetail"] = true;
                return RedirectToAction("Index");
            }

            var CustomerList = customers
                 .Select(s => new CustomerViewModel
                 {
                     Id = s.Id,
                     FullName = s.Name,
                     Email = s.Email,
                     CertificateUrl = s.CertificateUrl
                 })
                 .ToList();
            int totalUsers = CustomerList.Count();
            var pagedUsers = CustomerList.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalUsers / pageSize);
            ViewBag.CurrentPage = page;
            // Gửi danh sách Staff kèm ID (dùng ViewBag nếu cần)
            return View(pagedUsers);

        }

        public async Task<IActionResult> AllDoctor(int page = 1, int pageSize = 5)
        {
            var users = await _userManager.GetUsersInRoleAsync("Psychologist");
            var userList = users.Select(s => new PsychologistViewModel
            {
                Id = s.Id,
                Name = s.Name,
                Email = s.Email,
                Price = s.Price,
                Phone = s.PhoneNumber,
                Address = s.Address,
                Degree = s.CertificateUrl,
                Description = s.Description,
                Experience = s.Experience,
                ProfilePictureUrl = s.ProfilePictureUrl,
                Major = s.Major,
            }).ToList();
            int totalUsers = userList.Count();
            var pagedUsers = userList.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalUsers / pageSize);
            ViewBag.CurrentPage = page;
            return View(pagedUsers);
        }

        public async Task<IActionResult> AllAppointments(int page = 1, int pageSize = 5)
        {
            var user = await _userManager.GetUserAsync(User);

            var ListAppointments = await _context.Appointments
                .Where(a => a.Psychologist_ID == user.Id && a.Status == "Booked" || a.Status == "Confirmed")
                .Include(a => a.Client)
                .Include(a => a.Psychologist)
                .ToListAsync();

            if (ListAppointments.Count == 0)
            {
                TempData["NoWSDetail"] = true;
                return RedirectToAction("Index");
            }

            int totalUsers = ListAppointments.Count();
            var pagedUsers = ListAppointments.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalUsers / pageSize);
            ViewBag.CurrentPage = page;

            return View(pagedUsers);
        }
        //Contact
        public async Task<IActionResult> AllRequest(int page = 1, int pageSize = 5)
        {
            var ListRequest = await _context.Contacts
                .Include(c => c.User)
                .ToListAsync();

            if (ListRequest.Count == 0)
            {
                TempData["NoWSDetail"] = true;
                return RedirectToAction("Index", "Manager");
            }

            var model = ListRequest
                .Select(s => new ContactViewModel
                {
                    UserId = s.UserId,
                    Name = s.Name,
                    Email = s.Email,
                    Content = s.Content,
                })
                .ToList();
            int totalUsers = model.Count();
            var pagedUsers = model.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalUsers / pageSize);
            ViewBag.CurrentPage = page;

            return View(pagedUsers);
        }
        /*
        [HttpGet]
        public async Task<IActionResult> Resolve(string UserId)
        {
            var ListRequest = await _context.Contacts
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == UserId);

            if (ListRequest == null) return NotFound();
                var contactViewModel = new ContactViewModel
            {
                UserId = UserId,
                Name = ListRequest.Name,
                Email = ListRequest.Email,
                Content = ListRequest.Content
            };

            return View(contactViewModel);
        }
        */
        [HttpPost]
        public async Task<IActionResult> Resolve(string UserId, string ResponseContent)
        {
            var contact = await _context.Contacts.Include(c => c.User).FirstOrDefaultAsync(c => c.UserId == UserId);
            if (contact == null)
                return NotFound();

            // Gửi email
            await _emailService.SendEmailAsync(contact.Email, "Phản hồi yêu cầu", ResponseContent);

            // Có thể xóa yêu cầu nếu đã xử lý
            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Phản hồi đã được gửi!";
            return RedirectToAction("AllRequest");
        }


        #region Resolve_Upgrade_Request

        [HttpPost]
        public async Task<IActionResult> ApproveUpgrade(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Email không được để trống.";
                return RedirectToAction("UpgradeRequest");
            }

            // Tìm user theo email
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy user.";
                return RedirectToAction("UpgradeRequest");
            }

            // Kiểm tra user có đang ở role Customer không
            var isInCustomerRole = await _userManager.IsInRoleAsync(user, "Customer");
            if (!isInCustomerRole)
            {
                TempData["ErrorMessage"] = "User không phải là Customer hoặc đã được nâng cấp.";
                return RedirectToAction("UpgradeRequest");
            }

            // Remove role Customer
            var removeResult = await _userManager.RemoveFromRoleAsync(user, "Customer");
            if (!removeResult.Succeeded)
            {
                TempData["ErrorMessage"] = "Xóa role Customer thất bại.";
                return RedirectToAction("UpgradeRequest");
            }

            // Add role Psychologist
            var addResult = await _userManager.AddToRoleAsync(user, "Psychologist");
            if (!addResult.Succeeded)
            {
                TempData["ErrorMessage"] = "Thêm role Psychologist thất bại.";
                return RedirectToAction("UpgradeRequest");
            }

            TempData["SuccessMessage"] = $"Nâng cấp user {email} thành công.";
            await _emailService.SendEmailAsync(user.Email, "Nâng cấp thành công", "Bạn đã trở thành nhà tâm lý học của hệ thống chúng tôi!");

            return RedirectToAction("UpgradeRequest");
        }

    

        [HttpPost]
        public async Task<IActionResult> RejectUpgrade(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                // Cập nhật trạng thái của người dùng
                var customer = user ;
                if (customer != null)
                {
                    customer.CertificateUrl = null;
                    await _userManager.UpdateAsync(user);
                }
                // Gửi email thông báo
                var subject = "Yêu cầu nâng cấp tài khoản đã bị từ chối";
                var message = "Rất tiếc, yêu cầu nâng cấp tài khoản của bạn đã bị từ chối.";
                await _emailService.SendEmailAsync(user.Email, subject, message);
                return RedirectToAction("UpgradeRequest");
            }
            return NotFound();
        }

#endregion 


    }
}
