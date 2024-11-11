using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CotdQualifierRank.Database.Migrations
{
    /// <inheritdoc />
    public partial class MakeLiveIdPascalCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "liveId",
                table: "NadeoCompetitions",
                newName: "LiveId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LiveId",
                table: "NadeoCompetitions",
                newName: "liveId");
        }
    }
}
