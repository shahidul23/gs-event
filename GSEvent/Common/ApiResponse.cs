using System;

namespace GSEvent.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public int StatusCode {get; set;}
    public string Message {get; set;} = string.Empty;
    public T? Data {get; set;}
    public object? Errors {get; set;}
    public DateTime TimeStamp {get; set;}

    public static ApiResponse<T> SuccessResponse(
        T? data,
        string message = "Request successfull.",
        int statusCode = StatusCodes.Status200OK
    )
    {
        return new ApiResponse<T>
        {
            Success = true,
            StatusCode = statusCode,
            Message = message,
            Data = data,
            TimeStamp = DateTime.UtcNow
        };
    }
    public static ApiResponse<T> ErrorResponse(
        string message,
        int statusCode,
        object? errors = null

    )
    {
        return new ApiResponse<T>
        {
            Success = false,
            StatusCode = statusCode,
            Message = message,
            Errors = errors,
            TimeStamp = DateTime.UtcNow
        };
    }

}
