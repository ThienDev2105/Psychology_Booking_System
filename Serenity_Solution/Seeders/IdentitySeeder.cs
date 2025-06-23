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
            if (await userManager.FindByEmailAsync("Thang152@gamil.com") == null)
            {
                var user = new Psychologist
                {
                    UserName = "Lê Văn Thắng",
                    Email = "Thang152@gamil.com",
                    Name = "Lê Văn Thắng",
                    Phone = "123456789",
                    Major = "Tâm lý học trẻ em",
                    Address = "HCM, Việt Nam",
                    Degree = "~image/Degree/cunhantamly.jpg",
                    Description = "Nhà tâm lý học có nhiều năm kinh nghiệm trong ngành.",
                    Experience = "10 years",
                    Price = 1000000,
                    ProfilePictureUrl = "~image/Doctor/Van_Thang.png",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "Default@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Psychologist");
                }
            }
            //Kim nguyen 
            if (await userManager.FindByEmailAsync("KimNguyen2105@gamil.com") == null)
            {
                var user = new Psychologist
                {
                    UserName = "Kim Nguyễn",
                    Email = "KimNguyen2105@gamil.com",
                    Name = "Kim Nguyễn",
                    Phone = "0933555777",
                    Major = "Tâm lý học trẻ em",
                    Address = "Cần Thơ, Việt Nam",
                    Degree = "~image/Degree/cunhantamly.jpg",
                    Description = "Tư vấn tâm lý cho trẻ em và thanh thiếu niên.",
                    Experience = "6 years",
                    Price = 750000,
                    ProfilePictureUrl = "~image/Doctor/Kim_Nguan.png",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "Default@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Psychologist");
                }
            }
        }

    }
}
