using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAM_Upload.Migrations
{
    /// <inheritdoc />
    public partial class megeee2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ThumpnailUrl",
                table: "Files",
                newName: "Path");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Path",
                table: "Files",
                newName: "ThumpnailUrl");
        }
    }
}
