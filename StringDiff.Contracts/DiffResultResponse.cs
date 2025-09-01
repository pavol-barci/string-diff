namespace StringDiff.Contracts;

public enum DiffResultResponseType
{
    Equal = 0,
    DifferentSize = 1,
    NotEquals = 2,
    NotFinished = 3
}


public record DiffResultResponse
{
    public DiffResultResponseType Result { get; set; }
    public Difference? Difference { get; set; }

    public DiffResultResponse()
    {
    }
    
    public DiffResultResponse(DiffResultResponseType result)
    {
        Result = result;
    }
    
    public DiffResultResponse(DiffResultResponseType result, Difference difference) : this(result)
    {
        Difference = difference;
    }
}

public record Difference(int Offset, int Length);