using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TouRest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _2026 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_itinerary_schedule_users_GuideId",
                table: "itinerary_schedule");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrls",
                table: "reports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "itinerary_schedule",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_itinerary_schedule_users_GuideId",
                table: "itinerary_schedule",
                column: "GuideId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_itinerary_schedule_users_GuideId",
                table: "itinerary_schedule");

            migrationBuilder.DropColumn(
                name: "ImageUrls",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "itinerary_schedule");

            migrationBuilder.AddForeignKey(
                name: "FK_itinerary_schedule_users_GuideId",
                table: "itinerary_schedule",
                column: "GuideId",
                principalTable: "users",
                principalColumn: "Id");
        }
    }
}
