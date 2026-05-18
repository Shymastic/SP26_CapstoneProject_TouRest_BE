using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TouRest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddScheduleSpotAndGuide : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "providers",
                type: "decimal(11,8)",
                precision: 11,
                scale: 8,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "providers",
                type: "decimal(11,8)",
                precision: 11,
                scale: 8,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<Guid>(
                name: "GuideId",
                table: "itinerary_schedule",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Spot",
                table: "itinerary_schedule",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SpotLeft",
                table: "itinerary_schedule",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "agencies",
                type: "decimal(11,8)",
                precision: 11,
                scale: 8,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "agencies",
                type: "decimal(11,8)",
                precision: 11,
                scale: 8,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.CreateIndex(
                name: "IX_itinerary_schedule_GuideId",
                table: "itinerary_schedule",
                column: "GuideId");

            migrationBuilder.AddForeignKey(
                name: "FK_itinerary_schedule_users_GuideId",
                table: "itinerary_schedule",
                column: "GuideId",
                principalTable: "users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_itinerary_schedule_users_GuideId",
                table: "itinerary_schedule");

            migrationBuilder.DropIndex(
                name: "IX_itinerary_schedule_GuideId",
                table: "itinerary_schedule");

            migrationBuilder.DropColumn(
                name: "GuideId",
                table: "itinerary_schedule");

            migrationBuilder.DropColumn(
                name: "Spot",
                table: "itinerary_schedule");

            migrationBuilder.DropColumn(
                name: "SpotLeft",
                table: "itinerary_schedule");

            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "providers",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(11,8)",
                oldPrecision: 11,
                oldScale: 8);

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "providers",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(11,8)",
                oldPrecision: 11,
                oldScale: 8);

            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "agencies",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(11,8)",
                oldPrecision: 11,
                oldScale: 8);

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "agencies",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(11,8)",
                oldPrecision: 11,
                oldScale: 8);
        }
    }
}
