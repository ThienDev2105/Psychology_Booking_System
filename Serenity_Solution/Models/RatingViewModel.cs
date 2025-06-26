using System.ComponentModel.DataAnnotations;

namespace Serenity_Solution.Models
{
    public class RatingViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn số sao")]
        [Range(1, 5, ErrorMessage = "Rating phải từ 1 đến 5 sao")]
        public int Stars { get; set; }

        public string UserName { get; set; }
        public string UserAvatar { get; set; }
        public DateTime CreatedAt { get; set; }
        public int PodcastId { get; set; }
    }
}
