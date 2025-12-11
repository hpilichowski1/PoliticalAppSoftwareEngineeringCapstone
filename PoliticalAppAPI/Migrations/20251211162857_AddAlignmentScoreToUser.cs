using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PoliticalAppAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddAlignmentScoreToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "alignment_score",
                table: "users",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "alignment_score",
                table: "users");
        }
    }
}
