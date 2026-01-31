using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StateMachineExperiments.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LodCases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CaseNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CurrentState = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MemberId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    MemberName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LodCases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransitionHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LodCaseId = table.Column<int>(type: "INTEGER", nullable: false),
                    FromState = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ToState = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Trigger = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PerformedByAuthority = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransitionHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransitionHistory_LodCases_LodCaseId",
                        column: x => x.LodCaseId,
                        principalTable: "LodCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LodCases_CaseNumber",
                table: "LodCases",
                column: "CaseNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransitionHistory_LodCaseId",
                table: "TransitionHistory",
                column: "LodCaseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransitionHistory");

            migrationBuilder.DropTable(
                name: "LodCases");
        }
    }
}
