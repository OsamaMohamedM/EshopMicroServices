using OrderingApplication.Common.Pagination;

namespace OrderingApplication.Orders.Queries.GetOrders;

public record GetOrdersQuery(int PageIndex = 0, int PageSize = 10) : IQuery<GetOrdersResult>;

public record GetOrdersResult(PaginatedResult<OrderDto> Orders);
