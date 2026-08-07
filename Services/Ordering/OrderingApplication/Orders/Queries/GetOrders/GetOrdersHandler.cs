using Microsoft.EntityFrameworkCore;
using OrderingApplication.Common.Pagination;
using OrderingApplication.Orders.Extensions;

namespace OrderingApplication.Orders.Queries.GetOrders;

public class GetOrdersHandler(IApplicationDbContext dbContext) : IQueryHandler<GetOrdersQuery, GetOrdersResult>
{
    public async Task<GetOrdersResult> Handle(GetOrdersQuery query, CancellationToken cancellationToken)
    {
        var paginationRequest = new PaginationRequest(query.PageIndex, query.PageSize);
        var ordersQuery = dbContext.Orders
            .AsNoTracking()
            .OrderBy(o => o.OrderName.Value);

        var count = await ordersQuery.LongCountAsync(cancellationToken);
        var orders = await ordersQuery
            .Skip(paginationRequest.PageIndex * paginationRequest.PageSize)
            .Take(paginationRequest.PageSize)
            .ProjectToOrderDto()
            .ToListAsync(cancellationToken);

        return new GetOrdersResult(new PaginatedResult<OrderDto>(
            paginationRequest.PageIndex,
            paginationRequest.PageSize,
            count,
            orders));
    }
}
