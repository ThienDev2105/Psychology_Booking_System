using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EXE201.Commons.Migrations
{
    /// <inheritdoc />
    public partial class AddBaBalanceToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Name_Major",
                table: "AspNetUsers");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Major",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Major",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Address", "BaBalance", "CertificateUrl", "ConcurrencyStamp", "DateOfBirth", "Degree", "Description", "Discriminator", "Email", "EmailConfirmed", "Experience", "Gender", "LockoutEnabled", "LockoutEnd", "Major", "Name", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "Phone", "PhoneNumber", "PhoneNumberConfirmed", "Price", "ProfilePictureUrl", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "1", 0, "Hà Nội, Việt Nam", 0.0, "certificate1.jpg", "f070eb33-769b-494d-9edf-24d5babfb4c7", null, null, "Chuyên gia tư vấn tâm lý với hơn 10 năm kinh nghiệm", "User", "doctor1@example.com", true, "10 năm", "Unspecified", false, null, "Bác sĩ tâm thần", "Nguyễn Văn A", null, null, null, "0123456789", null, true, 1000000m, "doctor1.jpg", "19097167-5783-486e-8bbb-9eedc85dcdb5", false, "doctor1@example.com" },
                    { "2", 0, "TP.HCM, Việt Nam", 0.0, "certificate2.jpg", "a3fd1b7d-3c7d-4ea2-ac25-cb892d15115d", null, null, "Chuyên gia tư vấn tâm lý trẻ em và gia đình", "User", "doctor2@example.com", true, "8 năm", "Unspecified", false, null, "Nhà tâm lý học lâm sàng", "Trần Thị B", null, null, null, "0987654321", null, true, 800000m, "doctor2.jpg", "f148987f-cd1e-4cdb-9eef-66765b7cfec1", false, "doctor2@example.com" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Name_Major",
                table: "AspNetUsers",
                columns: new[] { "Name", "Major" },
                filter: "[Major] IS NOT NULL");
        }
    }
}
