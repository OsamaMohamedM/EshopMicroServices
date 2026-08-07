using Basket.API.Basket.DeleteBasket;
using Basket.API.Basket.GetBasket;
using Basket.API.Basket.StoreBasket;
using Basket.API.Data.Models;
using Basket.API.Data.Repositories;
using Basket.API.Data.Repositries;
using BuildingBlocks.Behaviors.LoggingBehavior;
using BuildingBlocks.Behaviors.ValidationBehavior;
using BuildingBlocks.Exceptions.Handler;
using BuildingBlocksMessageing.MassTransit;
using Carter;
using Discount.Grpc.Protos;
using Marten;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCarter(configurator: c =>
{
    c.WithModule<GetBasketApi>();
    c.WithModule<StoreBasketApi>();
    c.WithModule<DeleteBasketApi>();
});
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddOpenBehavior(typeof(LoggingBehaviorr<,>));
    cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
});
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddMessageBroker(builder.Configuration);

builder.Services.AddMarten(opt =>
{
    opt.Connection(builder.Configuration.GetConnectionString("BasketDatabase")!);
    opt.Schema.For<ShoppingCart>().Identity(x => x.UserName);
}).UseLightweightSessions();

builder.Services.AddStackExchangeRedisCache(cogfig =>
{
    cogfig.Configuration = builder.Configuration.GetConnectionString("Redis");
});
var discountGrpcClient = builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(opt =>
{
    opt.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]!);
});

if (builder.Environment.IsDevelopment())
{
    discountGrpcClient.ConfigurePrimaryHttpMessageHandler(() =>
    {
        return new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
    });
}

builder.Services.AddExceptionHandler<CustomExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.Decorate<IBasketRepository, CacheBasketRepository>();
var app = builder.Build();
app.MapCarter();
app.UseExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI();
app.Run();