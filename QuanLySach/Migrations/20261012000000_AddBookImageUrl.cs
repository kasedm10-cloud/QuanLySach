using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using QuanLySach.Data;

#nullable disable

namespace QuanLySach.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261012000000_AddBookImageUrl")]
    public partial class AddBookImageUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "ImageUrl", table: "Books");
        }
    }
}
