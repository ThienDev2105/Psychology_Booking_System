namespace Serenity_Solution.Models
{
    public class PodcastDetailViewModel
    {
        public PodcastViewModel Podcast { get; set; }
        public List<CommentViewModel> Comments { get; set; } = new List<CommentViewModel>();
        public CommentViewModel NewComment { get; set; } = new CommentViewModel();
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public int UserRating { get; set; } // Rating của user hiện tại
    }
}
