namespace OrderingApplication.DTOs;
    public record OrderItemDto(
        Guid OrderId,
        Guid ProductId,
        decimal Price,
        int Quantity
        );
