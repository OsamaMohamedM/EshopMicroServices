using Carter;
using Catalog.Api.Products.CreatProduct;
using Catalog.Api.Products.GetProduct;
using Catalog.Api.Products.GetProductById;
using Catalog.Api.Products.GetProductsByCategory;
using Catalog.Api.Products.UpdateProduct;
using Catalog.Api.Products.DeleteProductById;
using Marten;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCarter(configurator: c =>
{
    c.WithModule<CreateProductAPI>();
    c.WithModule<GetProductsAPi>();
    c.WithModule<GetProductByIdAPI>();
    c.WithModule<GetProductsByCategoryAPI>();
    c.WithModule<UpdateProductAPI>();
    c.WithModule<DeleteProductByIdAPI>();
});
builder.Services.AddMediatR(cofig =>
{
    cofig.RegisterServicesFromAssembly(typeof(Program).Assembly);
});
builder.Services.AddMarten(option =>
{
    option.Connection(builder.Configuration.GetConnectionString("CatalogConnection"));
}
).UseLightweightSessions();
var app = builder.Build();

app.MapCarter();
app.MapGet("/debug-routes", (IEnumerable<EndpointDataSource> endpointSources) =>
{
    var endpoints = endpointSources.SelectMany(es => es.Endpoints);
    return string.Join("\n", endpoints);
});

app.Run();