namespace BookingManagement.Common.Models
{
    // How the MVC client reports the outcome of an API call to its controllers: either the data,
    // or enough about the failure (status code, problem detail, unreachable) to choose a message.
    public class ApiResult
    {
        public bool IsSuccess { get; init; }

        public int? StatusCode { get; init; }

        public string? Detail { get; init; }

        public bool IsConnectionFailure { get; init; }
    }

    public class ApiResult<T> : ApiResult
    {
        public T? Data { get; init; }

        public static ApiResult<T> Success(T data) => new() { IsSuccess = true, Data = data };

        public static ApiResult<T> Failure(int? statusCode, string? detail) =>
            new() { StatusCode = statusCode, Detail = detail };

        public static ApiResult<T> ConnectionFailure() => new() { IsConnectionFailure = true };
    }
}
