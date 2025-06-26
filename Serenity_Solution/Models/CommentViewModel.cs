using System.ComponentModel.DataAnnotations;

namespace Serenity_Solution.Models
{
    public class CommentViewModel
    {
        public int CommetId { get; set; }
        [Required(ErrorMessage = "Nội dung comment không được để trống")]
        [StringLength(500, ErrorMessage = "Comment không được quá 500 ký tự")]
        public string CommentContent { get; set; }

        public DateTime DateComment { get; set; }
        public string? UserId { get; set; }
        public int? ParentCommentId { get; set; }
        public int? PodcastID { get; set; }

        // Additional properties for display - these are set by controller, not from form
        public string? UserName { get; set; }
        public string? UserAvatar { get; set; }

        // For rating functionality - allow 0 (no rating) to 5
        [Range(0, 5, ErrorMessage = "Rating phải từ 0 đến 5 sao")]
        public int Rating { get; set; } = 0; // Default to 0 (no rating)

        // For nested comments - these are not from form
        public List<CommentViewModel>? Replies { get; set; } = new List<CommentViewModel>();
        public CommentViewModel? ParentComment { get; set; }
    }
}
