using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVCApplicationCore.Migrations
{
    public partial class AddGetCategoryProductCountStoredProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder != null)
            {
                migrationBuilder.Sql(@"
                                        CREATE PROCEDURE GetCategoryProductCount
                                        AS
                                        BEGIN
                                            SELECT 
                                                c.CategoryId,
                                                c.Name,
                                                COUNT(p.ProductId) AS ProductCount
                                            FROM 
                                                Categories c
                                            LEFT JOIN 
                                                Products p ON c.CategoryId = p.CategoryId
                                            GROUP BY 
                                                c.CategoryId, c.Name
                                            ORDER BY 
                                                c.CategoryId;
                                        END");
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROCEDURE GetCategoryProductCount");
        }
    }
}
