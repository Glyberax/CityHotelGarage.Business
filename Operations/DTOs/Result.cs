namespace CityHotelGarage.Business.Operations.Results;

public class Result<T>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = "";
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();

    public static Result<T> Success(T data, string message = "")
    {
        return new Result<T>
        {
            IsSuccess = true,
            Data = data,
            Message = message
        };
    }

    public static Result<T> Failure(string message, List<string>? errors = null)
    {
        return new Result<T>
        {
            IsSuccess = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }
}

public class Result
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = "";
    public List<string> Errors { get; set; } = new();

    public static Result Success(string message = "")
    {
        return new Result
        {
            IsSuccess = true,
            Message = message
        };
    }

    public static Result Failure(string message, List<string>? errors = null)
    {
        return new Result
        {
            IsSuccess = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }
}