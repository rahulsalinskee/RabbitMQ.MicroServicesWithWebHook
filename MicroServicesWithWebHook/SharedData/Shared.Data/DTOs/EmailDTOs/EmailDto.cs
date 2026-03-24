using System.Text;

namespace Shared.Data.DTOs.EmailDTOs
{
    public record EmailDto
    {
        public string Title { get; set; } = string.Empty;

        public string? Content { get; set; }
    }
}
