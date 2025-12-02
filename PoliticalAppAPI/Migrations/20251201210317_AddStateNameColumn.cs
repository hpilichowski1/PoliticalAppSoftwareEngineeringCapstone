using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PoliticalAppAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddStateNameColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "state_name",
                table: "representatives",
                type: "varchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "state_name",
                table: "representatives");
        }
    }
}
