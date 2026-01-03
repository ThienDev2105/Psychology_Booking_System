using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXE201.Commons.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; } = string.Empty;
        //public string Email {  get; set; } = string.Empty;
        public string Phone {  get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }

        //customer
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; } = "Unspecified"; // Male, Female, Other
        public string? CertificateUrl { get; set; }
        public bool HasPaidDASS21Test { get; set; } = false;
        public DateTime? DatePayTest { get; set; } // Date when the user last logged in

        [NotMapped]
        public bool? DoctorCertificateUrl { get; set; } = false; // Indicates if the user has a doctor certificate
        public DateTime? CreateDate { get; set; } = DateTime.Now; // Date when the user was created
        //psychologist
        public string? Degree { get; set; }
        public string? Description { get; set; } = string.Empty;
        public string Experience { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } = 0;
        public string? Major { get; set; } = string.Empty; // Major in psychology or related field

        public double BaBalance { get; set; } = 0; // Balance for in-app purchases or services

        //public virtual ICollection<IdentityUserRole<string>> UserRoles { get; set; } = new List<IdentityUserRole<string>>(); // có thể test xóa
        public virtual ICollection<PodcastRating> PodcastRatings { get; set; } = new List<PodcastRating>();
        public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

        public virtual ICollection<Blog> BlogsCreated { get; set; } = new List<Blog>();

        //customer
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Appointment> ClientAppointments { get; set; }
        public ICollection<Appointment> PsychologistAppointments { get; set; }
        public virtual ICollection<UserTestResult> UserTestResults { get; set; } = new List<UserTestResult>();

        //psychologist

    }
}
