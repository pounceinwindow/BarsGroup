using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MatrixUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompetitionsMatrix");

            migrationBuilder.DropTable(
                name: "VacancyCompetition");

            migrationBuilder.DropTable(
                name: "Competitions");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Candidates",
                newName: "FullName");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "Candidates",
                newName: "Email");

            migrationBuilder.AlterColumn<string[]>(
                name: "PreviousWork",
                table: "Candidates",
                type: "character varying(200)[]",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string[]>(
                name: "Education",
                table: "Candidates",
                type: "character varying(200)[]",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "Skills",
                table: "Candidates",
                type: "character varying(200)[]",
                maxLength: 20,
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string>(
                name: "Telegram",
                table: "Candidates",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Competencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Competencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompetencyMatrices",
                columns: table => new
                {
                    InterviewId = table.Column<int>(type: "integer", nullable: false),
                    CompetencyId = table.Column<int>(type: "integer", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompetencyMatrices", x => new { x.InterviewId, x.CompetencyId });
                    table.ForeignKey(
                        name: "FK_CompetencyMatrices_Competencies_CompetencyId",
                        column: x => x.CompetencyId,
                        principalTable: "Competencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompetencyMatrices_Interviews_InterviewId",
                        column: x => x.InterviewId,
                        principalTable: "Interviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VacancyCompetencies",
                columns: table => new
                {
                    VacancyId = table.Column<int>(type: "integer", nullable: false),
                    CompetencyId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VacancyCompetencies", x => new { x.VacancyId, x.CompetencyId });
                    table.ForeignKey(
                        name: "FK_VacancyCompetencies_Competencies_CompetencyId",
                        column: x => x.CompetencyId,
                        principalTable: "Competencies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VacancyCompetencies_Vacancies_VacancyId",
                        column: x => x.VacancyId,
                        principalTable: "Vacancies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompetencyMatrices_CompetencyId",
                table: "CompetencyMatrices",
                column: "CompetencyId");

            migrationBuilder.CreateIndex(
                name: "IX_VacancyCompetencies_CompetencyId",
                table: "VacancyCompetencies",
                column: "CompetencyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompetencyMatrices");

            migrationBuilder.DropTable(
                name: "VacancyCompetencies");

            migrationBuilder.DropTable(
                name: "Competencies");

            migrationBuilder.DropColumn(
                name: "Skills",
                table: "Candidates");

            migrationBuilder.DropColumn(
                name: "Telegram",
                table: "Candidates");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "Candidates",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Candidates",
                newName: "FirstName");

            migrationBuilder.AlterColumn<string>(
                name: "PreviousWork",
                table: "Candidates",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string[]),
                oldType: "character varying(200)[]",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Education",
                table: "Candidates",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string[]),
                oldType: "character varying(200)[]",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Competitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Competitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompetitionsMatrix",
                columns: table => new
                {
                    CandidateId = table.Column<int>(type: "integer", nullable: false),
                    CompetitionId = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    Score = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompetitionsMatrix", x => new { x.CandidateId, x.CompetitionId });
                    table.ForeignKey(
                        name: "FK_CompetitionsMatrix_Candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Candidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompetitionsMatrix_Competitions_CompetitionId",
                        column: x => x.CompetitionId,
                        principalTable: "Competitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VacancyCompetition",
                columns: table => new
                {
                    VacancyId = table.Column<int>(type: "integer", nullable: false),
                    CompetitionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VacancyCompetition", x => new { x.VacancyId, x.CompetitionId });
                    table.ForeignKey(
                        name: "FK_VacancyCompetition_Competitions_CompetitionId",
                        column: x => x.CompetitionId,
                        principalTable: "Competitions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VacancyCompetition_Vacancies_VacancyId",
                        column: x => x.VacancyId,
                        principalTable: "Vacancies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionsMatrix_CompetitionId",
                table: "CompetitionsMatrix",
                column: "CompetitionId");

            migrationBuilder.CreateIndex(
                name: "IX_VacancyCompetition_CompetitionId",
                table: "VacancyCompetition",
                column: "CompetitionId");
        }
    }
}
