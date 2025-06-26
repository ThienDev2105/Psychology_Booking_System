
using System.ComponentModel.DataAnnotations;

namespace Serenity_Solution.Models
{
    public class DASSTestModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime TestDate { get; set; } = DateTime.Now;

        // Theo dõi câu hỏi đã trả lời
        public List<bool> AnsweredQuestions { get; set; } = new List<bool>();
        public int AnsweredCount { get; set; } = 0;

        // Câu hỏi DASS-21
        [Range(0, 3, ErrorMessage = "Vui lòng chọn một giá trị từ 0 đến 3")]
        public int Question1 { get; set; } // Tôi thấy khó mà thoải mái được
        
        [Range(0, 3, ErrorMessage = "Vui lòng chọn một giá trị từ 0 đến 3")]
        public int Question2 { get; set; } // Tôi bị khô miệng
        
        [Range(0, 3, ErrorMessage = "Vui lòng chọn một giá trị từ 0 đến 3")]
        public int Question3 { get; set; } // Tôi không thấy có chút cảm xúc tích cực nào
        
        [Range(0, 3, ErrorMessage = "Vui lòng chọn một giá trị từ 0 đến 3")]
        public int Question4 { get; set; } // Tôi bị rối loạn nhịp thở
        
        [Range(0, 3, ErrorMessage = "Vui lòng chọn một giá trị từ 0 đến 3")]
        public int Question5 { get; set; } // Tôi thấy khó bắt tay vào công việc
        
        [Range(0, 3, ErrorMessage = "Vui lòng chọn một giá trị từ 0 đến 3")]
        public int Question6 { get; set; } // Tôi phản ứng thái quá với mọi tình huống
        
        [Range(0, 3, ErrorMessage = "Vui lòng chọn một giá trị từ 0 đến 3")]
        public int Question7 { get; set; } // Tôi bị ra mồ hôi
        
        [Range(0, 3, ErrorMessage = "Vui lòng chọn một giá trị từ 0 đến 3")]
        public int Question8 { get; set; } // Tôi cảm thấy sợ hãi
        
        [Range(0, 3, ErrorMessage = "Vui lòng chọn một giá trị từ 0 đến 3")]
        public int Question9 { get; set; } // Tôi lo lắng về những tình huống có thể khiến tôi hoảng sợ
        
        [Range(0, 3, ErrorMessage = "Vui lòng chọn một giá trị từ 0 đến 3")]
        public int Question10 { get; set; } // Tôi thấy mình chẳng có gì để mong đợi cả
        
        [Range(0, 3, ErrorMessage = "Vui lòng chọn một giá trị từ 0 đến 3")]
        public int Question11 { get; set; } // Tôi thấy bản thân dễ bị kích động
        
        [Range(0, 3, ErrorMessage = "Vui lòng chọn một giá trị từ 0 đến 3")]
        public int Question12 { get; set; } // Tôi thấy khó thư giãn được
        
        [Range(0, 3, ErrorMessage = "Vui lòng chọn một giá trị từ 0 đến 3")]
        public int Question13 { get; set; } // Tôi cảm thấy chán nản, thất vọng
        
        [Range(0, 3, ErrorMessage = "Vui lòng chọn một giá trị từ 0 đến 3")]
        public int Question14 { get; set; } // Tôi không chấp nhận được việc có cái gì đó xen vào cản trở việc tôi đang làm
        
        [Range(0, 3, ErrorMessage = "Vui lòng chọn một giá trị từ 0 đến 3")]
        public int Question15 { get; set; } // Tôi thấy mình gần như hoảng loạn
        
        [Range(0, 3, ErrorMessage = "Vui lòng chọn một giá trị từ 0 đến 3")]
        public int Question16 { get; set; } // Tôi không thấy hăng hái với bất kỳ việc gì nữa
        
        [Range(0, 3, ErrorMessage = "Vui lòng chọn một giá trị từ 0 đến 3")]
        public int Question17 { get; set; } // Tôi cảm thấy mình chẳng đáng làm người
        
        [Range(0, 3, ErrorMessage = "Vui lòng chọn một giá trị từ 0 đến 3")]
        public int Question18 { get; set; } // Tôi thấy mình khá dễ phật ý, tự ái
        
        [Range(0, 3, ErrorMessage = "Vui lòng chọn một giá trị từ 0 đến 3")]
        public int Question19 { get; set; } // Tôi nghe thấy rõ tiếng nhịp tim dù chẳng làm việc gì cả
        
        [Range(0, 3, ErrorMessage = "Vui lòng chọn một giá trị từ 0 đến 3")]
        public int Question20 { get; set; } // Tôi hay sợ vô cớ
        
        [Range(0, 3, ErrorMessage = "Vui lòng chọn một giá trị từ 0 đến 3")]
        public int Question21 { get; set; } // Tôi thấy cuộc sống vô nghĩa

        // Kết quả
        public int DepressionScore { get; set; }
        public int AnxietyScore { get; set; }
        public int StressScore { get; set; }
        public string DepressionLevel { get; set; }
        public string AnxietyLevel { get; set; }
        public string StressLevel { get; set; }

        // Phương thức tính điểm và xác định mức độ
        public void CalculateScores()
        {
            // Tính điểm theo thang đo DASS-21
            // Trầm cảm: Câu 3, 5, 10, 13, 16, 17, 21
            DepressionScore = Question3 + Question5 + Question10 + Question13 + Question16 + Question17 + Question21;
            
            // Lo âu: Câu 2, 4, 7, 9, 15, 19, 20
            AnxietyScore = Question2 + Question4 + Question7 + Question9 + Question15 + Question19 + Question20;
            
            // Stress: Câu 1, 6, 8, 11, 12, 14, 18
            StressScore = Question1 + Question6 + Question8 + Question11 + Question12 + Question14 + Question18;

            // Nhân đôi điểm số vì thang đo DASS-42 gốc có 42 câu
            DepressionScore *= 2;
            AnxietyScore *= 2;
            StressScore *= 2;

            // Xác định mức độ
            DepressionLevel = GetDepressionLevel(DepressionScore);
            AnxietyLevel = GetAnxietyLevel(AnxietyScore);
            StressLevel = GetStressLevel(StressScore);
        }

        private string GetDepressionLevel(int score)
        {
            if (score <= 9) return "Bình thường";
            if (score <= 13) return "Nhẹ";
            if (score <= 20) return "Vừa";
            if (score <= 27) return "Nặng";
            return "Rất nặng";
        }

        private string GetAnxietyLevel(int score)
        {
            if (score <= 7) return "Bình thường";
            if (score <= 9) return "Nhẹ";
            if (score <= 14) return "Vừa";
            if (score <= 19) return "Nặng";
            return "Rất nặng";
        }

        private string GetStressLevel(int score)
        {
            if (score <= 14) return "Bình thường";
            if (score <= 18) return "Nhẹ";
            if (score <= 25) return "Vừa";
            if (score <= 33) return "Nặng";
            return "Rất nặng";
        }
    }
}
