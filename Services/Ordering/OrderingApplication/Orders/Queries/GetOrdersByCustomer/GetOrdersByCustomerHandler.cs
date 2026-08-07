using Microsoft.EntityFrameworkCore;
using OrderingApplication.Orders.Extensions;

namespace OrderingApplication.Orders.Queries.GetOrdersByCustomer;

public class GetOrdersByCustomerHandler(IApplicationDbContext dbContext) : IQueryHandler<GetOrdersByCustomerQuery, GetOrdersByCustomerResult>
{
    public async Task<GetOrdersByCustomerResult> Handle(GetOrdersByCustomerQuery query, CancellationToken cancellationToken)
    {
        var customerId = CustomerId.Create(query.CustomerId);
        var orders = await dbContext.Orders
            .AsNoTracking()
            .Where(o => o.CustomerId == customerId)
            .OrderBy(o => o.OrderName.Value)
            .ProjectToOrderDto()
            .ToListAsync(cancellationToken);

        return new GetOrdersByCustomerResult(orders);
    }
}
