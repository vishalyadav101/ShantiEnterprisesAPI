using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShantiEnterprises.API.Migrations
{
    /// <inheritdoc />
    public partial class AddWebsiteSettingHighlightText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HighlightText",
                table: "WebsiteSettings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HighlightText",
                table: "WebsiteSettings");
        }
    }
}
