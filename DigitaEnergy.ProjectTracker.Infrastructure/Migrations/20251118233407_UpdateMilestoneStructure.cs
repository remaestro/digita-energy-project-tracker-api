using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitaEnergy.ProjectTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMilestoneStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Milestones");

            migrationBuilder.RenameColumn(
                name: "DueDate",
                table: "Milestones",
                newName: "DatePlanned");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Milestones",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "Milestones",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateActual",
                table: "Milestones",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Workstream",
                table: "Milestones",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "Workstream",
                table: "Milestones");

            migrationBuilder.RenameColumn(
                name: "DatePlanned",
                table: "Milestones",
                newName: "DueDate");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Milestones",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
