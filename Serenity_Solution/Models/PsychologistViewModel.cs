namespace Serenity_Solution.Models
{
    public class PsychologistViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string Address { get; set; }
        public string? Degree { get; set; }
        public string? Description { get; set; }
        public string Experience { get; set; }
        public decimal Price { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Major { get; set; } = "Chưa cập nhật";
        public double BaBalance { get; set; } = 0;
        public IFormFile? ProfilePictureFile { get; set; }
        
    }
}
