using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiApplicationCore.Migrations
{
    public partial class SeedCategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "Name", "Description", "FileName" },
                values: new object[,]
                {
                    { 1, "Category 1", "Description 1", ""},
                    { 2, "Category 2", "Description 2", ""},
                    { 3, "Category 3", "Description 3", ""},
                    { 4, "Category 4", "Description 4", ""},
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValues: new object[] { 1,2,3,4});
        }
    }
}
