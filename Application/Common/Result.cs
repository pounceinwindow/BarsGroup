using Application.Common;

namespace DoctorSite.Application.Common
{
    public class Result
    {
        public bool IsSucceed { get; set; }

        public List<string> Errors { get; set; } = new();

        public Exception? Exception { get; set; }

        public static Result Failure(List<string> errors, Exception? ex = null)
            => new Result { Errors = errors, Exception = ex, IsSucceed = false };

        public static Result Failure(DomainError error)
            => new Result { Errors = new List<string> { error.Message }, IsSucceed = false };

        public async static Task<Result> FailureAsync(List<string> errors, Exception? ex = null)
            => new Result { Errors = errors, Exception = ex, IsSucceed = false };

        public static Result Failure(string error, Exception? ex = null)
            => new Result { Errors = new List<string>() { error }, Exception = ex, IsSucceed = false };

        public async static Task<Result> FailureAsync(string error, Exception? ex = null)
            => new Result { Errors = new List<string>() { error }, Exception = ex, IsSucceed = false };

        public static Result Failure(Exception ex)
            => new Result { Errors = new List<string>() { ex.Message }, Exception = ex, IsSucceed = false };

        public async static Task<Result> FailureAsync(Exception ex)
            => new Result { Errors = new List<string>() { ex.Message }, Exception = ex, IsSucceed = false };

        public static Result Success()
            => new Result { IsSucceed = true };

        public async static Task<Result> SuccessAsync()
            => new Result { IsSucceed = true };
    }

    public class Result<T>
    {
        public bool IsSucceed { get; set; }

        public T? Data { get; set; }

        public List<string> Errors { get; set; } = new();

        public Exception? Exception { get; set; }

        public static Result<T> Failure(List<string> errors, Exception? ex = null)
            => new Result<T> { Errors = errors, Exception = ex, IsSucceed = false };

        public static Result<T> Failure(DomainError error)
            => new Result<T> { Errors = new List<string> { error.Message }, IsSucceed = false };

        public async static Task<Result<T>> FailureAsync(List<string> errors, Exception? ex = null)
            => new Result<T> { Errors = errors, Exception = ex, IsSucceed = false };

        public static Result<T> Failure(string error, Exception? ex = null)
            => new Result<T> { Errors = new List<string>() { error }, Exception = ex, IsSucceed = false };

        public async static Task<Result<T>> FailureAsync(string error, Exception? ex = null)
            => new Result<T> { Errors = new List<string>() { error }, Exception = ex, IsSucceed = false };

        public static Result<T> Failure(Exception ex)
            => new Result<T> { Errors = new List<string>() { ex.Message }, Exception = ex, IsSucceed = false };

        public async static Task<Result<T>> FailureAsync(Exception ex)
            => new Result<T> { Errors = new List<string>() { ex.Message }, Exception = ex, IsSucceed = false };

        public static Result<T> Success(T data)
            => new Result<T> { Data = data, IsSucceed = true };

        public async static Task<Result<T>> SuccessAsync(T data)
            => new Result<T> { Data = data, IsSucceed = true };
    }
}
