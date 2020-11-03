using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cosette.Tuner.Web.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chromosomes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreationTimeUtc = table.Column<DateTime>(nullable: false),
                    ElapsedTime = table.Column<double>(nullable: false),
                    Fitness = table.Column<int>(nullable: false),
                    ReferenceEngineWins = table.Column<int>(nullable: false),
                    ExperimentalEngineWins = table.Column<int>(nullable: false),
                    Draws = table.Column<int>(nullable: false),
                    Errors = table.Column<int>(nullable: false),
                    ChromosomeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chromosomes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chromosomes_Chromosomes_ChromosomeId",
                        column: x => x.ChromosomeId,
                        principalTable: "Chromosomes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreationTimeUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EngineStatistics",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreationTimeUtc = table.Column<DateTime>(nullable: false),
                    IsReferenceEngine = table.Column<bool>(nullable: false),
                    ChromosomeId = table.Column<int>(nullable: false),
                    AverageTimePerGame = table.Column<double>(nullable: false),
                    AverageDepth = table.Column<double>(nullable: false),
                    AverageNodesCount = table.Column<double>(nullable: false),
                    AverageNodesPerSecond = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EngineStatistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EngineStatistics_Chromosomes_ChromosomeId",
                        column: x => x.ChromosomeId,
                        principalTable: "Chromosomes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Generations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TestId = table.Column<int>(nullable: false),
                    CreationTimeUtc = table.Column<DateTime>(nullable: false),
                    ElapsedTime = table.Column<double>(nullable: false),
                    BestFitness = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Generations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Generations_Tests_TestId",
                        column: x => x.TestId,
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChromosomeGenes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Value = table.Column<int>(nullable: false),
                    ChromosomeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChromosomeGenes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChromosomeGenes_Generations_ChromosomeId",
                        column: x => x.ChromosomeId,
                        principalTable: "Generations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenerationGenes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Value = table.Column<int>(nullable: false),
                    GenerationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenerationGenes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenerationGenes_Generations_GenerationId",
                        column: x => x.GenerationId,
                        principalTable: "Generations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChromosomeGenes_ChromosomeId",
                table: "ChromosomeGenes",
                column: "ChromosomeId");

            migrationBuilder.CreateIndex(
                name: "IX_Chromosomes_ChromosomeId",
                table: "Chromosomes",
                column: "ChromosomeId");

            migrationBuilder.CreateIndex(
                name: "IX_EngineStatistics_ChromosomeId",
                table: "EngineStatistics",
                column: "ChromosomeId");

            migrationBuilder.CreateIndex(
                name: "IX_GenerationGenes_GenerationId",
                table: "GenerationGenes",
                column: "GenerationId");

            migrationBuilder.CreateIndex(
                name: "IX_Generations_TestId",
                table: "Generations",
                column: "TestId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChromosomeGenes");

            migrationBuilder.DropTable(
                name: "EngineStatistics");

            migrationBuilder.DropTable(
                name: "GenerationGenes");

            migrationBuilder.DropTable(
                name: "Chromosomes");

            migrationBuilder.DropTable(
                name: "Generations");

            migrationBuilder.DropTable(
                name: "Tests");
        }
    }
}
