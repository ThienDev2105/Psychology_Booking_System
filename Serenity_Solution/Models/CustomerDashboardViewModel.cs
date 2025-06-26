
using EXE201.Commons.Models;


namespace Serenity_Solution.Models
{
    public class CustomerDashboardViewModel
    {
        public CustomerViewModel Customer { get; set; }
        public List<Appointment> RecentAppointments { get; set; }
        public int TotalAppointments { get; set; }
        public List<Notification> Notifications { get; set; } = new List<Notification>();
    }
} 

