namespace Shopping.Web.Models.Catalog
{
    public class ProductModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default;
        public string Description { get; set; }
        public List<string> Category { get; set; } = new();
        public string ImageFile { get; set; } = default;
        public decimal Price { get; set; }
    }

    public record GetProductResponse(IEnumerable<ProductModel> Products);
    public record GetProductByCategory(IEnumerable<ProductModel> Products);
    public record GetProductById(ProductModel Product);
}