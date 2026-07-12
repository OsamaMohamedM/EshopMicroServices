using Ordering.Api;
using OrderingApplication;
using OrderingInfastructure;
using OrderingInfastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOrderingApplication()
      .AddOrderingApiServices()
      .AddOrderingInfastructure(builder.Configuration);

var app = builder.Build();
app.AddOrderingApiUses();
await app.MigrateAndSeedDatabaseAsync();

app.MapGet("/", () => "Hello World!");

app.Run();