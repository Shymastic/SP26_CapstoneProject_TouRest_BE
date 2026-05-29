using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TouRest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicalResults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "medical_results",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PassengerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_medical_results", x => x.Id);
                    table.ForeignKey(
                        name: "FK_medical_results_booking_passengers_PassengerId",
                        column: x => x.PassengerId,
                        principalTable: "booking_passengers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_medical_results_itinerary_schedule_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "itinerary_schedule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_medical_results_providers_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "providers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "medical_result_images",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MedicalResultId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_medical_result_images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_medical_result_images_medical_results_MedicalResultId",
                        column: x => x.MedicalResultId,
                        principalTable: "medical_results",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_medical_result_images_MedicalResultId",
                table: "medical_result_images",
                column: "MedicalResultId");

            migrationBuilder.CreateIndex(
                name: "IX_medical_results_PassengerId_ScheduleId",
                table: "medical_results",
                columns: new[] { "PassengerId", "ScheduleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_medical_results_ProviderId",
                table: "medical_results",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_medical_results_ScheduleId",
                table: "medical_results",
                column: "ScheduleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "medical_result_images");

            migrationBuilder.DropTable(
                name: "medical_results");
        }
    }
}
