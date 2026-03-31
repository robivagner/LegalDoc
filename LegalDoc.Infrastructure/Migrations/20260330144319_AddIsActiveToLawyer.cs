using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalDoc.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveToLawyer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Lawyers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Lawyers");
        }
    }
}
