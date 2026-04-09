using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AffiliateBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddArticleLanguage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "Articles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "TranslationGroupId",
                table: "Articles",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Language",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "TranslationGroupId",
                table: "Articles");
        }
    }
}
