namespace StringDiff.Contracts;

public class ErrorResponse(string message, IEnumerable<ValidationError>? validationErrors = null)
{
    public string Message { get; set; } = message;
    public IEnumerable<ValidationError>? ValidationErrors { get; set; } = validationErrors;
}

public class ValidationError
{
    public string Property { get; set; }
    public IEnumerable<string>? Errors { get; set; }
}