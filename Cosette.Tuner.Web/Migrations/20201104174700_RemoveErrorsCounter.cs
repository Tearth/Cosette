using Microsoft.EntityFrameworkCore.Migrations;

namespace Cosette.Tuner.Web.Migrations
{
    public partial class RemoveErrorsCounter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Errors",
                table: "Chromosomes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Errors",
                table: "Chromosomes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
