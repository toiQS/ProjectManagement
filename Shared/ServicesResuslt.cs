namespace Shared
{
    public class ServicesResult<T>
    {
        public string Message { get; private set; } = string.Empty;
        public T? Data { get; private set; }
        public bool Status { get; private set; }

        public ServicesResult() {}

        public ServicesResult(string message)
        {
            Message = message;
            Status = false;
        }

        public ServicesResult(T data)
        {
            if (data is null)
            {
                Status = false;
                Message = "Data is null here";
            }
            else
            {
                Data = data;
                Status = true;
            }
        }

        public static ServicesResult<T> Success(T data)
        {
            return new ServicesResult<T>(data);
        }

        public static ServicesResult<T> Failure(string message)
        {
            return new ServicesResult<T>(message);
        }
    }
}
