using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using EXE201.Commons.Data;
using EXE201.Commons.Models;
using EXE201.Services.Interfaces;
using Humanizer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Serenity_Solution.Models;
using Serenity_Solution.Unity;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Serenity_Solution.Controllers
{
    public class AccountController : Controller
    {
        private readonly Cloudinary _cloudinary;
        private readonly IAccountService _accountService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context;

        private readonly IEmailService _emailService;
        public AccountController(UserManager<User> userManager, IAccountService accountService, IEmailService emailService, SignInManager<User> signInManager,
            ApplicationDbContext context, Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
            _accountService = accountService;
            _userManager = userManager;
            _emailService = emailService;
            _signInManager = signInManager;
            _context = context;
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                Random rnd = new Random();
                var NameDefault = "user_" + rnd.Next(1, 1000001);

                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = NameDefault,
                    Phone = model.Phone,
                    DateOfBirth = DateTime.Now,
                    CreateDate = DateTime.Now,
                    Gender = model.Gender,
                    Address = model.Address,
                    ProfilePictureUrl = model.Gender == "Nam" 
                        ? "/image/Logo/AvatarMale.jpg"
                        : "/image/Logo/AvatarFemale.jpg",
                };

                var result = await _accountService.RegisterAsync(user, model.Password);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                    return RedirectToAction("Login");
                }

                // Hiển thị lỗi nếu đăng ký thất bại
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Unexpected error: {ex.Message}");
                return View(model);
            }
        }


        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Demo()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                
                var result = await _accountService.LoginAsync(
                    viewModel.Email.Trim(),
                    viewModel.Password.Trim(),
                    viewModel.RememberMe
                );
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(viewModel.Email.Trim());
                    if (user.Name == "System Admin")
                    {
                        return RedirectToAction("Index", "Manager");
                    }
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Thông tin không hợp lệ. Vui lòng nhập lại!!!.");
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            return RedirectToAction("Login");
        }
        // Display all users
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        public async Task<IActionResult> Index()
        {
            var users = await _accountService.GetAllUsersAsync();
            return View(users);
        }

        // Display user details
        public async Task<IActionResult> Details(string id)
        {
            var id1 = await _accountService.GetCurrentUserIdAsync();
            var user = await _accountService.GetUserByIdAsync(id1);
            if (user == null) return NotFound();
            return View(user);
        }

        public async Task<IActionResult> EditPsychologist(string id)
        {
            var user = await _accountService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            var psychologist = user;  // Ép kiểu về Psychologist

            var viewModel = new PsychologistViewModel
            {
                Id = psychologist.Id,
                Name = psychologist.Name,
                Email = psychologist.Email,
                Phone = psychologist.PhoneNumber,
                Address = psychologist.Address,
                Description = psychologist.Description,
                Experience = psychologist.Experience,
                Price = psychologist.Price,
                ProfilePictureUrl = psychologist.ProfilePictureUrl,
                Major = psychologist.Major
            };
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPsychologist(string id, PsychologistViewModel  model)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            if (!ModelState.IsValid) return View(model);

            var doctor = await _userManager.Users
                .OfType<User>() // Lọc User chỉ lấy Staff
                .FirstOrDefaultAsync(u => u.Id == id);

            if (doctor == null) return NotFound();


            if (!string.IsNullOrEmpty(model.Experience))
            {
                string[] lines = model.Experience.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                List<string> formattedExperience = new List<string>();

                foreach (var rawLine in lines)
                {
                    string line = rawLine.Trim();

                    // Kiểm tra mốc thời gian dạng YYYY-YYYY: ở đầu dòng
                    if (Regex.IsMatch(line, @"^\d{4}-\d{4}:"))
                    {
                        // Nếu chưa có "•" ở đầu thì thêm
                        if (!line.StartsWith("• "))
                        {
                            line = "• " + line;
                        }
                    }
                    else
                    {
                        // Nếu dòng không bắt đầu bằng • thì thêm
                        if (!line.StartsWith("• "))
                        {
                            line = "• " + line;
                        }
                    }

                    formattedExperience.Add(line);
                }

                model.Experience = string.Join("\n", formattedExperience);
            }



            doctor.Name = model.Name;
            doctor.Email = model.Email;
            doctor.PhoneNumber = model.Phone;
            doctor.Address = model.Address;
            doctor.Description = model.Description;
            doctor.Experience = model.Experience; 
            doctor.Price = model.Price;
            doctor.Major = model.Major;


            if (model.ProfilePictureFile != null && model.ProfilePictureFile.Length > 0)
            {
                using var stream = model.ProfilePictureFile.OpenReadStream();

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(model.ProfilePictureFile.FileName, stream),
                    PublicId = $"profile_pictures/customer_{id}_{DateTime.UtcNow.Ticks}",
                    Folder = "profile_pictures"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    doctor.ProfilePictureUrl = uploadResult.SecureUrl.ToString();
                }
                else
                {
                    ModelState.AddModelError("", "Lỗi khi tải ảnh đại diện lên Cloudinary.");
                    return View(model);
                }
            }
            var updateResult = await _userManager.UpdateAsync(doctor);

            if (updateResult.Succeeded)
            {
                return RedirectToAction(nameof(PsychologistProfile));
            }

            // Nếu có lỗi
            foreach (var error in updateResult.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);

        }

        // Edit user - Get (Load user data into form)
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _accountService.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            var customer = user;  // Ép kiểu về Customer

            var viewModel = new EditCustomerViewModel
            {
                Id = customer.Id,
                FullName = customer.Name,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address,
                DateOfBirth = customer.DateOfBirth,
                Gender = customer.Gender,
                ProfilePictureUrl = customer.ProfilePictureUrl
            };

            return View(viewModel);
        }

        // Edit user - Post (Save changes)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditCustomerViewModel model)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            if (!ModelState.IsValid) return View(model);

            var customer = await _userManager.Users
                .OfType<User>() // Lọc User chỉ lấy Staff
                .FirstOrDefaultAsync(u => u.Id == id);

            if (customer == null) return NotFound();

            customer.Name = model.FullName;
            customer.Email = model.Email;
            customer.PhoneNumber = model.Phone;
            customer.Address = model.Address;
            customer.DateOfBirth = model.DateOfBirth;
            customer.Gender = model.Gender;
            //customer.ProfilePictureUrl = model.ProfilePictureUrl;
            // Xử lý upload ảnh lên Cloudinary

            
            if (model.ProfilePictureFile != null && model.ProfilePictureFile.Length > 0)
            {
                using var stream = model.ProfilePictureFile.OpenReadStream();

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(model.ProfilePictureFile.FileName, stream),
                    PublicId = $"profile_pictures/customer_{id}_{DateTime.UtcNow.Ticks}",
                    Folder = "profile_pictures"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    customer.ProfilePictureUrl = uploadResult.SecureUrl.ToString();
                }
                else
                {
                    ModelState.AddModelError("", "Lỗi khi tải ảnh đại diện lên Cloudinary.");
                    return View(model);
                }
            }
            


            var updateResult = await _userManager.UpdateAsync(customer);

            if (updateResult.Succeeded)
            {
                TempData["SuccessInfo"] = "Thay đổi thông tin thành công.";
                return RedirectToAction(nameof(CustomerProfile));
            }

            // Nếu có lỗi
            foreach (var error in updateResult.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        // Delete user - Get confirmation page
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _accountService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        // Delete user - Post (Confirm delete)
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _accountService.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            var result = await _accountService.DeleteUserAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            return View("Error");
        }
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CustomerProfile()
        {
            var user = await _userManager.GetUserAsync(User) ; // Ép kiểu về Customer
            if (user == null) return NotFound();

            var customerViewModel = new CustomerViewModel
            {
                Id = user.Id,
                FullName = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                Address = user.Address,
                ProfilePictureUrl = user.ProfilePictureUrl,  
                CertificateUrl = user.CertificateUrl
            };
            
            return View(customerViewModel);
        }

        [Authorize(Roles = "Psychologist")]
        public async Task<IActionResult> PsychologistProfile()
        {
            var user = await _userManager.GetUserAsync(User) ;

            if (!await _userManager.IsInRoleAsync(user, "Psychologist"))
            {
                return RedirectToAction("Index", "Home");
            }

            var psychologist = await _context.ApplicationUsers
                .Include(p => p.PsychologistAppointments)
                .FirstOrDefaultAsync(p => p.Id == user.Id);


            if (psychologist == null)
                {
                    return RedirectToAction("Index", "Home");
                }

            // map dữ liệu vào view model...

            var experienceFormat = string.IsNullOrEmpty(psychologist.Experience)
                ? ""
                : Regex.Replace(psychologist.Experience, @"(?<=• \d{4}-\d{4}:.*?)(?= • \d{4}-\d{4}:)", "\n");


            var model = new PsychologistViewModel
            {
                Id = psychologist.Id,
                Name = psychologist.Name,
                Email = psychologist.Email,
                Degree = psychologist.CertificateUrl,
                Description = psychologist.Description,
                Phone = psychologist.PhoneNumber,
                Address = user.Address,
                Experience = experienceFormat,
                Price = psychologist.Price,
                ProfilePictureUrl = psychologist.ProfilePictureUrl,
                Major = psychologist.Major,
                BaBalance = psychologist.BaBalance,
            };

            return View(model);
        }
        public async Task<IActionResult> SendTestEmail()
        {
            await _emailService.SendEmailAsync("mylpdde180283@fpt.edu.vn", "Test Email", "<h1>Hello from ASP.NET</h1>");
            return Content("Email Sent Successfully!");
        }

        public async Task<IActionResult> ChangePassword()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user); // Keep user logged in
            TempData["SuccessMessage"] = "Password changed successfully!";
            if (await _userManager.IsInRoleAsync(user, "Customer"))
            {
                return RedirectToAction("CustomerProfile", "Account"); // Redirect to profile page
            }
            return RedirectToAction("StaffProfile", "Account"); // Redirect to profile page

        }

        public async Task<IActionResult> Forgot()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Forgot(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản";
                return View();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = Url.Action("ResetPassword", "Account", new { token, email = user.Email }, Request.Scheme);

            await _emailService.SendEmailAsync(user.Email, "Reset Password", $"Click here to reset your password: <a href='{resetLink}'>Reset Password</a>");

            TempData["SuccessMessage"] = "Link đặt lại đã gửi đến email của bạn";
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Invalid password reset request.";
                return RedirectToAction("Login");
            }

            var model = new ResetPasswordViewModel { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                TempData["ErrorMessage"] = "No account found with this email.";
                return RedirectToAction("Forgot");
            }

            var resetResult = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (!resetResult.Succeeded)
            {
                foreach (var error in resetResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            TempData["SuccessMessage"] = "Password has been reset successfully! You can now log in.";
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> ProfileTest()
        {
            var userId = await _accountService.GetCurrentUserIdAsync();
            var user = await _accountService.GetUserByIdAsync(userId);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpGet]
        public IActionResult UpdateLevel(string id)
        {
            var model = new CertificateUploadViewModel
            {
                CustomerId = id
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateLevel(CertificateUploadViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _accountService.GetUserByIdAsync(model.CustomerId);
            var customer = user;

            if (customer == null)
                return NotFound();

            if (model.CertificateFile != null && model.CertificateFile.Length > 0)
            {
                var fileExt = Path.GetExtension(model.CertificateFile.FileName).ToLower();
                if (fileExt != ".jpg" && fileExt != ".jpeg" && fileExt != ".png")
                {
                    ModelState.AddModelError("CertificateFile", "Chỉ chấp nhận ảnh định dạng JPG, PNG.");
                    return View(model);
                }

                // ✅ Gọi AI để kiểm tra ảnh chứng chỉ
                bool isValid = await AIHelper.IsMedicalCertificateAsync(model.CertificateFile);

                if (!isValid)
                {
                    //hiện thông báo tại UpdateLevel
                    ModelState.AddModelError("CertificateFile", "Ảnh không hợp lệ. Vui lòng tải lên ảnh chứng chỉ hành nghề bác sĩ rõ ràng.");
                    return View(model); // Trả về UI gốc với lỗi
                }

                // ✅ Nếu hợp lệ, upload lên Cloudinary
                using var stream = model.CertificateFile.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(model.CertificateFile.FileName, stream),
                    PublicId = $"certificates/customer_{model.CustomerId}_{DateTime.UtcNow.Ticks}",
                    Folder = "certificates"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    customer.CertificateUrl = uploadResult.SecureUrl.ToString();
                    _context.Update(customer);
                    await _context.SaveChangesAsync();

                    TempData["SuccessUpdate"] = "Chứng chỉ đã được xác nhận và lưu thành công.";
                    return RedirectToAction("CustomerProfile", new { id = model.CustomerId });
                }

                ModelState.AddModelError("", $"Lỗi khi upload lên Cloudinary: {uploadResult.Error?.Message}");
            }
            else
            {
                ModelState.AddModelError("CertificateFile", "Vui lòng chọn ảnh để tải lên.");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Scheduled_Appointments(int page = 1, int pageSize = 5)
        {
            var user = await _userManager.GetUserAsync(User);           

            var ListAppointments = await _context.Appointments
                .Where(a => a.Psychologist_ID == user.Id && a.Status == "Booked")
                .Include(a => a.Client)
                .Include(a => a.Psychologist)
                .ToListAsync();

            if (ListAppointments.Count == 0)
            {
                TempData["NoWSDetail"] = true;
            }

            int totalUsers = ListAppointments.Count();
            var pagedUsers = ListAppointments.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalUsers / pageSize);
            ViewBag.CurrentPage = page;
            
            return View(pagedUsers);
        }
        [HttpPost]
        public async Task<IActionResult> AcceptAppointment(int id)
        {
            var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
            {
                return NotFound();
            }

            appointment.Status = "Confirmed";
            _context.Appointments.Update(appointment);
            _context.SaveChanges();

            if(User.IsInRole("Psychologist"))
            {
                // Gửi email thông báo cho khách hàng
                var emailContent = $"Cuộc hẹn tư vấn của bạn với  {appointment.Psychologist.Name} đã được xác nhận.";
                await _emailService.SendEmailAsync(appointment.Client.Email, "Xác Nhận tư vấn", emailContent);
            }           

            return RedirectToAction(nameof(Scheduled_Appointments));
   
        }
        [HttpPost]
        public async Task<IActionResult> RejectAppointment(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Client)
                .Include(a => a.Psychologist).FirstOrDefaultAsync(a => a.Id == id);

            var check = appointment.Psychologist.Name;
            var checkemail = appointment.Psychologist.Email;

            if (appointment == null)
            {
                return NotFound();
            }

            appointment.Status = "Canceled";

            // 2. delete conversation
            var clientBooked = appointment.Client;
            var currentUser = await _userManager.GetUserAsync(User);
            
            var existingConversation = await _context.Conversations
            .FirstOrDefaultAsync(c =>
                (c.User1Id == currentUser.Id && c.User2Id == clientBooked.Id) ||
                (c.User1Id == clientBooked.Id && c.User2Id == currentUser.Id));

            if (existingConversation != null)
            {
                // 3. Nếu chưa có -> Tạo Conversation mới
                _context.Conversations.Remove(existingConversation);
                await _context.SaveChangesAsync();
            }

            if (User.IsInRole("Psychologist"))
            {
                // Gửi email thông báo cho khách hàng
                await _emailService.SendEmailAsync(appointment.Client.Email, 
                    "Xác Nhận tư vấn", 
                    $"Cuộc hẹn tư vấn của bạn với  {appointment.Psychologist.Name} đã bị từ chối.");
            }
            else
            {
                // Gửi email thông báo cho bác sĩ
                await _emailService.SendEmailAsync(appointment.Psychologist.Email, 
                    "Hủy cuộc hẹn", 
                    $"Cuộc hẹn tư vấn của bạn với  {appointment.Client.Name} đã bị hủy.");
            }
            _context.Appointments.Remove(appointment);
            _context.SaveChanges();

            return RedirectToAction(nameof(Scheduled_Appointments));
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CustomerDashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Lấy các dữ liệu cần thiết cho dashboard
            var appointments = _context.Appointments
                .Include(a => a.Psychologist)
                .Where(a => a.Client_ID == user.Id)  // Sửa CustomerId thành Client_ID
                .OrderByDescending(a => a.Scheduled_time)  // Sửa AppointmentDate thành Scheduled_time
                .ToList();

            // Tạo ViewModel cho dashboard
            var dashboardViewModel = new CustomerDashboardViewModel
            {
                Customer = new CustomerViewModel
                {
                    Id = user.Id,
                    FullName = user.Name,
                    Email = user.Email,
                    Phone = user.PhoneNumber,
                    DateOfBirth = user.DateOfBirth,
                    Gender = user.Gender,
                    Address = user.Address,
                    ProfilePictureUrl = user.ProfilePictureUrl,
                    CertificateUrl = user.CertificateUrl
                },


               RecentAppointments = appointments.Take(5).ToList(),
                
                TotalAppointments = appointments.Count(),  // Thêm () để gọi phương thức Count()
                
            };
            
            return View(dashboardViewModel);
        }

        public IActionResult LoginWithGoogle()
        {
            var redirectUrl = Url.Action("GoogleResponse", "Account");

            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return Challenge(properties, "Google");

        }

        public async Task<IActionResult> GoogleResponse()
        {
            try
            {
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    Console.WriteLine("External login info is null");
                    return RedirectToAction("Login");
                }

                Console.WriteLine($"Login Provider: {info.LoginProvider}");
                Console.WriteLine($"Provider Key: {info.ProviderKey}");

                // Tìm xem user đã từng login bằng Google chưa
                var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
                if (result.Succeeded)
                {
                    Console.WriteLine("User signed in successfully");
                    return RedirectToAction("Index", "Home");
                }

                // Nếu chưa có, tạo user mới
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var name = info.Principal.FindFirstValue(ClaimTypes.Name);

                if (string.IsNullOrEmpty(email))
                {
                    Console.WriteLine("Email not found in Google response");
                    return RedirectToAction("Login");
                }

                // Kiểm tra xem email đã tồn tại chưa
                var existingUser = await _userManager.FindByEmailAsync(email);
                if (existingUser != null)
                {
                    // User đã tồn tại nhưng chưa link với Google
                    var addLoginResult = await _userManager.AddLoginAsync(existingUser, info);
                    if (addLoginResult.Succeeded)
                    {
                        await _signInManager.SignInAsync(existingUser, isPersistent: false);
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    // Tạo user mới
                    var user = new User
                    {
                        UserName = email,
                        Email = email,
                        Name = name,
                        DateOfBirth = DateTime.Now,
                        EmailConfirmed = true // Google email đã được verify
                    };

                    var identityResult = await _userManager.CreateAsync(user);
                    if (identityResult.Succeeded)
                    {
                        identityResult = await _userManager.AddLoginAsync(user, info);
                        if (identityResult.Succeeded)
                        {
                            // Thêm vai trò Customer
                            await _userManager.AddToRoleAsync(user, "Customer");
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return RedirectToAction("Index", "Home");
                        }
                    }

                    // Log errors nếu có
                    foreach (var error in identityResult.Errors)
                    {
                        Console.WriteLine($"Identity Error: {error.Description}");
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

                return RedirectToAction("Login");
            }
            catch
            {
                ModelState.AddModelError("", "Lỗi khi đăng nhập bằng Google. Vui lòng thử lại sau.");
                return RedirectToAction("Login");
            }
        }

    }
}
