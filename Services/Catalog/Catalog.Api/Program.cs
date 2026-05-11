using BuildingBlocks.Behaviors.LoggingBehavior;
using BuildingBlocks.Behaviors.ValidationBehavior;
using BuildingBlocks.Exceptions.Handler;
using Carter;
using Catalog.Api.Data;
using Catalog.Api.Products.CreatProduct;
using Catalog.Api.Products.DeleteProductById;
using Catalog.Api.Products.GetProduct;
using Catalog.Api.Products.GetProductById;
using Catalog.Api.Products.GetProductsByCategory;
using Catalog.Api.Products.UpdateProduct;
using FluentValidation;
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
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddMediatR(cofig =>
{
    cofig.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cofig.AddOpenBehavior(typeof(ValidatorBehavior<,>));
    cofig.AddOpenBehavior(typeof(ILoggingBehaviorr<,>));
});

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddMarten(option =>
{
    option.Connection(builder.Configuration.GetConnectionString("CatalogConnection"));
}
).UseLightweightSessions();
builder.Services.AddExceptionHandler<CustomExceptionHandler>();
builder.Services.AddProblemDetails();

if (builder.Environment.IsDevelopment())
    builder.Services.InitializeMartenWith<CatalogInitialData>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName);
});
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapCarter();
app.UseExceptionHandler();
app.MapGet("/debug-routes", (IEnumerable<EndpointDataSource> endpointSources) =>
{
    var endpoints = endpointSources.SelectMany(es => es.Endpoints);
    return string.Join("\n", endpoints);
});

app.Run();