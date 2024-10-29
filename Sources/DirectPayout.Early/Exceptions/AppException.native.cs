using DirectPayout.Early.Constants;

namespace DirectPayout.Early.Exceptions;

public class AppException : Exception
{
    public string ErrCode { get; set; }

    public AppException()
    {
    }

    public AppException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public AppException(string message, string code = null) : base(message)
    {
        ErrCode = code;
    }
}

public class RunningOddsException : AppException
{
    public RunningOddsException() : base(ErrorMsg.RunningOdds, ErrorCodes.RunningOdds)
    {
    }

    public RunningOddsException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public class MatchCalculatedException : AppException
{
    public MatchCalculatedException() : base(ErrorMsg.MatchCalculated, ErrorCodes.MatchCalculated)
    {
    }

    public MatchCalculatedException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public class InvalidInputException : AppException
{
    public InvalidInputException() : base(ErrorMsg.InvalidInput, ErrorCodes.InvalidInput)
    {
    }

    public InvalidInputException(string message, string errorCode) : base(message, errorCode)
    {
    }

    public InvalidInputException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public class FTInconsistentException : AppException
{
    public FTInconsistentException() : base(ErrorMsg.FTInconsistent, ErrorCodes.FTInconsistent)
    {
    }

    public FTInconsistentException(string message, Exception innerException) : base(message, innerException)
    {
    }
}