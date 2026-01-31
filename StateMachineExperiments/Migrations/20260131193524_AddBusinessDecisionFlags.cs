using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StateMachineExperiments.Migrations
{
    /// <inheritdoc />
    public partial class AddBusinessDecisionFlags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AppealFiled",
                table: "LodCases",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "EstimatedCost",
                table: "LodCases",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InjurySeverity",
                table: "LodCases",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresLegalReview",
                table: "LodCases",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresWingReview",
                table: "LodCases",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppealFiled",
                table: "LodCases");

            migrationBuilder.DropColumn(
                name: "EstimatedCost",
                table: "LodCases");

            migrationBuilder.DropColumn(
                name: "InjurySeverity",
                table: "LodCases");

            migrationBuilder.DropColumn(
                name: "RequiresLegalReview",
                table: "LodCases");

            migrationBuilder.DropColumn(
                name: "RequiresWingReview",
                table: "LodCases");
        }
    }
}
