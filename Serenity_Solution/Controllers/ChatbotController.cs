using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Collections.Generic;


namespace Serenity_Solution.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        private static readonly Dictionary<string, string> _responses = new Dictionary<string, string>
        {
            { "xin chào", "Xin chào! Tôi có thể giúp gì cho bạn hôm nay?" },
            { "hello", "Xin chào! Tôi có thể giúp gì cho bạn hôm nay?" },
            { "hi", "Xin chào! Tôi có thể giúp gì cho bạn hôm nay?" },
            { "làm sao thực hiện kiểm tra tâm lý", "Để thực hiện kiểm tra tâm lý:<br/>• Bấm vào mục \"Kiểm tra tâm lý\" tại đầu trang hoặc \"Kiểm tra sức khỏe nhận thức\"" },
            { "làm sao để thực hiện kiểm tra tâm lý", "Để thực hiện kiểm tra tâm lý:<br/>• Bấm vào mục \"Kiểm tra tâm lý\" tại đầu trang hoặc \"Kiểm tra sức khỏe nhận thức\"" },
            { "làm sao để thực hiện bài kiểm tra tâm lý", "Để thực hiện kiểm tra tâm lý:<br/>• Bấm vào mục \"Kiểm tra tâm lý\" tại đầu trang hoặc \"Kiểm tra sức khỏe nhận thức\"" },
            { "tôi có thể tâm sự với cùng bạn không", "Tất nhiên! Tôi luôn ở đây để lắng nghe bạn. Bạn có thể chia sẻ bất cứ điều gì đang khiến bạn lo lắng hoặc buồn phiền." },
            { "tôi có thể tâm sự với cùng bạn không?", "Tất nhiên! Tôi luôn ở đây để lắng nghe bạn. Bạn có thể chia sẻ bất cứ điều gì đang khiến bạn lo lắng hoặc buồn phiền." },
            { "tôi cần tư vấn thêm về sức khỏe tâm lý", "Chúng tôi có các chuyên gia tâm lý giàu kinh nghiệm. Bạn có thể xem danh sách các bác sĩ và đặt lịch tư vấn trực tiếp tại mục 'Danh sách bác sĩ' trên menu chính." },
            { "tôi cần thêm thông tin về chuyên gia tâm lý", "Bạn có thể xem thông tin chi tiết về các chuyên gia tâm lý tại mục 'Danh sách bác sĩ'. Tại đó có thông tin về kinh nghiệm, chuyên môn và đánh giá từ khách hàng." },
            { "podcast hôm nay", "Chúng tôi có nhiều podcast mới về sức khỏe tâm lý. Bạn có thể truy cập mục 'Podcast' để nghe các bài nói chuyện về quản lý stress, lo âu và nhiều chủ đề khác." },
            { "podcast hôm nay?", "Chúng tôi có nhiều podcast mới về sức khỏe tâm lý. Bạn có thể truy cập mục 'Podcast' để nghe các bài nói chuyện về quản lý stress, lo âu và nhiều chủ đề khác." },
            { "đặt lịch hẹn", "Để đặt lịch hẹn với chuyên gia tâm lý, vui lòng truy cập mục 'Danh sách bác sĩ', chọn chuyên gia phù hợp và nhấn nút 'Đặt lịch hẹn'." },
            { "làm sao đặt lịch hẹn", "Để đặt lịch hẹn với chuyên gia tâm lý, vui lòng truy cập mục 'Danh sách bác sĩ', chọn chuyên gia phù hợp và nhấn nút 'Đặt lịch hẹn'." },
            { "cảm ơn", "Rất vui được giúp đỡ bạn! Nếu bạn cần thêm thông tin gì, đừng ngần ngại hỏi tôi nhé." },
        };

        [HttpPost("query")]
        public IActionResult GetResponse([FromBody] ChatbotQuery query)
        {
            if (string.IsNullOrWhiteSpace(query?.Message))
            {
                return BadRequest(new { error = "Message cannot be empty" });
            }

            string message = query.Message.ToLower();
            string response = "Xin chào! Tôi có thể giúp gì cho bạn hôm nay?";

            // Tìm kiếm câu trả lời chính xác
            if (_responses.TryGetValue(message, out string exactMatch))
            {
                response = exactMatch;
            }
            else
            {
                // Tìm kiếm câu trả lời theo từ khóa
                foreach (var pair in _responses)
                {
                    if (message.Contains(pair.Key))
                    {
                        response = pair.Value;
                        break;
                    }
                }
            }

            return Ok(new { response });
        }

        public class ChatbotQuery
        {
            public string Message { get; set; }
        }
    }

} 

