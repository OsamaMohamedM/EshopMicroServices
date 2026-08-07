using Microsoft.EntityFrameworkCore;
using OrderingApplication.Orders.Extensions;

namespace OrderingApplication.Orders.Queries.GetOrdersByName;

public class GetOrdersByNameHandler(IApplicationDbContext dbContext) : IQueryHandler<GetOrdersByNameQuery, GetOrdersByNameResult>
{
    public async Task<GetOrdersByNameResult> Handle(GetOrdersByNameQuery query, CancellationToken cancellationToken)
    {
        var orders = await dbContext.Orders
            .AsNoTracking()
            .Where(o => o.OrderName.Value == query.OrderName)
            .OrderBy(o => o.OrderName.Value)
            .ProjectToOrderDto()
            .ToListAsync(cancellationToken);

        return new GetOrdersByNameResult(orders);
    }
}
