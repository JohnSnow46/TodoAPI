namespace TodoAPI.Application.DTOs.Common
{
    public class ValidationErrorResponse
    {
        public string Message { get; set; } = "Validation failed";
        public Dictionary<string, List<string>> Errors { get; set; } = new();
    }
}
