using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using EXE201.Commons.Data;
using EXE201.Commons.Models;
using EXE201.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serenity_Solution.Models;
using System.Text;
using System.Threading.Tasks;

namespace Serenity_Solution.Controllers
{
    public class BlogsController : Controller
    {
        private readonly Cloudinary _cloudinary;
        private readonly IAccountService _accountService;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        public BlogsController(Cloudinary cloudinary, IAccountService accountService, UserManager<User> userManager, ApplicationDbContext context)
        {
            _cloudinary = cloudinary;
            _accountService = accountService;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var blogs = await _context.Blogs
                    .Where(b => b.Status == true)
                    .OrderByDescending(b => b.CreateDate)
                    .Include(b => b.Author)
                    .ToListAsync();


                if (!blogs.Any())
                    TempData["NoWSDetail"] = true;

                bool noDetails = TempData["NoWSDetail"] as bool? ?? false;
                ViewBag.NoWSDetail = noDetails;

                return View(blogs);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return Content("Lỗi: " + ex.Message + (ex.InnerException?.Message ?? ""));
            }
        }

        public async Task<IActionResult> Detail(int id)
        {
            var blog = await _context.Blogs.Include(b => b.Author).FirstOrDefaultAsync(b => b.Id == id && b.Status == true);
            if (blog == null)
                return NotFound();

            return View(blog);
        }

        public IActionResult Create()
        {           
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Psychologist")]
        public async Task<IActionResult> Create(BlogCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorBlog"] = "Vui lòng nhập đủ thông tin!";
                return View(model);
            }
                

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                TempData["ErrorBlog"] = "Bạn cần đăng nhập để tạo blog!";
                return RedirectToAction("Login", "Account");
            }

            // Upload thumbnail to Cloudinary
            var thumbnailUrl = await UploadThumbnailToCloudinary(model.Thumbnail);

            // Convert Word to HTML content
            string contentHtml;
            using (var stream = model.ContentFile.OpenReadStream())
            {
                contentHtml = ConvertWordToHtml(stream);
            }

            var blog = new Blog
            {
                Title = model.Title,
                ThumbnailUrl = thumbnailUrl,
                BlogType = model.BlogType,
                Summary = model.Summary,
                Content = contentHtml,
                Status = false, // chờ duyệt
                AuthorId = user.Id,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now
            };

            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();

            TempData["SuccessBlog"] = "Blog đã được tạo, đang chờ phê duyệt!";
            return RedirectToAction("Index");
        }

        private string ConvertWordToHtml(Stream docxStream)
        {
            var doc = new Aspose.Words.Document(docxStream);

            var options = new Aspose.Words.Saving.HtmlSaveOptions
            {
                ExportImagesAsBase64 = true,
                ExportHeadersFootersMode = Aspose.Words.Saving.ExportHeadersFootersMode.None,
                PrettyFormat = true,
                CssStyleSheetType = Aspose.Words.Saving.CssStyleSheetType.Embedded,
                // Các tùy chọn khác
            };

            // Xử lý hình ảnh
            foreach (Aspose.Words.Drawing.Shape shape in doc.GetChildNodes(Aspose.Words.NodeType.Shape, true))
            {
                if (shape.HasImage && shape.Width > 500)
                {
                    double ratio = 500.0 / shape.Width;
                    shape.Width = 500;
                    shape.Height *= ratio;
                }
            }

            using (var ms = new MemoryStream())
            {
                doc.Save(ms, options);
                var html = Encoding.UTF8.GetString(ms.ToArray());

                // Thêm CSS vào HTML sau khi convert
                return InjectCustomCss(html);
            }
        }

        private string InjectCustomCss(string html)
        {
            // Tạo wrapper div với class đặc biệt
            string wrappedHtml = $"<div class=\"docx-generated-content\">{html}</div>";

            // CSS sẽ chỉ áp dụng cho phần tử có class docx-generated-content
            string customCss = @"
    <style>
    /* CSS chỉ áp dụng cho nội dung từ DOCX */
    .docx-generated-content {
        font-family: 'Arial', sans-serif;
        line-height: 1.6;
        color: #333;
        max-width: 800px;
        margin: 20px auto;
        padding: 20px;
        background: #fff;
    }
    
    .docx-generated-content h1,
    .docx-generated-content h2,
    .docx-generated-content h3 {
        color: #2c3e50;
        margin-top: 1.5em;
        font-family: 'Georgia', serif;
        text-align: center;
    }
    
    .docx-generated-content img {
        max-width: 100%;
        height: auto;
        display: block;
        margin: 15px auto;
    }
    
    .docx-generated-content table {
        border-collapse: collapse;
        width: 100%;
        margin: 20px 0;
    }
    
    .docx-generated-content table,
    .docx-generated-content th,
    .docx-generated-content td {
        border: 1px solid #ddd;
    }
    
    .docx-generated-content blockquote {
        background-color: #f8f9fa;
        border-left: 4px solid #5bc0be;
        padding: 1rem;
        margin: 1rem 0;
    }
    
    /* Responsive cho mobile */
    @@media (max-width: 768px) {
        .docx-generated-content {
            padding: 10px;
        }
    }
    </style>
    ";

            // Tìm vị trí thẻ </head> để chèn CSS
            int headEndIndex = wrappedHtml.IndexOf("</head>");

            if (headEndIndex >= 0)
            {
                return wrappedHtml.Insert(headEndIndex, customCss);
            }

            // Nếu không tìm thấy thẻ head, thêm cả CSS và wrapper
            return customCss + wrappedHtml;
        }

        /*
        private string ConvertWordToHtml(Stream docxStream)
        {
            var doc = new Aspose.Words.Document(docxStream);

            var options = new Aspose.Words.Saving.HtmlSaveOptions
            {
                ExportImagesAsBase64 = true,
                ExportHeadersFootersMode = Aspose.Words.Saving.ExportHeadersFootersMode.PerSection,
                PrettyFormat = true
            };

            using (var ms = new MemoryStream())
            {
                doc.Save(ms, options);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
        */
        private async Task<string> UploadThumbnailToCloudinary(IFormFile file)
        {
            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream)
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult.SecureUrl.ToString();
            }
        }



    }
}
