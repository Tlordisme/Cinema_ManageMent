using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CM_API.Migrations
{
    /// <inheritdoc />
    public partial class Final : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CMTicketId",
                table: "TicketSeats",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CMRoomId",
                table: "Seats",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Foods",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Foods", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketSeats_CMTicketId",
                table: "TicketSeats",
                column: "CMTicketId");

            migrationBuilder.CreateIndex(
                name: "IX_Seats_CMRoomId",
                table: "Seats",
                column: "CMRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Seats_Rooms_CMRoomId",
                table: "Seats",
                column: "CMRoomId",
                principalTable: "Rooms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketSeats_Tickets_CMTicketId",
                table: "TicketSeats",
                column: "CMTicketId",
                principalTable: "Tickets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Seats_Rooms_CMRoomId",
                table: "Seats");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketSeats_Tickets_CMTicketId",
                table: "TicketSeats");

            migrationBuilder.DropTable(
                name: "Foods");

            migrationBuilder.DropIndex(
                name: "IX_TicketSeats_CMTicketId",
                table: "TicketSeats");

            migrationBuilder.DropIndex(
                name: "IX_Seats_CMRoomId",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "CMTicketId",
                table: "TicketSeats");

            migrationBuilder.DropColumn(
                name: "CMRoomId",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "Rooms");
        }
    }
}
