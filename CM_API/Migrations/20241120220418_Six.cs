using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CM_API.Migrations
{
    /// <inheritdoc />
    public partial class Six : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CMShowtime_Movies_MovieID",
                table: "CMShowtime");

            migrationBuilder.DropForeignKey(
                name: "FK_CMShowtime_Rooms_RoomID",
                table: "CMShowtime");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_CMShowtime_ShowtimeId",
                table: "Tickets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CMShowtime",
                table: "CMShowtime");

            migrationBuilder.RenameTable(
                name: "CMShowtime",
                newName: "Showtimes");

            migrationBuilder.RenameIndex(
                name: "IX_CMShowtime_RoomID",
                table: "Showtimes",
                newName: "IX_Showtimes_RoomID");

            migrationBuilder.RenameIndex(
                name: "IX_CMShowtime_MovieID",
                table: "Showtimes",
                newName: "IX_Showtimes_MovieID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Showtimes",
                table: "Showtimes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Showtimes_Movies_MovieID",
                table: "Showtimes",
                column: "MovieID",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Showtimes_Rooms_RoomID",
                table: "Showtimes",
                column: "RoomID",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Showtimes_ShowtimeId",
                table: "Tickets",
                column: "ShowtimeId",
                principalTable: "Showtimes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Showtimes_Movies_MovieID",
                table: "Showtimes");

            migrationBuilder.DropForeignKey(
                name: "FK_Showtimes_Rooms_RoomID",
                table: "Showtimes");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Showtimes_ShowtimeId",
                table: "Tickets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Showtimes",
                table: "Showtimes");

            migrationBuilder.RenameTable(
                name: "Showtimes",
                newName: "CMShowtime");

            migrationBuilder.RenameIndex(
                name: "IX_Showtimes_RoomID",
                table: "CMShowtime",
                newName: "IX_CMShowtime_RoomID");

            migrationBuilder.RenameIndex(
                name: "IX_Showtimes_MovieID",
                table: "CMShowtime",
                newName: "IX_CMShowtime_MovieID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CMShowtime",
                table: "CMShowtime",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CMShowtime_Movies_MovieID",
                table: "CMShowtime",
                column: "MovieID",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CMShowtime_Rooms_RoomID",
                table: "CMShowtime",
                column: "RoomID",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_CMShowtime_ShowtimeId",
                table: "Tickets",
                column: "ShowtimeId",
                principalTable: "CMShowtime",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
