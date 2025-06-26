using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EXE201.Commons.Migrations
{
    /// <inheritdoc />
    public partial class AddHasPaidDASS21TestToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasPaidDASS21Test",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasPaidDASS21Test",
                table: "AspNetUsers");
        }
    }
}
