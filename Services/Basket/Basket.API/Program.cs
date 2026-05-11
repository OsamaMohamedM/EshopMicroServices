using Basket.API.Data.Models;
using Basket.API.Data.Repositories;
using BuildingBlocks.Behaviors.LoggingBehavior;
using BuildingBlocks.Behaviors.ValidationBehavior;
using BuildingBlocks.Exceptions.Handler;
using Carter;
using Marten;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCarter();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddOpenBehavior(typeof(LoggingBehaviorr<,>));
    cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
});

builder.Services.AddMarten(opt =>
{
    opt.Connection(builder.Configuration.GetConnectionString("BasketDateBase")!);
    opt.Schema.For<ShoppingCart>().Identity(x => x.UserName);
}).UseLightweightSessions();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddScoped<IBasketRepository, BasketRepository>();
var app = builder.Build();
app.MapCarter();
app.UseExceptionHandler();
app.Run();