using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StateMachineExperiments.Migrations
{
    /// <inheritdoc />
    public partial class ConvertCurrentStateToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CurrentState",
                table: "LodCases",
                type: "INTEGER",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50);

            migrationBuilder.CreateTable(
                name: "FormalLodCases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CaseNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CurrentState = table.Column<int>(type: "INTEGER", maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MemberId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    MemberName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: true),
                    IsDeathCase = table.Column<bool>(type: "INTEGER", nullable: false),
                    ToxicologyRequired = table.Column<bool>(type: "INTEGER", nullable: false),
                    ToxicologyComplete = table.Column<bool>(type: "INTEGER", nullable: false),
                    AppealFiled = table.Column<bool>(type: "INTEGER", nullable: false),
                    InvestigatingOfficerId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    InvestigatingOfficerName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    InvestigationStartDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InvestigationCompletionDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeterminationResult = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormalLodCases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FormalTransitionHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FormalLodCaseId = table.Column<int>(type: "INTEGER", nullable: false),
                    FromState = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ToState = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Trigger = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PerformedByAuthority = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormalTransitionHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormalTransitionHistory_FormalLodCases_FormalLodCaseId",
                        column: x => x.FormalLodCaseId,
                        principalTable: "FormalLodCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormalLodCases_CaseNumber",
                table: "FormalLodCases",
                column: "CaseNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormalTransitionHistory_FormalLodCaseId",
                table: "FormalTransitionHistory",
                column: "FormalLodCaseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormalTransitionHistory");

            migrationBuilder.DropTable(
                name: "FormalLodCases");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentState",
                table: "LodCases",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldMaxLength: 50);
        }
    }
}
