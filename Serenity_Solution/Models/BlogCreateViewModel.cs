using System.ComponentModel.DataAnnotations;

namespace Serenity_Solution.Models
{
    public class BlogCreateViewModel
    {
        public string Title { get; set; }

        public IFormFile Thumbnail { get; set; }

        public string BlogType { get; set; }

        public string Summary { get; set; }

        public IFormFile ContentFile { get; set; } // .doc/.docx file
    }

}
