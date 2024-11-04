using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TPpweb.Migrations
{
    public partial class fotofile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<byte[]>(
                name: "Fotografia",
                table: "Houses",
                type: "varbinary(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Fotografia",
                table: "Houses");

            migrationBuilder.AddColumn<string>(
                name: "GestorId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
