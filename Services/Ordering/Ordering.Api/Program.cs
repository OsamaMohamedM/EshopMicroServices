using Ordering.Api;
using OrderingApplication;
using OrderingInfastructure;
using OrderingInfastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOrderingApplication(builder.Configuration)
      .AddOrderingApiServices()
      .AddOrderingInfastructure(builder.Configuration);

var app = builder.Build();
app.UseApiServices();
await app.MigrateAndSeedDatabaseAsync();

app.Run();