using CloudinaryDotNet;
using EXE201.Commons.Data;
using EXE201.Commons.Models;
using EXE201.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serenity_Solution.Models;
using System.Globalization;

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
        public IActionResult Index(int YearField = 0, int MonthField = 0)
        {
            // Lấy năm hiện tại nếu người dùng chưa chọn năm (YearField = 0)
            int currentYear = DateTime.Now.Year;
            if (YearField == 0) YearField = currentYear;

            // Lấy toàn bộ dữ liệu cuộc hẹn có thời gian đặt lịch
            // ToList() để chuyển sang xử lý trên bộ nhớ (client-side) vì EF không thể translate các hàm như .Year
            var data = _context.Appointments
                .Where(a => a.Scheduled_time != null)
                .Include(a => a.Psychologist) // Bao gồm thông tin bác sĩ
                .ToList();

            var dataUser = _context.Users.ToList(); // Lấy toàn bộ người dùng để sử dụng trong biểu đồ Line

            var chartLine = dataUser
               .Where(u => u.CreateDate.HasValue && u.CreateDate.Value.Year == YearField &&
                    (MonthField == 0 || u.CreateDate.Value.Month == MonthField))
                .GroupBy(u => MonthField == 0 
                    ? u.CreateDate.Value.Month    // Nếu chọn cả năm: nhóm theo tháng
                    : u.CreateDate.Value.Day)     // Nếu chọn 1 tháng: nhóm theo ngày
                .ToList();

            var chartLineData = chartLine
                .Select(g => new
                {
                    Key = g.Key,
                    Label = MonthField == 0 ? $"Tháng {g.Key}" : g.Key.ToString(),
                    Count = g.Count()
                })
                .OrderBy(x => x.Key)
                .ToList();

            // Lọc theo năm và tháng (nếu có), sau đó nhóm theo tháng (nếu xem cả năm) hoặc theo ngày (nếu xem 1 tháng cụ thể)
            var chartData = data
                .Where(a => a.Scheduled_time.Year == YearField &&
                            (MonthField == 0 || a.Scheduled_time.Month == MonthField))
                .GroupBy(a => MonthField == 0
                    ? a.Scheduled_time.Month    // Nếu chọn cả năm: nhóm theo tháng
                    : a.Scheduled_time.Day)     // Nếu chọn 1 tháng: nhóm theo ngày
                .Select(g => new
                {
                    Key = g.Key,                                // Dùng để sắp xếp
                    Label = MonthField == 0
                        ? $"Tháng {g.Key}"                      // Nhãn trục X: "Tháng n" nếu xem theo năm
                        : g.Key.ToString(),                     // Nhãn trục X: "n" nếu xem theo ngày trong tháng
                    Total = g.Sum(a => a.Price ?? 0)            // Tổng tiền của từng nhóm
                })
                .OrderBy(x => x.Key)                            // Sắp xếp theo thứ tự tháng hoặc ngày tăng dần
                .ToList();

            //loc data cua tung bac si theo khoang thoi gian da chon
            var chartDonutData = data
                .Where(a => a.Scheduled_time.Year == YearField &&
                            (MonthField == 0 || a.Scheduled_time.Month == MonthField))
                .GroupBy(a => a.Psychologist.Name)             // Nhóm theo tên bác sĩ
                .Select(g => new
                {
                    Label = g.Key,                              // Tên bác sĩ
                    Total = g.Sum(a => a.Price ?? 0)           // Tổng doanh thu của bác sĩ đó
                })
                .ToList();

            // Truyền dữ liệu sang view để vẽ biểu đồ
            ViewBag.ChartLabels = chartData.Select(d => d.Label).ToList();  // Nhãn trục X
            ViewBag.ChartData = chartData.Select(d => d.Total).ToList();    // Giá trị trục Y (doanh thu)


            ViewBag.ChartDonutLabels = chartDonutData.Select(d => d.Label).ToList(); // Nhãn cho biểu đồ donut
            ViewBag.ChartDonutData = chartDonutData.Select(d => d.Total).ToList();   // Giá trị cho biểu đồ donut

            // Lưu lại năm/tháng đã chọn để khi render form dropdown không bị mất trạng thái
            ViewBag.SelectedYear = YearField;
            ViewBag.SelectedMonth = MonthField;

            // Nếu không có dữ liệu thì báo lên view (hiển thị alert)
            if (!chartData.Any())
                TempData["NoWSDetail"] = true;

            // Lấy danh sách năm có trong dữ liệu Appointment để đổ dropdown chọn năm
            var years = _context.Appointments
                .Select(a => a.Scheduled_time.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();

            ViewBag.Years = years;

            var AllUssers = _userManager.Users.Count();
            var AllClients = _userManager.GetUsersInRoleAsync("Customer").Result.Count();
            var AllDoctors = _userManager.GetUsersInRoleAsync("Psychologist").Result.Count();
            ViewBag.AllUsers = AllUssers;
            ViewBag.AllClients = AllClients;
            ViewBag.AllDoctors = AllDoctors;
            // Gửi lại thông báo lỗi (nếu có) sang view
            bool noDetails = TempData["NoWSDetail"] as bool? ?? false;
            ViewBag.NoWSDetail = noDetails;

            ViewBag.LineLabels = chartLineData.Select(d => d.Label).ToList();  // Nhãn trục X
            ViewBag.LineData = chartLineData.Select(d => d.Count).ToList();    // Số lượng user đăng ký

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
        #region blogs
            public async Task<IActionResult> AllBlogs(int page = 1, int pageSize = 5)
            {
                var blogs = await _context.Blogs
                    .Where(b => b.Status == false)
                    .Include(b => b.Author)
                    .ToListAsync();
                if (blogs.Count == 0)
                {
                    TempData["NoWSDetail"] = true;
                    return RedirectToAction("Index");
                }
                var blogList = blogs.Select(b => new BlogViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    Content = b.Content,
                    AuthorName = b.Author.Name,
                    CreatedAt = b.CreateDate,
                    ThumbnailUrl = b.ThumbnailUrl
                }).ToList();
                int totalBlogs = blogList.Count();
                var pagedBlogs = blogList.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                ViewBag.TotalPages = (int)Math.Ceiling((double)totalBlogs / pageSize);
                ViewBag.CurrentPage = page;
                return View(pagedBlogs);
            }

        [HttpGet]
        public async Task<IActionResult> BlogDetails(int id)
        {
            var blog = await _context.Blogs.Include(b => b.Author).FirstOrDefaultAsync(b => b.Id == id && b.Status == false);
            if (blog == null)
                return NotFound();
            
            return View(blog);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveBlog(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                TempData["ErrorMessageBlog"] = "Không tìm thấy bài viết.";
                return RedirectToAction("AllBlogs");
            }
            blog.Status = true; // Đánh dấu là đã duyệt
            _context.Blogs.Update(blog);
            await _context.SaveChangesAsync();
            TempData["SuccessMessageBlog"] = "Bài viết đã được duyệt thành công.";
            return RedirectToAction("AllBlogs");
        }
        [HttpPost]
        public async Task<IActionResult> RejectBlog(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                TempData["ErrorMessageBlog"] = "Không tìm thấy bài viết.";
                return RedirectToAction("AllBlogs");
            }
            _context.Blogs.Remove(blog); // Xóa bài viết
            await _context.SaveChangesAsync();
            TempData["SuccessMessageBlog"] = "Bài viết đã bị từ chối và xóa thành công.";
            return RedirectToAction("AllBlogs");
        }
        #endregion

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
                TempData["ErrorUpgrade"] = "Không tìm thấy user.";
                return RedirectToAction("UpgradeRequest");
            }

            // Kiểm tra user có đang ở role Customer không
            var isInCustomerRole = await _userManager.IsInRoleAsync(user, "Customer");
            if (!isInCustomerRole)
            {
                TempData["ErrorUpgrade"] = "User không phải là Customer hoặc đã được nâng cấp.";
                return RedirectToAction("UpgradeRequest");
            }

            // Remove role Customer
            var removeResult = await _userManager.RemoveFromRoleAsync(user, "Customer");
            if (!removeResult.Succeeded)
            {
                TempData["ErrorUpgrade"] = "Xóa role Customer thất bại.";
                return RedirectToAction("UpgradeRequest");
            }

            // Add role Psychologist
            var addResult = await _userManager.AddToRoleAsync(user, "Psychologist");
            if (!addResult.Succeeded)
            {
                TempData["ErrorUpgrade"] = "Thêm role Psychologist thất bại.";
                return RedirectToAction("UpgradeRequest");
            }

            TempData["SuccessUpgrade"] = $"Nâng cấp user {email} thành công.";
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
