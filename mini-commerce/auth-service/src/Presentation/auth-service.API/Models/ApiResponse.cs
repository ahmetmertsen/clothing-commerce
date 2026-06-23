namespace auth_service.API.Models
{
    public class ApiResponse
    {
        public bool IsSuccess { get; set; }
        public ErrorResponse? Error { get; set; }
    }
}
