using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitaEnergy.ProjectTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionAndDueDateToMilestones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "Milestones");

            migrationBuilder.DropColumn(
                name: "Comments",
                table: "Milestones");

            migrationBuilder.DropColumn(
                name: "DateActual",
                table: "Milestones");

            migrationBuilder.RenameColumn(
                name: "Workstream",
                table: "Milestones",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "DatePlanned",
                table: "Milestones",
                newName: "DueDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DueDate",
                table: "Milestones",
                newName: "DatePlanned");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Milestones",
                newName: "Workstream");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Milestones",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "Milestones",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateActual",
                table: "Milestones",
                type: "datetime2",
                nullable: true);
        }
    }
}
