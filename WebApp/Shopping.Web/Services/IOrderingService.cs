using Refit;
using Shopping.Web.Models.Order;

namespace Shopping.Web.Services;

public interface IOrderingService
{
    [Get("/ordering-service/orders")]
    Task<GetOrdersResponse> GetOrders([Query] int? pageIndex, [Query] int? pageSize);

    [Get("/ordering-service/orders/{orderName}")]
    Task<GetOrdersByNameResponse> GetOrdersByName(string orderName);

    [Get("/ordering-service/orders/customer/{customerId}")]
    Task<GetOrdersByCustomerResponse> GetOrdersByCustomer(Guid customerId);
}