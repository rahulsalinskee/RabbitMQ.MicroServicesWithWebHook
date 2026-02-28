using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Data.Models.OrderModel
{
    public class Order
    {
        public int ID { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public DateTime Date { get; set; }
    }
}
