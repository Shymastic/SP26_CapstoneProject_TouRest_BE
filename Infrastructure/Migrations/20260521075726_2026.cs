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

            // ImageUrls may already exist — add conditionally
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1 FROM sys.columns
                    WHERE object_id = OBJECT_ID(N'reports') AND name = N'ImageUrls'
                )
                BEGIN
                    ALTER TABLE [reports] ADD [ImageUrls] nvarchar(max) NULL;
                END
            ");

            // Status column for itinerary_schedule
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1 FROM sys.columns
                    WHERE object_id = OBJECT_ID(N'itinerary_schedule') AND name = N'Status'
                )
                BEGIN
                    ALTER TABLE [itinerary_schedule] ADD [Status] int NOT NULL DEFAULT 0;
                END
            ");

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
