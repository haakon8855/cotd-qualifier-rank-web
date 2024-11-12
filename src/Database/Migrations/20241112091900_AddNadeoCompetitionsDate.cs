using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CotdQualifierRank.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddNadeoCompetitionsDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "NadeoCompetitions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            
            migrationBuilder.Sql(@"
                UPDATE NadeoCompetitions
                SET Date = CONVERT(date, SUBSTRING(Name, PATINDEX('%[0-9][0-9][0-9][0-9]-[0-9][0-9]-[0-9][0-9]%', Name), 10), 23)
                    WHERE PATINDEX('%[0-9][0-9][0-9][0-9]-[0-9][0-9]-[0-9][0-9]%', Name) > 0
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "NadeoCompetitions");
        }
    }
}
