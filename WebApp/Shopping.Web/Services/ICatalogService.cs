using Refit;
using Shopping.Web.Models.Catalog;

namespace Shopping.Web.Services
{
    public interface ICatalogService
    {
        [Get("/catalog-service/products/?pageNumber={pageNumber}&pageSize={pageSize}")]
        Task<GetProductResponse> GetProducts();

        [Get("/catalog-service/products/{id}")]
        Task<GetProductById> GetProductById(int id);

        [Get("/catalog-service/products/category/{category}")]
        Task<IEnumerable<GetProductByCategory>> GetProductsByCategory(string category);
    }
}