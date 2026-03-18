using Shared.Data.DTOs.OrderDTOs;
using Shared.Data.DTOs.ResponseDTOs;

namespace Order.API.Repository.OrderServices.Services
{
    public interface IOrderService
    {
        public Task<ResponseDto> GetAllOrdersAsync(string? columnName = null, string? filterValue = null);

        public Task<ResponseDto> GetOrderByIdAsync(int orderId);

        public Task<ResponseDto> AddOrderAsync(AddOrderDto addNewOrderDto);

        public Task<(ResponseDto responseDto, OrderSummaryDto? orderSummaryDto)> GetOrderSummaryAsync();

        //public Task<ResponseDto> UpdateOrderAsync(int orderId, UpdateOrderDto updateOrderDto);

        //public Task<ResponseDto> DeleteOrderAsync(int orderId);
    }
}
