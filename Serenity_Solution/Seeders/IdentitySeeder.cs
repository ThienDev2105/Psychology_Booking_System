using EXE201.Commons.Models;
using Microsoft.AspNetCore.Identity;

namespace Serenity_Solution.Seeders
{
    public class IdentitySeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            //them  
            
            string[] roleNames = { "Admin", "Psychologist", "Customer" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new ApplicationRole(roleName));
                }
            } 
            // Tìm hoặc tạo user Admin
            if (userManager.Users.All(u => u.Email != "admin@example.com"))
            {
                var adminUser = new Admin
                {
                    UserName = "admin@example.com",
                    Email = "admin@example.com",
                    Name = "System Admin",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // psychologist
            if (await userManager.FindByEmailAsync("Thang152@gmail.com") == null)
            {
                var user = new Psychologist
                {
                    UserName = "Thang152@gmail.com",
                    Email = "Thang152@gmail.com",
                    Name = "Lê Văn Thắng",
                    Phone = "123456789",
                    PhoneNumber = "123456789",
                    Major = "Tư vấn viên tâm lý",
                    Address = "HCM, Việt Nam",
                    Degree = "/image/Degree/cunhantamly.jpg",
                    CertificateUrl = "/image/Degree/cunhantamly.jpg",
                    Description = "Nhà tâm lý học có nhiều năm kinh nghiệm trong ngành.",
                    Experience = "2024–nay: Tham gia dự án hỗ trợ tâm lý học đường cho học sinh THPT tại TP.HCM, tư vấn các trường hợp stress, lo âu, áp lực học tập.",
                    Price = 120000,
                    ProfilePictureUrl = "/image/Doctor/Van_Thang.png",
                    BaBalance = 0,
                    Gender = "Male",
                    HasPaidDASS21Test = false,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "Thang152@");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Psychologist");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Lỗi tạo người dùng: {error.Code} - {error.Description}");
                    }
                }
            }
            //Kim nguyen 
            if (await userManager.FindByEmailAsync("KimNguyen2105@gmail.com") == null)
            {
                var user = new Psychologist
                {
                    UserName = "KimNguyen2105@gmail.com",
                    Email = "KimNguyen2105@gmail.com",
                    Name = "Kim Nguyễn",
                    Phone = "0933555777",
                    PhoneNumber = "0933555777",
                    Major = "Nhà tâm lý học lâm sàng",
                    Address = "Cần Thơ, Việt Nam",
                    CertificateUrl = "/image/Degree/cunhantamly.jpg",
                    Degree = "/image/Degree/cunhantamly.jpg",
                    Description = "Tư vấn tâm lý cho trẻ em và thanh thiếu niên.",
                    Experience = "2020–2023: Làm việc tại Bệnh viện Tâm thần Trung Ương, chuyên khoa Tâm lý trị liệu cho người trưởng thành. 2023–2024: Thực hiện trị liệu nhận thức – hành vi (CBT) cho bệnh nhân trầm cảm nhẹ tại phòng khám tư nhân.",
                    Price = 500000,
                    ProfilePictureUrl = "/image/Doctor/Kim_Ngan.png",
                    BaBalance = 0,
                    Gender = "Female",
                    HasPaidDASS21Test = false,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "KimNguyen2105@");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Psychologist");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Lỗi tạo người dùng: {error.Code} - {error.Description}");
                    }
                }
            }
        }
        //add psychologist
        public static async Task SeedDataPsychologist(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            // Tạo role nếu chưa có
            /*
            string[] roles = new[] { "Psychologist", "Customer", "Admin" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new ApplicationRole(role));
                }
            }*/

            // Lê Văn Thắng
            
        }

    }
}
