using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.DataLayer;
using Order.API.Repository.CacheServices.Services;
using Order.API.Repository.FilterServices.Services;
using Order.API.Repository.OrderServices.Services;
using Shared.Data.DTOs.EmailDTOs;
using Shared.Data.DTOs.OrderDTOs;
using Shared.Data.DTOs.OrderEmailBodyDTOs;
using Shared.Data.DTOs.ResponseDTOs;
using Shared.Data.Mapper.OrderMapper;
using Shared.Data.Models.ErrorModel;
using System.Text;
using OrderModel = Shared.Data.Models.OrderModel.Order;

namespace Order.API.Repository.OrderServices.Implementations
{
    public class OrderImplementation : IOrderService
    {
        private readonly OrderDbContext _orderDbContext;
        private readonly IFilterService<OrderModel> _filterService;
        private readonly ICacheService _cacheService;
        private readonly IPublishEndpoint _publishEndpoint;
        private const string ALL_ORDERS_CACHE_KEY = "ALL_ORDERS";

        public OrderImplementation(OrderDbContext orderDbContext, IFilterService<OrderModel> filterService, ICacheService cacheService, IPublishEndpoint publishEndpoint)
        {
            this._orderDbContext = orderDbContext;
            this._filterService = filterService;
            this._cacheService = cacheService;
            this._publishEndpoint = publishEndpoint;
        }

        public async Task<ResponseDto> AddOrderAsync(AddOrderDto addNewOrderDto)
        {
            if (addNewOrderDto is null)
            {
                ApplicationError applicationError = new()
                {
                    When = DateTime.Now,
                    Message = "Order Does Not Exist!",
                };

                return new ResponseDto()
                {
                    IsSuccess = false,
                    Result = null,
                    Message = applicationError.Message,
                    When = applicationError.When,
                };
            }

            var orderDto = new OrderDto()
            {
                ProductName = addNewOrderDto.ProductName,
                ProductPrice = addNewOrderDto.ProductPrice,
                Quantity = addNewOrderDto.Quantity,
                TotalAmount = addNewOrderDto.TotalAmount,
                Date = addNewOrderDto.Date,
                ProductId = addNewOrderDto.ProductId,
            };

            var order = orderDto.ConvertOrderDtoToOrderModelExtension();

            await this._orderDbContext.Orders.AddAsync(order);
            await this._orderDbContext.SaveChangesAsync();

            /* Invalidate (Delete) the cache */
            await this._cacheService.RemoveDataAsync(key: ALL_ORDERS_CACHE_KEY);

            var addedProductDto = order.ConvertOrderModelToOrderDtoExtension();

            var response = await GetOrderSummaryAsync();

            OrderEmailBodyDto orderEmailBodyDto = new()
            {
                OrderID = response.orderSummaryDto!.ID,
                ProductName = response.orderSummaryDto.ProductName,
                ProductPrice = response.orderSummaryDto.ProductPrice,
                Quantity = response.orderSummaryDto.Quantity,
                TotalAmount = response.orderSummaryDto.TotalAmount,
                Date = response.orderSummaryDto.Date,
            };

            await EmailContentAsync(orderEmailBodyDto: orderEmailBodyDto);

            return new ResponseDto()
            {
                IsSuccess = true,
                Result = addedProductDto,
                Message = "Order Placed Successfully!",
                When = DateTime.Now,
            };
        }

        public async Task<ResponseDto> GetAllOrdersAsync(string? columnName = null, string? filterValue = null)
        {
            var orders = await this._orderDbContext.Orders.AsNoTracking().ToListAsync();

            if (orders is null)
            {
                ApplicationError applicationError = new()
                {
                    When = DateTime.Now,
                    Message = "Orders not found"
                };

                return new ResponseDto()
                {
                    IsSuccess = false,
                    Result = null,
                    Message = applicationError.Message,
                    When = applicationError.When,
                };
            }

            if (orders.Count is 0)
            {
                ApplicationError applicationError = new()
                {
                    When = DateTime.Now,
                    Message = "Order Not Found!",
                };

                return new ResponseDto()
                {
                    IsSuccess = false,
                    Result = 0,
                    Message = applicationError.Message,
                    When = applicationError.When,
                };
            }

            OrderDto orderDto = new();

            IList<OrderDto> orderDtos = new List<OrderDto>();

            var filteredOrders = this._filterService.ApplyFilterOn(queryOn: orders.AsQueryable(), columnName: columnName, filterKeyWord: filterValue);

            if (filteredOrders.Any())
            {
                foreach (var order in filteredOrders)
                {
                    orderDto = order.ConvertOrderModelToOrderDtoExtension();

                    orderDtos.Add(orderDto);
                }

                return new ResponseDto()
                {
                    IsSuccess = true,
                    Result = orderDtos,
                    Message = "Orders Found!",
                    When = DateTime.Now,
                };
            }
            else
            {
                ApplicationError applicationError = new()
                {
                    When = DateTime.Now,
                    Message = "Order Not Found With This Filter Keyword!",
                };

                return new ResponseDto()
                {
                    IsSuccess = false,
                    Result = null,
                    Message = applicationError.Message,
                    When = applicationError.When,
                };
            }
        }

        public async Task<ResponseDto> GetOrderByIdAsync(int orderId)
        {
            var cacheKey = $"PRODUCT_{orderId}";

            /* Initially, Check the cache for the product */

            var getProductFromCache = await this._cacheService.GetDataAsync<OrderModel>(key: cacheKey);

            if (getProductFromCache is not null)
            {
                return new ResponseDto()
                {
                    IsSuccess = true,
                    Result = getProductFromCache,
                    Message = "Product Found In Cache!",
                    When = DateTime.Now,
                };
            }

            var product = await this._orderDbContext.Orders.FirstOrDefaultAsync(product => product.ID == orderId);

            if (product is null)
            {
                ApplicationError applicationError = new()
                {
                    Message = "Product Is Null!",
                    When = DateTime.Now,
                };

                return new ResponseDto()
                {
                    IsSuccess = false,
                    Result = null,
                    Message = applicationError.Message,
                    When = applicationError.When,
                };
            }

            /* Add the product to the cache */
            await this._cacheService.SetDataAsync<OrderModel>(key: cacheKey, data: product, absoluteExpireTime: TimeSpan.FromMinutes(5));

            return new ResponseDto()
            {
                IsSuccess = true,
                Result = product,
                Message = "Product Found!",
                When = DateTime.Now,
            };
        }

        public async Task<(ResponseDto responseDto, OrderSummaryDto? orderSummaryDto)> GetOrderSummaryAsync()
        {
            var order = await this._orderDbContext.Orders.AsNoTracking().FirstOrDefaultAsync();

            var products = await this._orderDbContext.Products.AsNoTracking().ToListAsync();

            var productInformation = products.Find(product => product.ID == order!.ProductId);

            if (order is null)
            {
                ApplicationError applicationError = new()
                {
                    When = DateTime.Now,
                    Message = "Order Not Found!",
                };

                return 
                (
                    responseDto: new ResponseDto()
                    {
                        IsSuccess = false,
                        Result = null,
                        Message = applicationError.Message,
                        When = applicationError.When,
                    }, 
                    orderSummaryDto: null
                );
            }

            OrderSummaryDto orderSummaryDto = new()
            {
                ID = order.ID,
                ProductId = order.ProductId,
                ProductName = productInformation!.Name,
                ProductPrice = productInformation.Price,
                Quantity = order.Quantity,
                TotalAmount = order.Quantity * productInformation.Price,
                Date = order.Date,
            };

            return
            (
                responseDto: new ResponseDto()
                {
                    IsSuccess = true,
                    Result = orderSummaryDto,
                    Message = "Order Summary Found!",
                    When = DateTime.Now,
                },
                orderSummaryDto: orderSummaryDto
            );
        }

        private async Task EmailContentAsync(OrderEmailBodyDto orderEmailBodyDto)
        {
            var contentForEmail = BuildOrderEmailBody(orderEmailBodyDto: orderEmailBodyDto);

            await this._publishEndpoint.Publish(message: new EmailDto() { Title = "Order Information", Content = contentForEmail });

            await ClearOrderTableAsync();
        }

        private static string BuildOrderEmailBody(OrderEmailBodyDto orderEmailBodyDto)
        {
            StringBuilder displayOrderDetail = new();

            displayOrderDetail.AppendLine($"<h1><strong>Order Information Summary</strong></h1> <br>");

            displayOrderDetail.AppendLine($"<h2><strong>Order ID:</strong> {orderEmailBodyDto.OrderID}</h2> <br>");

            displayOrderDetail.AppendLine($"<h2>Order Item:</h2>");

            displayOrderDetail.AppendLine($"<ul>");

            displayOrderDetail.AppendLine($"<li>Order Placed Date: {orderEmailBodyDto.Date}</li>");
            displayOrderDetail.AppendLine($"<li>Product Name: {orderEmailBodyDto.ProductName}</li>");
            displayOrderDetail.AppendLine($"<li>Price: {orderEmailBodyDto.ProductPrice}</li>");
            displayOrderDetail.AppendLine($"<li>Quantity: {orderEmailBodyDto.Quantity}</li>");
            displayOrderDetail.AppendLine($"<li>Total Amount: {orderEmailBodyDto.TotalAmount}</li>");

            displayOrderDetail.AppendLine($"</ul>");

            displayOrderDetail.AppendLine("Thank you for your order!");

            return displayOrderDetail.ToString();
        }

        private async Task ClearOrderTableAsync()
        {
            this._orderDbContext.Orders.Remove(await this._orderDbContext.Orders.FirstOrDefaultAsync());

            await this._orderDbContext.SaveChangesAsync();
        }
    }
}
