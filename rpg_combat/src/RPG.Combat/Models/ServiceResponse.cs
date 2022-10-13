namespace RPG.Combat.Models
{
    public class ServiceResponse<T>
    {
        public T Data { get; set; }
        public bool Success { get; set; } = true;
        public string Message { get; set; }

        public static ServiceResponse<T> From(T data)
        {
            return new ServiceResponse<T> {
                Data = data,
                Success = true
            };
        }

        public static ServiceResponse<T> FailedFrom(string message)
        {
            return new ServiceResponse<T> {
                Success = false,
                Message = message
            };
        }
    }
}