using EXE201.Commons.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXE201.Commons.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        //public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public virtual DbSet<Appointment> Appointments { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        //public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Podcast> Podcasts { get; set; }
        public virtual DbSet<PodcastRating> PodcastRatings { get; set; }
        public virtual DbSet<Psychologist> Psychologists { get; set; }
        public virtual DbSet<User> ApplicationUsers { get; set; }
        public virtual DbSet<UserTestResult> UserTestResults { get; set; }
        public virtual DbSet<PsychTest> PsychTests { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<Conversation> Conversations { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Blog> Blogs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
         
            // PodcastRating -> User (cascade delete)
            modelBuilder.Entity<PodcastRating>()
                .HasOne(pr => pr.User)
                .WithMany(u => u.PodcastRatings)
                .HasForeignKey(pr => pr.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // PodcastRating -> Podcast (no cascade delete)
            modelBuilder.Entity<PodcastRating>()
                .HasOne(pr => pr.Podcast)
                .WithMany(p => p.PodcastRatings)
                .HasForeignKey(pr => pr.PodcastID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Client)
                .WithMany(u => u.ClientAppointments)
                .HasForeignKey(a => a.Client_ID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Psychologist)
                .WithMany(u => u.PsychologistAppointments)
                .HasForeignKey(a => a.Psychologist_ID)
                .OnDelete(DeleteBehavior.Restrict);
            //them
            modelBuilder.Entity<Conversation>()
                .HasMany(c => c.Messages)
                .WithOne(m => m.Conversation)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.User1)
                .WithMany()
                .HasForeignKey(c => c.User1Id)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.User2)
                .WithMany()
                .HasForeignKey(c => c.User2Id)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.LastMessage)
                .WithMany()
                .HasForeignKey(c => c.LastMessageId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
