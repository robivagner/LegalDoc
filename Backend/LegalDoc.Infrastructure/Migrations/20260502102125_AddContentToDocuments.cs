using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalDoc.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContentToDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "LegalDocuments",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "LegalDocuments");
        }
    }
}
