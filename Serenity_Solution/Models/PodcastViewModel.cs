
using System;

namespace Serenity_Solution.Models

{
    public class PodcastViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string AudioUrl { get; set; }
        public string Author { get; set; }
        public string Duration { get; set; }
        public string Category { get; set; }
        public DateTime? PublishedDate { get; set; }
        public double Rating { get; set; }
        public int RatingCount { get; set; }
    }
} 

