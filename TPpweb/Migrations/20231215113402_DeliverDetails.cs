using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TPpweb.Migrations
{
    public partial class DeliverDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "Details",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OptionalEquipment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Damages = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DamageImages = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Observations = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FkEmployer = table.Column<int>(type: "int", nullable: false),
                    ReservationId = table.Column<int>(type: "int", nullable: false),
                    ReservationIdReservation = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Details", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Details_Employer_FkEmployer",
                        column: x => x.FkEmployer,
                        principalTable: "Employer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Details_Reservations_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservations",
                        principalColumn: "IdReservation",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Details_Reservations_ReservationIdReservation",
                        column: x => x.ReservationIdReservation,
                        principalTable: "Reservations",
                        principalColumn: "IdReservation");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Details_FkEmployer",
                table: "Details",
                column: "FkEmployer");

            migrationBuilder.CreateIndex(
                name: "IX_Details_ReservationId",
                table: "Details",
                column: "ReservationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Details_ReservationIdReservation",
                table: "Details",
                column: "ReservationIdReservation",
                unique: true,
                filter: "[ReservationIdReservation] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Details");

            migrationBuilder.AddColumn<string>(
                name: "GestorId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
