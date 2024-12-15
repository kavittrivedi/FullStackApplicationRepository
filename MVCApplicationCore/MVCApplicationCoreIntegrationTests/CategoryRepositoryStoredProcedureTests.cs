using MVCApplicationCoreIntegrationTests.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCApplicationCoreIntegrationTests
{
    public class CategoryRepositoryStoredProcedureTests: IClassFixture<AppDbContextFixture>
    {
        private readonly AppDbContextFixture _fixture;

        public CategoryRepositoryStoredProcedureTests(AppDbContextFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void GetCategoryProductCounts_ReturnsCorrectData()
        {
            // Arrange
            var context = _fixture._context;

            // Act
            var result = context.GetCategoryProductCounts();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.Name  == "Category 1" && c.ProductCount == 2);
            Assert.Contains(result, c => c.Name == "Category 2" && c.ProductCount == 1);
        }
    }
}
