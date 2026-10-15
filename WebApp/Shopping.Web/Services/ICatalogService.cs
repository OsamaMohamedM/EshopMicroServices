using Refit;
using Shopping.Web.Models.Catalog;

namespace Shopping.Web.Services
{
    public interface ICatalogService
    {
        [Get("/catalog-service/products/?pageNumber={pageNumber}&pageSize={pageSize}")]
        Task<GetProductResponse> GetProducts(int pageNumber = 1, int pageSize = 5);

        [Get("/catalog-service/products/{id}")]
        Task<GetProductById> GetProductById(Guid id);

        [Get("/catalog-service/products/category/{category}")]
        Task<GetProductByCategory> GetProductsByCategory(string category);
    }
}