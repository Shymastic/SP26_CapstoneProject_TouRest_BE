using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TouRest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProviderIdToItineraryStop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProviderId",
                table: "itinerary_stops",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_itinerary_stops_ProviderId",
                table: "itinerary_stops",
                column: "ProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_itinerary_stops_providers_ProviderId",
                table: "itinerary_stops",
                column: "ProviderId",
                principalTable: "providers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_itinerary_stops_providers_ProviderId",
                table: "itinerary_stops");

            migrationBuilder.DropIndex(
                name: "IX_itinerary_stops_ProviderId",
                table: "itinerary_stops");

            migrationBuilder.DropColumn(
                name: "ProviderId",
                table: "itinerary_stops");
        }
    }
}
