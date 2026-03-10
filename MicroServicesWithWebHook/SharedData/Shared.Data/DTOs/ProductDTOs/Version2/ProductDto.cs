using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Data.DTOs.ProductDTOs.Version2
{
    public class ProductDto
    {
        public int ID { get; set; }

        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal CostPrice { get; set; }
    }
}
