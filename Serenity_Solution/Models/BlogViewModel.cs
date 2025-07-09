namespace Serenity_Solution.Models
{
    public class BlogViewModel
    {
        public int Id { get; set; }
        public string? Title { get; set; } = string.Empty;
        public string? Content { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; } // URL of the blog image
        public string? Summary { get; set; } // Short summary of the blog content
        public string? BlogType { get; set; } = "Default"; // Bệnh tâm lý phổ biến, Rối loạn tâm trạng, Tâm lý học lâm sàng, Kỹ năng quản lý cảm xúc, Tư vấn và trị liệu tâm lý
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdateDate { get; set; } // Date when the blog was last updated
        public bool Status { get; set; } = false; // Indicates if the blog is active or not
        public string AuthorName { get; set; } = string.Empty;
    }
}
