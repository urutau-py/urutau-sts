namespace urutau.Models.Shared;

public class DefaultResponse
{
    public List<string> Messages { get; set; } = new();
    public bool Succeeded { get; set; }

    public DefaultResponse(bool succeeded)
    {
        Succeeded = succeeded;
    }
    
    public DefaultResponse(bool succeeded, string message)
    {
        Succeeded = succeeded;
        Messages = new List<string> { message };
    }
    
    public DefaultResponse(bool succeeded, List<string> messages)
    {
        Succeeded = succeeded;
        Messages = messages;
    }

    public DefaultResponse()
    {
        
    }
}

public sealed class DefaultResponse<T> : DefaultResponse where T : class
{
    public DefaultResponse(bool succeeded)
    {
        Succeeded = succeeded;
    }
    
    public DefaultResponse(bool succeeded, List<string> messages)
    {
        Succeeded = succeeded;
        Messages = messages;
    }
    
    public DefaultResponse(bool succeeded, List<string> messages, T data)
    {
        Succeeded = succeeded;
        Messages = messages;
        Data = data;
    }
    
    public DefaultResponse(bool succeeded, T data)
    {
        Succeeded = succeeded;
        Data = data;
    }
    
    public T? Data { get; set; }
}