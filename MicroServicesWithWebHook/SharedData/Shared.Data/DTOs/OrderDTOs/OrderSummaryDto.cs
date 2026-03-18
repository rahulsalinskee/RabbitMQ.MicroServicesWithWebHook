using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Data.DTOs.OrderDTOs
{
    public record OrderSummaryDto
    {
        public int ID { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public decimal ProductPrice { get; set; }

        public int Quantity { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime Date { get; set; }
    }
}
