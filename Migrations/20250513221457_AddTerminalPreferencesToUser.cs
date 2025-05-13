using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimplySH.Migrations
{
    /// <inheritdoc />
    public partial class AddTerminalPreferencesToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TerminalColor",
                table: "AspNetUsers",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "TerminalFontSize",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TerminalColor",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TerminalFontSize",
                table: "AspNetUsers");
        }
    }
}
