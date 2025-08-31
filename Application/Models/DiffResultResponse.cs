namespace StringDiff.Application.Models;

public class DiffResultResponse
{
    public string? Result { get; set; }

    public DiffResultResponse(string? result)
    {
        Result = result;
    }
}