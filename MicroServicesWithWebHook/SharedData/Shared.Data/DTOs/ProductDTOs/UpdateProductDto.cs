using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Shared.Data.DTOs.ProductDTOs
{
    public class UpdateProductDto
    {
        public int ID { get; set; }

        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
    }
}
