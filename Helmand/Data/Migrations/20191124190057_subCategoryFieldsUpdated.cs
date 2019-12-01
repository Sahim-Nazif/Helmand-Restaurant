using Microsoft.EntityFrameworkCore.Migrations;

namespace Helmand.Data.Migrations
{
    public partial class subCategoryFieldsUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "subCName",
                table: "SubCategory",
                newName: "SubCName");

            migrationBuilder.RenameColumn(
                name: "subCId",
                table: "SubCategory",
                newName: "SubCId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubCName",
                table: "SubCategory",
                newName: "subCName");

            migrationBuilder.RenameColumn(
                name: "SubCId",
                table: "SubCategory",
                newName: "subCId");
        }
    }
}
