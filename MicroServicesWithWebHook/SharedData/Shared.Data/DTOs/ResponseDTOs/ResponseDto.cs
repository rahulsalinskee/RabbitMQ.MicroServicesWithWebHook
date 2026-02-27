namespace Shared.Data.DTOs.ResponseDTOs
{
    public class ResponseDto
    {
        public object? Result { get; set; } 

        public bool IsSuccess { get; set; } = false;

        public string Message { get; set; } = string.Empty;

        public DateTime WhenErrorOccured { get; set; } = DateTime.Now;
    }
}
