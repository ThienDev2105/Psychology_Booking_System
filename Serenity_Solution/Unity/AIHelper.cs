using Google.Cloud.Vision.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.Build.Logging;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Serenity_Solution.Unity
{
    public static class AIHelper
    {
        // Không cần chỉnh đường dẫn tuyệt đối. Chúng ta dùng relative path đến thư mục gốc
        private const string CredentialFileName = "serenity-solution-fe7d19f14900.json";

        public static async Task<bool> IsMedicalCertificateAsync(IFormFile file)
        {
            // Xác định đường dẫn tuyệt đối đến serenity.json
            var credentialPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CredentialFileName);
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);

            // Tạo Google Vision client
            var client = await ImageAnnotatorClient.CreateAsync();

            // Đọc nội dung ảnh từ IFormFile
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            ms.Seek(0, SeekOrigin.Begin);
            var image = Image.FromStream(ms);

            // Gọi OCR API
            var response = await client.DetectDocumentTextAsync(image);
            var extractedText = response?.Text?.ToLower() ?? "";

            Console.WriteLine("📄 Văn bản OCR:\n" + extractedText);

            // Từ khóa để xác định là chứng chỉ bác sĩ tâm lý
            string[] keywords = {
            "giấy phép", "chứng chỉ", "tâm lý", "bác sĩ", "tham vấn",
            "psychologist", "mental health", "counselor", "certificate", "license"
        };

            return keywords.Any(keyword => extractedText.Contains(keyword));
        }
    }
}
