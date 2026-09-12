using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shopping.Web.Models.Catalog;
using Shopping.Web.Services;

namespace Shopping.Web.Pages
{
    public class IndexModel(ILogger<IndexModel> logger, ICatalogService catalogService) : PageModel
    {
        public IEnumerable<ProductModel> ProductList { get; set; } = default!;

        public async Task<IActionResult> OnGet()
        {
            logger.LogInformation("Index page requested");
            var result = await catalogService.GetProducts();
            ProductList = result.Products;
            return Page();
        }
    }
}