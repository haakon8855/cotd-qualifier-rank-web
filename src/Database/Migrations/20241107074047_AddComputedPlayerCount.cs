using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CotdQualifierRank.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddComputedPlayerCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlayerCount",
                table: "Competitions",
                type: "int",
                nullable: false,
                defaultValue: 0);
            
            // Set PlayerCount to correct value based on existing data in Records table
            migrationBuilder.Sql(@"
                UPDATE Competitions
                SET PlayerCount = (
                    SELECT COUNT(*)
                    FROM Records
                    WHERE Records.CompetitionId = Competitions.Id
                )
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlayerCount",
                table: "Competitions");
        }
    }
}
