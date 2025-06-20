using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuiTanThanh_2280602928_W3.Migrations
{
    /// <inheritdoc />
    public partial class UpdateShiftModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Shifts");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "Shifts",
                newName: "Date");

            migrationBuilder.AddColumn<int>(
                name: "ShiftType",
                table: "Shifts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShiftType",
                table: "Shifts");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Shifts",
                newName: "StartTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "Shifts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
