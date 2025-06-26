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
using System.Text.Json;

namespace Serenity_Solution.Controllers
{
    [Route("[controller]")]
    public class TestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IVnPayServicecs _vpnPayServicecs;
        private readonly IEmailService _emailService;

        public TestController(ApplicationDbContext context, UserManager<User> userManager, IVnPayServicecs vnPayServicecs, IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _vpnPayServicecs = vnPayServicecs;
            _emailService = emailService;
        }

        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            bool hasPaid = false;

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                hasPaid = user?.HasPaidDASS21Test ?? false;
            }

            ViewData["HasPaidDASS21Test"] = hasPaid;
            return View();
        }
        [Authorize]
        [HttpPost]
        [Route("Payment")]
        public IActionResult Payment([FromBody] TestPaymentRequest request)
        {
            try
            {
                double TestPrice = 29000;

                var userBooking = _userManager.GetUserAsync(User).Result;
                if (userBooking == null)
                    return Unauthorized("User not logged in");

                var orderId = new Random().Next(1000, 100000);
                var vnPayModel = new TestPaymentRequest
                {
                    Amount = TestPrice,
                    CreateDate = DateTime.Now,
                    Description = $"Thanh toán thực hiện bài test",
                    FullName = userBooking.Name,
                    OrderId = orderId,
                    TestType = request.TestType
                };
                var paymentUrl = _vpnPayServicecs.CreateTestPaymentUrl(HttpContext, vnPayModel);

                return Json(new { success = true, redirectUrl = paymentUrl });
            }
            catch (Exception ex)
            {
                // Log lỗi nếu có
                Console.WriteLine(ex.ToString());
                return Json(new { success = false, message = "Có lỗi xảy ra khi tạo thanh toán" });
            }
        }

        [Authorize]
        [HttpGet]
        [Route("PaymentCallBack")]
        public async Task<IActionResult> PaymentCallBackAsync()
        {
            try
            {
                var response = _vpnPayServicecs.TestPaymentExecute(Request.Query);
                var code = response.VnPayResponseCode;
                if (response == null || response.VnPayResponseCode != "00")
                {
                    TempData["Testerror"] = "Lỗi thanh toán";
                    return RedirectToAction("Index");

                }
                var currentUser = await _userManager.GetUserAsync(User);

                if (currentUser != null)
                {
                    await _emailService.SendEmailAsync(currentUser.Email, "Thanh toán thành công", $"Bạn đã thanh toán chi phí cho bài Test");
                    currentUser.HasPaidDASS21Test = true;
                    await _userManager.UpdateAsync(currentUser);
                }
                var adminAmount = _userManager.Users.FirstOrDefault(u => u.Email == "admin@example.com");
                if (adminAmount != null)
                {
                    adminAmount.BaBalance += 29000; // Cộng 29,000 vào số dư của admin
                    await _userManager.UpdateAsync(adminAmount);
                }
                await _context.SaveChangesAsync();

                TempData["Testsuccess"] = "Thanh toán thành công";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log lỗi nếu có
                Console.Write(ex.ToString());
                TempData["Testerror"] = "Có lỗi xảy ra trong quá trình xử lý";
                return RedirectToAction("Index");
            }

        }



        [HttpGet]
        [Route("DASS21")]
        public IActionResult DASS21()
        {
            var model = new DASSTestModel();
            return View(model);
        }

        [HttpPost]
        [Route("DASS21")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DASS21(DASSTestModel model, string AnsweredQuestions, int AnsweredCount)
        {
            try
            {
                // Chuyển đổi chuỗi JSON thành danh sách bool
                if (!string.IsNullOrEmpty(AnsweredQuestions))
                {
                    model.AnsweredQuestions = JsonSerializer.Deserialize<List<bool>>(AnsweredQuestions);
                    // Đảm bảo AnsweredCount khớp với số lượng câu hỏi đã trả lời
                    model.AnsweredCount = model.AnsweredQuestions.Count(q => q);
                }
                else
                {
                    model.AnsweredQuestions = new List<bool>();
                    model.AnsweredCount = 0;
                }

                // Tính toán điểm số
                model.CalculateScores();

                // Lưu kết quả nếu người dùng đã đăng nhập
                if (User.Identity.IsAuthenticated)
                {
                    var user = await _userManager.GetUserAsync(User);
                    model.UserId = user.Id;
                }

                // Log để debug
                System.Diagnostics.Debug.WriteLine($"Total questions answered: {model.AnsweredCount}");
                System.Diagnostics.Debug.WriteLine($"Answered questions array: {string.Join(", ", model.AnsweredQuestions)}");

                // Chuyển đến trang kết quả với các tham số cần thiết
                return RedirectToAction("DASS21Result", new
                {
                    depression = model.DepressionScore,
                    anxiety = model.AnxietyScore,
                    stress = model.StressScore,
                    depressionLevel = model.DepressionLevel,
                    anxietyLevel = model.AnxietyLevel,
                    stressLevel = model.StressLevel,
                    answeredCount = model.AnsweredCount,
                    answeredQuestions = AnsweredQuestions
                });
            }
            catch (Exception ex)
            {
                // Log lỗi nếu có
                System.Diagnostics.Debug.WriteLine("Lỗi khi xử lý form DASS21: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);
                return View(model);
            }
        }

        [HttpGet]
        [Route("DASS21Result")]
        public IActionResult DASS21Result(
            int depression,
            int anxiety,
            int stress,
            string depressionLevel,
            string anxietyLevel,
            string stressLevel,
            int answeredCount,
            string answeredQuestions)
        {
            // Ensure we have values even if they weren't provided
            depression = Math.Max(0, depression);
            anxiety = Math.Max(0, anxiety);
            stress = Math.Max(0, stress);

            depressionLevel = string.IsNullOrEmpty(depressionLevel) ? "Bình thường" : depressionLevel;
            anxietyLevel = string.IsNullOrEmpty(anxietyLevel) ? "Bình thường" : anxietyLevel;
            stressLevel = string.IsNullOrEmpty(stressLevel) ? "Bình thường" : stressLevel;

            // Set the values in ViewData
            ViewData["DepressionScore"] = depression;
            ViewData["AnxietyScore"] = anxiety;
            ViewData["StressScore"] = stress;
            ViewData["DepressionLevel"] = depressionLevel;
            ViewData["AnxietyLevel"] = anxietyLevel;
            ViewData["StressLevel"] = stressLevel;

            // Thêm dữ liệu về câu hỏi đã trả lời
            ViewData["AnsweredCount"] = answeredCount;
            ViewData["AnsweredQuestions"] = answeredQuestions ?? "[]"; // Provide a default if null
            ViewData["TotalQuestions"] = 21;

            return View();
        }

    }
}

