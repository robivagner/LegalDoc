using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalDoc.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDocumentAnalysisFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaskType",
                table: "ReviewTasks");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ReviewTasks",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<string>(
                name: "Clauses",
                table: "LegalDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Risks",
                table: "LegalDocuments",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Clauses",
                table: "LegalDocuments");

            migrationBuilder.DropColumn(
                name: "Risks",
                table: "LegalDocuments");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ReviewTasks",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TaskType",
                table: "ReviewTasks",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
