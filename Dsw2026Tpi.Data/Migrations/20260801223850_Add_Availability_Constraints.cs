using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dsw2026Tpi.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Availability_Constraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AvailabilityRules_Doctors_DoctorId",
                table: "AvailabilityRules");

            migrationBuilder.DropForeignKey(
                name: "FK_AvailabilitySlots_AvailabilityRules_AvailabilityRuleId",
                table: "AvailabilitySlots");

            migrationBuilder.DropForeignKey(
                name: "FK_AvailabilitySlots_Doctors_DoctorId",
                table: "AvailabilitySlots");

            migrationBuilder.DropIndex(
                name: "IX_AvailabilitySlots_DoctorId",
                table: "AvailabilitySlots");

            migrationBuilder.DropIndex(
                name: "IX_AvailabilityRules_DoctorId",
                table: "AvailabilityRules");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "AvailabilitySlots",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_AvailabilitySlots_DoctorId_SlotDate_StartTime",
                table: "AvailabilitySlots",
                columns: new[] { "DoctorId", "SlotDate", "StartTime" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AvailabilityRules_DoctorId_Year_Month_DayOfWeek_StartTime_EndTime",
                table: "AvailabilityRules",
                columns: new[] { "DoctorId", "Year", "Month", "DayOfWeek", "StartTime", "EndTime" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AvailabilityRules_Doctors_DoctorId",
                table: "AvailabilityRules",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AvailabilitySlots_AvailabilityRules_AvailabilityRuleId",
                table: "AvailabilitySlots",
                column: "AvailabilityRuleId",
                principalTable: "AvailabilityRules",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_AvailabilitySlots_Doctors_DoctorId",
                table: "AvailabilitySlots",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AvailabilityRules_Doctors_DoctorId",
                table: "AvailabilityRules");

            migrationBuilder.DropForeignKey(
                name: "FK_AvailabilitySlots_AvailabilityRules_AvailabilityRuleId",
                table: "AvailabilitySlots");

            migrationBuilder.DropForeignKey(
                name: "FK_AvailabilitySlots_Doctors_DoctorId",
                table: "AvailabilitySlots");

            migrationBuilder.DropIndex(
                name: "IX_AvailabilitySlots_DoctorId_SlotDate_StartTime",
                table: "AvailabilitySlots");

            migrationBuilder.DropIndex(
                name: "IX_AvailabilityRules_DoctorId_Year_Month_DayOfWeek_StartTime_EndTime",
                table: "AvailabilityRules");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "AvailabilitySlots",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.CreateIndex(
                name: "IX_AvailabilitySlots_DoctorId",
                table: "AvailabilitySlots",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_AvailabilityRules_DoctorId",
                table: "AvailabilityRules",
                column: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_AvailabilityRules_Doctors_DoctorId",
                table: "AvailabilityRules",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AvailabilitySlots_AvailabilityRules_AvailabilityRuleId",
                table: "AvailabilitySlots",
                column: "AvailabilityRuleId",
                principalTable: "AvailabilityRules",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AvailabilitySlots_Doctors_DoctorId",
                table: "AvailabilitySlots",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
