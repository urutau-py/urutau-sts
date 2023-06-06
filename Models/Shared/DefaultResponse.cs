namespace urutau.Models.Shared;

public class DefaultResponse
{
    public List<DefaultError> Errors { get; set; } = new();
    public bool Succeeded { get; set; }

    public DefaultResponse(bool succeeded)
    {
        Succeeded = succeeded;
    }
    
    public DefaultResponse(bool succeeded, DefaultError error)
    {
        Succeeded = succeeded;
        Errors = new List<DefaultError> { error };
    }
    
    public DefaultResponse(bool succeeded, List<DefaultError> errors)
    {
        Succeeded = succeeded;
        Errors = errors;
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
    
    public DefaultResponse(bool succeeded, List<DefaultError> errors)
    {
        Succeeded = succeeded;
        Errors = errors;
    }
    
    public DefaultResponse(bool succeeded, List<DefaultError> errors, T data)
    {
        Succeeded = succeeded;
        Errors = errors;
        Data = data;
    }
    
    public DefaultResponse(bool succeeded, T data)
    {
        Succeeded = succeeded;
        Data = data;
    }
    
    public T? Data { get; set; }
}

public sealed class DefaultError
{
    public DefaultError(string code, string description)
    {
        Code = code;
        Description = description;
    }
    
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}