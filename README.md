# EshopMicroServices

## Overview

EshopMicroServices is a .NET 10 e-commerce solution built as independently deployable services. It demonstrates Clean Architecture, CQRS with MediatR, Carter Minimal APIs, Marten, Entity Framework Core, gRPC, RabbitMQ/MassTransit, Redis, YARP, Refit, Razor Pages, and Docker Compose.

> This README documents the implementation currently present in the repository.

## Architecture

```text
                         +----------------------+
                         | Shopping.Web         |
                         | Razor Pages + Refit  |
                         +----------+-----------+
                                    |
                                    v
                         +----------------------+
                         | YARP Gateway         |
                         | Routing + rate limit |
                         +----+---------+-------+
                              |         |
                 /catalog-service  /basket-service  /ordering-service
                              |         |                 |
                              v         v                 v
                         Catalog     Basket           Ordering
                           API        API               API
                              |       / | \               |
                              v      /  |  \              v
                         PostgreSQL Redis gRPC       SQL Server
                                      |    |
                                      |    v
                                      | Discount.Grpc
                                      |    |
                                      | SQLite
                                      v
                                   RabbitMQ
                                      |
                                      v
                                   Ordering
```

### Communication paths

```text
Shopping.Web --Refit/HTTP--> YARP --HTTP--> Catalog, Basket, Ordering
Basket --unary gRPC/HTTP2--> Discount.Grpc
Basket --MassTransit/RabbitMQ event--> Ordering
```

The services own separate data stores and communicate through HTTP contracts, protobuf contracts, or integration events rather than sharing databases.

## Repository structure

```text
BuildingBlocks/                 Shared CQRS, behaviors, exceptions
BuildingBlocksMessageing/       MassTransit and shared events
Services/Catalog/Catalog.Api/   Product HTTP API
Services/Basket/Basket.API/     Basket HTTP API
Services/Discount/Discount.Grpc/Discount gRPC server
Services/Ordering/
  Ordering.Api/                 Ordering host and HTTP endpoints
  OrderingApplication/          Commands, queries, consumers, DTOs
  OrderingDomain/               Order domain model
  OrderingInfastructure/        EF Core, mappings, migrations, interceptors
APIGetWays/YARPGetWay/          Reverse proxy gateway
WebApp/Shopping.Web/            Razor Pages storefront
docker-compose.yml              Application and infrastructure containers
```

All application projects currently target `net10.0`.

## Components

| Component | Responsibility | Technology |
|---|---|---|
| `Shopping.Web` | Storefront UI and API client | Razor Pages, Refit |
| `YARPGetWay` | Public gateway and rate limiting | YARP, fixed-window limiter |
| `Catalog.Api` | Product CRUD and category queries | Carter, MediatR, Marten/PostgreSQL |
| `Basket.API` | Basket CRUD, discount calculation, checkout integration | Carter, MediatR, Marten, Redis |
| `Discount.Grpc` | Coupon CRUD and discount lookup | gRPC, EF Core, SQLite |
| `Ordering.Api` | Ordering HTTP host | Carter, Swagger |
| `OrderingApplication` | Ordering use cases and event consumers | MediatR, FluentValidation, MassTransit |
| `OrderingDomain` | Order aggregate and domain rules | Domain model |
| `OrderingInfastructure` | Persistence and infrastructure wiring | EF Core, SQL Server |

## Request flows

### Web request

```text
Browser
  -> Shopping.Web Razor PageModel
  -> Refit client
  -> YARP route
  -> Catalog/Basket/Ordering endpoint
  -> MediatR handler or application use case
  -> Database/cache/external service
  -> Response back through the same path
```

### Store basket and apply discounts

```text
StoreBasketApi
  -> StoreBasketHandler
  -> for each cart item:
       DiscountProtoServiceClient.GetDiscountAsync()
       -> HTTP/2 + protobuf
       -> DiscountService.GetDiscount()
       -> DiscountContext -> SQLite
       -> GetDiscountResponse
       -> item.Price -= Coupon.Amount
  -> IBasketRepository
  -> CacheBasketRepository -> BasketRepository
  -> Marten -> PostgreSQL
```

### Checkout event

```text
Basket checkout
  -> RabbitMQ through MassTransit
  -> BasketCheckOutEventHandler
  -> CreatOrderCommand
  -> MediatR
  -> CreateOrderHandler
  -> Order aggregate
  -> IApplicationDbContext
  -> EF Core -> SQL Server
```

## Catalog API

`Services/Catalog/Catalog.Api/Program.cs` registers Carter, MediatR, FluentValidation, Marten, Swagger, and shared exception handling. Product endpoints are organized in separate folders:

| Endpoint | Folder | Behavior |
|---|---|---|
| `POST /products` | `Products/CreatProduct` | Maps request to command and creates a product |
| `GET /products` | `Products/GetProducts` | Returns projected products |
| `GET /products/{id}` | `Products/GetProductById` | Gets one product |
| `GET /products/category/{category}` | `Products/GetProductsByCategory` | Filters by category |
| `PUT /products` | `Products/UpdateProduct` | Updates a product |
| `DELETE /products/{id}` | `Products/DeleteProductById` | Deletes a product |

Typical endpoint flow:

```text
Carter module -> ISender.Send() -> MediatR handler
              -> logging/validation behavior
              -> Marten IDocumentSession
              -> Catalog PostgreSQL
```

Mapster converts HTTP request/response records. Read handlers use LINQ projections into DTOs.

## Basket API

`Basket.API/Program.cs` registers Carter modules, MediatR behaviors, Swagger, MassTransit, Marten, Redis, the generated Discount gRPC client, exception handling, and the repository decorator.

`StoreBasketHandler` receives `IBasketRepository` and `DiscountProtoService.DiscountProtoServiceClient`. It calls the Discount service for each `ShoppingCartItem`, subtracts the returned coupon amount, then persists the basket.

```text
StoreBasketHandler
   |
   +--> DiscountProtoServiceClient.GetDiscountAsync(product name)
   |
   +--> IBasketRepository.StoreShoppingCartAsync(cart)
             |
             +--> CacheBasketRepository decorator
                         |
                         +--> BasketRepository
                                      |
                                      +--> Marten IDocumentSession
```

Marten identifies `ShoppingCart` documents by `UserName`. Redis is configured as the distributed cache.

## Discount gRPC

The contract is `Services/Discount/Discount.Grpc/Protos/discount.proto`:

```protobuf
syntax = "proto3";
option csharp_namespace = "Discount.Grpc.Protos";
package discount;

service DiscountProtoService {
  rpc GetDiscount(GetDiscountRequest) returns (GetDiscountResponse);
  rpc CreateDiscount(CreateDiscountRequest) returns (CreateDiscountResponse);
  rpc DeleteDiscount(DeleteDiscountRequest) returns (DeleteDiscountResponse);
  rpc UpdateDiscount(UpdateDiscountRequest) returns (UpdateDiscountResponse);
}
```

All four RPCs are unary. Messages use `CouponModel` with `id`, `productName`, `description`, and `amount`.

The Discount project includes the contract with `GrpcServices="Server"`; Basket links the same file with `GrpcServices="Client"`. Build-generated code provides:

- `DiscountProtoServiceClient`, used by Basket.
- `DiscountProtoServiceBase`, inherited by `DiscountService`.
- Protobuf message classes and serialization/marshalling code.

Do not edit generated files under `obj`; edit the `.proto` file and rebuild.

### gRPC flow

```text
StoreBasketHandler
  -> DiscountProtoServiceClient.GetDiscountAsync()
  -> protobuf serialization
  -> HTTP/2 request to DiscountUrl
  -> generated server routing
  -> DiscountService.GetDiscount()
  -> DiscountContext -> SQLite
  -> Coupon -> CouponModel using Mapster
  -> protobuf response
  -> Basket subtracts Coupon.Amount
```

`Discount.Grpc/Program.cs` calls `AddGrpc()`, registers `DiscountContext` with SQLite, maps `DiscountService` using `MapGrpcService<DiscountService>()`, and runs migrations through `UseMigration()`.

`DiscountService.GetDiscount` searches by `ProductName`. If no coupon exists, it returns a default coupon with amount `0`. Update and delete throw `RpcException` with `StatusCode.NotFound` when the coupon does not exist.

Basket registers the client with `AddGrpcClient` using `GrpcSettings:DiscountUrl`. Development configuration accepts local certificates through `DangerousAcceptAnyServerCertificateValidator`; this should not be used as a production certificate policy.

## Ordering

Ordering is split into API, Application, Domain, and Infrastructure projects:

```text
Ordering.Api
  -> OrderingApplication
       -> OrderingDomain
       -> IApplicationDbContext
  -> OrderingInfastructure
       -> ApplicationDbContext
       -> SQL Server
```

`OrderingApplication` registers MediatR handlers, validation, logging, feature management, and MassTransit consumers. `CreateOrderHandler` creates the aggregate, adds order items, saves through `IApplicationDbContext`, and returns the order ID.

`BasketCheckOutEventHandler` consumes `BasketCheckOutEvent`, maps customer/address/payment/item information to `OrderDto`, creates `CreatOrderCommand`, and sends it through MediatR. Infrastructure registers SQL Server EF Core, entity configurations, migrations, audit interception, and domain-event dispatch interception.

## Gateway and web application

YARP routes configured in `APIGetWays/YARPGetWay/appsettings.json`:

| Prefix | Container destination |
|---|---|
| `/catalog-service/{**catch-all}` | `http://catalog.api:8080` |
| `/basket-service/{**catch-all}` | `http://basket.api:8080` |
| `/ordering-service/{**catch-all}` | `http://ordering.api:8080` |

The route transform removes the service prefix. `/catalog-service/products` is forwarded as `/products`. Every route uses a fixed-window policy permitting five requests per ten seconds.

`Shopping.Web` is a Razor Pages application. It registers `ICatalogService`, `IBasketService`, and `IOrderingService` through Refit. Their base address is `ApiSettings:GatewayBaseUri`, currently `https://localhost:6064`.

## Shared building blocks

`BuildingBlocks/CQRS` provides MediatR-based `ICommand<TResponse>` and `IQuery<TResponse>` abstractions. The shared logging behavior logs requests, responses, and elapsed time. The validation behavior executes registered FluentValidation validators and throws `ValidationException` when failures exist. Shared exception handlers produce Problem Details responses.

`BuildingBlocksMessageing/MassTransit/Extensions.cs` configures MassTransit with kebab-case endpoint names, RabbitMQ credentials from configuration, optional consumer assembly scanning, and automatic endpoint configuration.

## GitHub repository description

Use this short description in the GitHub repository settings:

> A .NET 10 e-commerce microservices platform using Clean Architecture, CQRS, gRPC, RabbitMQ, Marten, Redis, YARP, and Razor Pages.

Suggested GitHub topics:

```text
.net, dotnet, microservices, clean-architecture, cqrs, mediatR, grpc,
rabbitmq, masstransit, marten, postgresql, entity-framework-core, redis,
yarp, razor-pages, docker, ecommerce
```

## API usage examples

The gateway is the preferred public entry point. Internal service URLs are used when running individual projects locally.

### Catalog examples

Create a product:

```http
POST /catalog-service/products
Content-Type: application/json

{
  "name": "IPhone X",
  "description": "A sample product",
  "price": 500,
  "imageFile": "iphone-x.png",
  "category": ["Phones", "Apple"]
}
```

Get all products:

```http
GET /catalog-service/products
```

Get one product:

```http
GET /catalog-service/products/{id}
```

Get products by category:

```http
GET /catalog-service/products/category/Phones
```

Update a product:

```http
PUT /catalog-service/products
Content-Type: application/json

{
  "id": "00000000-0000-0000-0000-000000000000",
  "name": "IPhone X",
  "description": "Updated description",
  "price": 450,
  "imageFile": "iphone-x.png",
  "category": ["Phones", "Apple"]
}
```

Delete a product:

```http
DELETE /catalog-service/products/{id}
```

### Basket examples

The Basket API is exposed through its Carter modules. The exact request body is defined by the basket request records in `Services/Basket/Basket.API/Basket`.

```http
GET /basket-service/basket/{userName}
```

```http
POST /basket-service/basket
Content-Type: application/json

{
  "userName": "customer-1",
  "items": [
    {
      "quantity": 1,
      "price": 500,
      "productName": "IPhone X",
      "productId": "00000000-0000-0000-0000-000000000000"
    }
  ]
}
```

When the basket is stored, Basket calls Discount for each item before saving the final price.

### Discount gRPC examples

The Discount service is intended to be called with a gRPC client generated from `discount.proto`, not with a normal browser GET request.

```csharp
var response = await discountClient.GetDiscountAsync(
    new GetDiscountRequest { ProductName = "IPhone X" });

var amount = response.Coupon.Amount;
```

Available RPCs:

| RPC | Request | Response | Behavior |
|---|---|---|---|
| `GetDiscount` | `GetDiscountRequest` | `GetDiscountResponse` | Finds a coupon by product name |
| `CreateDiscount` | `CreateDiscountRequest` | `CreateDiscountResponse` | Creates a coupon |
| `UpdateDiscount` | `UpdateDiscountRequest` | `UpdateDiscountResponse` | Updates a coupon by ID |
| `DeleteDiscount` | `DeleteDiscountRequest` | `DeleteDiscountResponse` | Deletes a coupon by ID |

## Contract and data ownership

Each service owns its own model and storage. The same business concept can therefore have different representations at different boundaries:

```text
Catalog Product entity
        |
        +--> Catalog API DTOs

Basket ShoppingCart entity
        |
        +--> Basket HTTP request/response models

Discount Coupon entity
        |
        +--> protobuf CouponModel

Ordering Order aggregate
        |
        +--> Ordering DTOs and integration event data
```

This separation prevents one service’s persistence model from becoming a shared dependency for every other service.

## Runtime modes

### Local process mode

When projects are started directly from Visual Studio or the .NET CLI:

```text
Shopping.Web       -> https://localhost:6064
YARP Gateway       -> local gateway profile
Catalog/Basket     -> localhost database ports
Discount           -> http://localhost:5002 or https://localhost:5052
Ordering           -> localhost SQL Server
RabbitMQ/Redis     -> localhost ports
```

### Docker Compose mode

Inside the Compose network, services communicate by container DNS names:

```text
catalog.api:8080
basket.api:8080
ordering.api:8080
messagebroker:5672
distributed_cache:6379
postgres_catalog
postgres_basket
sqlserver_ordering
```

Do not use `localhost` to connect from one container to another. Inside a container, `localhost` means that same container.

## Observability and diagnostics

The current implementation uses ASP.NET Core logging and the shared MediatR logging behavior. Important diagnostic points are:

- `LoggingBehaviorr<TRequest,TResponse>` logs request names, request content, responses, and elapsed time.
- `DiscountService` logs coupon creation, updates, deletes, and missing discount lookups.
- `BasketCheckOutEventHandler` logs consumed checkout event IDs.
- ASP.NET Core logs are configured through each service’s `appsettings.json`.
- Catalog, Basket, and Ordering expose Swagger services in their startup configuration.
- Catalog includes a `/debug-routes` endpoint that lists registered endpoint routes.

For production, add correlation IDs, structured logging sinks, distributed tracing, health checks, metrics, and centralized log storage.

## Validation and request lifecycle

A typical command lifecycle is:

```text
HTTP request
  -> Carter binds JSON to request DTO
  -> Mapster creates command
  -> ISender.Send(command)
  -> MediatR pipeline behaviors
       -> logging
       -> validation where registered
  -> handler
  -> repository/database/external service
  -> result mapping
  -> HTTP response
```

A validation failure becomes a shared exception-handling response instead of allowing invalid data to reach the handler.

## Testing guidance

The repository’s runtime architecture is suitable for several test levels:

### Unit tests

Test domain objects, handlers, validators, and mapping logic without starting containers. Mock or fake:

- `IBasketRepository`.
- `DiscountProtoServiceClient` or an abstraction around it.
- `IApplicationDbContext`.
- Logging dependencies where needed.

### Integration tests

Run the relevant infrastructure and test:

- Marten/PostgreSQL persistence for Catalog and Basket.
- EF Core/SQLite behavior for Discount.
- EF Core/SQL Server behavior for Ordering.
- RabbitMQ consumer delivery.
- gRPC request/response behavior.

### End-to-end tests

Start the gateway and web application, then verify:

```text
Razor Page -> Refit -> YARP -> service -> persistence
```

No test project was found during the current repository inspection, so tests should be added before treating the sample as production-ready.

## Troubleshooting

### Basket cannot reach Discount

1. Confirm `Discount.Grpc` is running.
2. Confirm Basket’s `GrpcSettings:DiscountUrl` matches the active Discount profile.
3. Confirm the protocol is HTTP/2.
4. Check whether the client is using `localhost` or the Compose service name for the selected runtime mode.
5. Review Discount server logs.

### Gateway returns connection errors

1. Confirm Catalog, Basket, or Ordering is running.
2. In Docker, verify the destination container name and port.
3. Check the YARP route prefix and path transform.
4. Check the gateway rate limiter if requests are being rejected.

### Database connection failures

1. Confirm the relevant database container is healthy.
2. Verify port mappings and credentials.
3. Use localhost addresses for host processes and container names for Docker processes.
4. Check migration/initialization logs.

### Empty or incorrect discount values

1. Confirm `ProductName` matches the seeded coupon name.
2. Check the SQLite `discountDb` file used by the Discount service.
3. Verify the `Coupon` to `CouponModel` Mapster mapping.
4. Remember that missing coupons intentionally return amount `0` from `GetDiscount`.

## Security and production hardening

The repository is a learning-oriented implementation and requires additional hardening for production:

- Move passwords and connection strings to a secret manager.
- Remove development certificate bypasses.
- Add authentication and authorization at the gateway and service boundaries.
- Add gRPC metadata/token propagation where required.
- Add request deadlines, cancellation propagation, retries, and circuit breakers for remote calls.
- Add idempotency for integration events and important write operations.
- Validate message contracts and protect RabbitMQ management endpoints.
- Add health checks and readiness probes.
- Restrict database network access.
- Add TLS between services where required.
- Add rate limits appropriate for each endpoint rather than relying only on the current fixed-window gateway policy.

## Suggested roadmap

Possible next improvements for the project:

1. Add automated tests for each service and the complete checkout flow.
2. Add health checks for databases, Redis, RabbitMQ, and gRPC dependencies.
3. Add a shared contracts project for versioned integration events.
4. Introduce an abstraction around the Discount gRPC client for easier testing.
5. Add gRPC deadlines and structured `RpcException` handling in Basket.
6. Add authentication and authorization.
7. Add OpenTelemetry tracing and metrics.
8. Add CI validation for build, tests, formatting, and container builds.
9. Fix naming inconsistencies such as `CreatProduct`, `GetProductsAPi`, and `OrderingInfastructure`.
10. Separate Docker and local configuration more explicitly.

## Persistence and configuration

| Service | Store | Configuration |
|---|---|---|
| Catalog | PostgreSQL/Marten | `ConnectionStrings:CatalogConnection` |
| Basket | PostgreSQL/Marten | `ConnectionStrings:BasketDatabase` |
| Basket cache | Redis | `ConnectionStrings:Redis` |
| Discount | SQLite/EF Core | `ConnectionStrings:Database` |
| Ordering | SQL Server/EF Core | `ConnectionStrings:DefaultConnection` |
| Messaging | RabbitMQ | `MessageBroker:*` |

Important development values currently in configuration:

```text
Gateway:       https://localhost:6064
Discount gRPC: http://localhost:5002 (HTTPS profile: 5052)
Catalog DB:    localhost:5434
Basket DB:     localhost:5435
Redis:         localhost:6379
RabbitMQ:      localhost:5672
SQL Server:    localhost,1433
```

Do not use checked-in development credentials in production. Use environment variables, user secrets, or a secret manager.

## Docker Compose

`docker-compose.yml` defines application services:

```text
catalog.api, basket.api, discount.grpc, ordering.api, yarp.gateway
```

and infrastructure services:

```text
postgres_catalog, postgres_basket, sqlserver_ordering,
distributed_cache, messagebroker
```

```text
                 yarp.gateway
                /      |       \
        catalog.api  basket.api  ordering.api
             |        / |  \          |
        Postgres    PG Redis gRPC   SQL Server
                              |        |
                           Discount  RabbitMQ
                              |
                            SQLite
```

Compose declares health/start dependencies: Catalog waits for PostgreSQL; Basket waits for PostgreSQL, Redis, RabbitMQ, and Discount; Ordering waits for SQL Server and RabbitMQ; Gateway waits for the HTTP services.

## Running the project

### Prerequisites

- .NET 10 SDK.
- Docker Desktop with Compose support.
- A trusted HTTPS development certificate when using HTTPS profiles.

### Build

```powershell
dotnet build
```

### Run all containers

```powershell
docker compose up --build
docker compose up --build -d
docker compose down
docker compose down -v # also removes persisted volumes
```

For Visual Studio development, start Discount before Basket, then Catalog, Basket, Ordering, Gateway, and Shopping.Web. When running in Docker, use Compose hostnames; when running locally, use localhost values.

## Error handling, security, and streaming

- Catalog, Basket, and Ordering use shared exception handling and Problem Details.
- FluentValidation is registered in Catalog and Ordering; Basket registers the shared validation behavior.
- Discount uses `RpcException`/`StatusCode.NotFound` for missing update/delete targets.
- Basket currently does not catch `RpcException` around discount calls; production code should define fallback, timeout, and retry behavior.
- No JWT authentication, gRPC metadata authentication, authorization policy, or gRPC interceptors were found in the inspected implementation. Razor Pages calls `UseAuthorization`, but no authentication setup was verified.
- The gRPC contract uses unary RPCs only. No client, server, or bidirectional streaming exists.

## Development conventions

### Add a Catalog endpoint

1. Create a feature folder under `Catalog.Api/Products`.
2. Add request/response and command/query records.
3. Add a MediatR handler.
4. Add a Carter module.
5. Add validation and Mapster mappings where needed.
6. Register the module in `Program.cs`.
7. Build and test.

### Add a gRPC operation

1. Change `discount.proto`.
2. Rebuild to regenerate client, base, and message types.
3. Override the operation in `DiscountService`.
4. Add the client call in the consuming service.
5. Define error and cancellation behavior.
6. Never edit generated files directly.

### Add an Ordering use case

Keep commands and handlers in `OrderingApplication`, business rules in `OrderingDomain`, persistence abstractions in the application layer, and EF/database implementation in `OrderingInfastructure`.

## Known repository notes

- Docker service URLs and localhost development URLs are both present; use the set matching the execution mode.
- Compose references `APIGETWays/YarpGateway/Dockerfile`, while the visible project folder is `APIGetWays/YARPGetWay`; verify this path before running Compose on a case-sensitive filesystem.
- Catalog currently registers `AddValidatorsFromAssembly` twice.
- Existing names include `CreatProduct`, `GetProductsAPi`, `OrderingInfastructure`, and `DependanceInjection`; these are documented as-is for navigation.
- Generated files and local database artifacts may appear under project output directories and should normally be regenerated or excluded from source control.

## Technology summary

| Area | Technology |
|---|---|
| Runtime | .NET 10 |
| UI | ASP.NET Core Razor Pages |
| Gateway | YARP |
| HTTP APIs | ASP.NET Core Minimal APIs + Carter |
| CQRS | MediatR |
| Validation | FluentValidation |
| Mapping | Mapster |
| Documents | Marten + PostgreSQL |
| Relational data | EF Core + SQL Server/SQLite |
| RPC | gRPC + Protocol Buffers + HTTP/2 |
| Events | MassTransit + RabbitMQ |
| Cache | Redis |
| HTTP clients | Refit |
| Containers | Docker Compose |

## Final mental model

```text
Contracts (.proto and HTTP DTOs)
        |
        v
Generated clients/base classes + handwritten endpoints/services
        |
        v
Shopping.Web -> Refit -> YARP -> Catalog/Basket/Ordering
                                      |
                                      v
                              Carter + MediatR
                                      |
                 +--------------------+--------------------+
                 |                                         |
                 v                                         v
        Marten / gRPC / Redis                     Domain + EF Core
                 |                                         |
                 v                                         v
          PostgreSQL / SQLite                   SQL Server / RabbitMQ
```

The essential summary is: the system uses synchronous HTTP for web-facing APIs, unary gRPC for Basket-to-Discount calls, and asynchronous RabbitMQ events for checkout-to-order integration. Each service owns its persistence, while shared building blocks standardize CQRS, validation, logging, exception handling, and messaging.