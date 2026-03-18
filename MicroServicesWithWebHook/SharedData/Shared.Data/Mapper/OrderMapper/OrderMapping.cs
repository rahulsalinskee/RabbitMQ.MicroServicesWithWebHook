using Shared.Data.DTOs.OrderDTOs;
using OrderModel = Shared.Data.Models.OrderModel.Order;

namespace Shared.Data.Mapper.OrderMapper
{
    public static class OrderMapping
    {
        public static OrderModel ConvertOrderDtoToOrderModelExtension(this OrderDto orderDto)
        {
            return new OrderModel()
            {
                ID = orderDto.Id,
                ProductId = orderDto.ProductId,
                Quantity = orderDto.Quantity,
                Date = orderDto.Date,
            };
        }

        public static OrderDto ConvertOrderModelToOrderDtoExtension(this OrderModel orderModel)
        {
            return new OrderDto()
            {
                Id = orderModel.ID,
                ProductId = orderModel.ProductId,
                Quantity = orderModel.Quantity,
                Date = orderModel.Date
            };
        }
    }
}
