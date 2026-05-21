namespace MovieBase.Common;

/// <summary>
/// Result Object Pattern => Wikipedia
/// </summary>
/// <typeparam name="T"></typeparam>
public class Result<T>
{
    public Result(bool success, T? payload, string message="")
    {
        Success = success;
        Payload = payload;
        Message = message;
    }

    public bool Success { get; }
    public T? Payload { get; }
    public string Message { get; }
}
