using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EXE201.Commons.Migrations
{
    /// <inheritdoc />
    public partial class FixBlog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_AspNetUsers_CustomerId",
                table: "Blogs");

            migrationBuilder.DropIndex(
                name: "IX_Blogs_CustomerId",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Blogs");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                table: "Blogs",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_AuthorId",
                table: "Blogs",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_AspNetUsers_AuthorId",
                table: "Blogs",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_AspNetUsers_AuthorId",
                table: "Blogs");

            migrationBuilder.DropIndex(
                name: "IX_Blogs_AuthorId",
                table: "Blogs");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                table: "Blogs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                table: "Blogs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_CustomerId",
                table: "Blogs",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_AspNetUsers_CustomerId",
                table: "Blogs",
                column: "CustomerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
