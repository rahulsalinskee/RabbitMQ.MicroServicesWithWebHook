using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Data.DTOs.ProductDTOs.Version2
{
    public class UpdateProductDto
    {
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal CostPrice { get; set; }
    }
}
