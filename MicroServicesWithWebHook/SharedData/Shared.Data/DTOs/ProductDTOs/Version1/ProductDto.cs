using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Data.DTOs.ProductDTOs.Version1
{
    public class ProductDto
    {
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
    }
}
