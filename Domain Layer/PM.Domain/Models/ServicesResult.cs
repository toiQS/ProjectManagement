namespace PM.Domain.Models
{
    public class ServicesResult<T>
    {
        private T _value;
        private string _message;
        private bool _status;
        public ServicesResult(T value)
        {
            if (value is null)
            {
                _message = "No data here";
                _status = true;
            }
            else
            {
                _message = "Success";

            }
        }
    }
}
