using System;

namespace HyPlayer.PlayCore.Abstraction.Models;

public sealed class PlayCoreError
{
    public PlayCoreError(string errorCode, string errorMessage, Exception? exception = null)
    {
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
        Exception = exception;
    }

    public string ErrorCode { get; }
    public string ErrorMessage { get; }
    public Exception? Exception { get; }
}

public sealed class PlayCoreResult<T>
{
    private readonly T? _value;
    private readonly PlayCoreError? _error;

    private PlayCoreResult(T value)
    {
        IsSuccess = true;
        _value = value;
    }

    private PlayCoreResult(PlayCoreError error)
    {
        IsSuccess = false;
        _error = error;
    }

    public bool IsSuccess { get; }
    public bool IsError => !IsSuccess;

    public T Value
    {
        get
        {
            if (IsError)
                throw new InvalidOperationException("Cannot read Value from an error result.");
            return _value!;
        }
    }

    public PlayCoreError Error
    {
        get
        {
            if (IsSuccess)
                throw new InvalidOperationException("Cannot read Error from a success result.");
            return _error!;
        }
    }

    public string? ErrorCode => _error?.ErrorCode;
    public string? ErrorMessage => _error?.ErrorMessage;
    public Exception? Exception => _error?.Exception;

    public static PlayCoreResult<T> CreateSuccess(T value)
    {
        return new PlayCoreResult<T>(value);
    }

    public static PlayCoreResult<T> CreateError(string errorCode, string errorMessage, Exception? exception = null)
    {
        return new PlayCoreResult<T>(new PlayCoreError(errorCode, errorMessage, exception));
    }

    public static PlayCoreResult<T> CreateError(PlayCoreError error)
    {
        return new PlayCoreResult<T>(error);
    }

    public TResult Match<TResult>(Func<T, TResult> success, Func<PlayCoreError, TResult> error)
    {
        return IsSuccess ? success(Value) : error(Error);
    }

    public static implicit operator PlayCoreResult<T>(T value)
    {
        return CreateSuccess(value);
    }

    public static implicit operator PlayCoreResult<T>(PlayCoreError error)
    {
        return CreateError(error);
    }

    public static implicit operator PlayCoreResult<T>(Exception exception)
    {
        return CreateError(exception.GetType().Name, exception.Message, exception);
    }
}
