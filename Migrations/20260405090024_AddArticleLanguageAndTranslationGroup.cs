using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AffiliateBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddArticleLanguageAndTranslationGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Articles_Language",
                table: "Articles",
                column: "Language");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_TranslationGroupId",
                table: "Articles",
                column: "TranslationGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Articles_Language",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_TranslationGroupId",
                table: "Articles");
        }
    }
}
