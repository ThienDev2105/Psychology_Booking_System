using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXE201.Commons.Models
{
    public class Blog
    {
        [Key]
        public int Id { get; set; }
        public string? Title { get; set; } = string.Empty;
        public string? Content { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; } // URL of the blog image
        public string? Summary { get; set; } // Short summary of the blog content
        public string? BlogType { get; set; } = "Default"; // Bệnh tâm lý phổ biến, Rối loạn tâm trạng, Tâm lý học lâm sàng, Kỹ năng quản lý cảm xúc, Tư vấn và trị liệu tâm lý
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public DateTime? UpdateDate { get; set; } // Date when the blog was last updated
        public bool Status { get; set; } = false; // Indicates if the blog is active or not
        public string? AuthorId { get; set; } = string.Empty;
        [ForeignKey("AuthorId")]
        public virtual User? Author { get; set; }
    }
}
