using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TPpweb.Migrations
{
    public partial class gestorid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GestorId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GestorId",
                table: "AspNetUsers");
        }
    }
}
