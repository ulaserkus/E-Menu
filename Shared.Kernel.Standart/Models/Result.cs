namespace Shared.Kernel
{
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public string Error { get; set; }
        public T Value { get; set; }
        public int StatusCode { get; set; }

        public Result() { }
        private Result(T value, bool isSuccess, string error, int statusCode)
        {
            Value = value;
            IsSuccess = isSuccess;
            Error = error;
            StatusCode = statusCode;
        }

        public static Result<T> Success(T value, int statusCode = 200)
            => new Result<T>(value, true, null, statusCode);

        public static Result<T> Failure(string error, int statusCode = 400)
            => new Result<T>(default, false, error, statusCode);
    }
}