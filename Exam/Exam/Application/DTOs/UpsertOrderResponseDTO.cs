namespace Exam.Application.DTOs
{
    public class UpsertOrderResponseDTO
    {
        public string Message { get; set; }
        public object Data { get; set; }
        public UpsertOrderResponseDTO(string message, object data)
        {
            Message = message;
            Data = data;
        }
    }
}