namespace SimplePos.Domain.Common.ResultPattern
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }

        protected Result(bool isSuccess, Error error)
        {
            if (isSuccess && error != Error.None ||
                !isSuccess && error == Error.None)
            {
                throw new ArgumentException("Invalid Error", nameof(error));
            }
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new Result(true, Error.None);
        public static Result Failure(Error error) => new Result(false, error);
    }

    public class Result<T> : Result
    {
        public T? Data { get; }

        private Result(bool isSuccess, T? data, Error error) : base(isSuccess, error)
        {
            Data = data;
        }

        public static Result<T> Success(T data) => new Result<T>(true, data, Error.None);
        public new static Result<T> Failure(Error error) => new Result<T>(false, default, error);
    }
}