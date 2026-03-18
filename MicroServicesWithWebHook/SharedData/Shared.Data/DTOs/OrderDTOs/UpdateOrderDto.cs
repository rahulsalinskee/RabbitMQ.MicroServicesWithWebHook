namespace Shared.Data.DTOs.OrderDTOs
{
    public record UpdateOrderDto
    (
        int ProductId,
        string ProductName,
        decimal ProductPrice,
        int Quantity,
        decimal TotalAmount,
        DateTime Date
    );
}
