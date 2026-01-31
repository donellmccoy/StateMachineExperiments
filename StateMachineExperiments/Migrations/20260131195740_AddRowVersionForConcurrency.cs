using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StateMachineExperiments.Migrations
{
    /// <inheritdoc />
    public partial class AddRowVersionForConcurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "LodCases",
                type: "BLOB",
                rowVersion: true,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "LodCases");
        }
    }
}
