using StringDiff.Application.Services;

namespace StringDiff.Domain;

public class DiffResult
{
    public string? Result { get; set; }
    
    public DiffResultResponse ToDiffResultResponse()
    {
        return new DiffResultResponse(Result);
    }
}