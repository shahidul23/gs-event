using System;

namespace GSEvent.Exceptions;

public class BadRequestException : AppException
{
    public BadRequestException(
        string message
    ) : base(message, StatusCodes.Status400BadRequest)
    {  }
}
