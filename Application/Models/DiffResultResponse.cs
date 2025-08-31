namespace StringDiff.Application.Models;

public enum DiffResultResponseType
{
    Equal = 0,
    DifferentSize = 1,
    NotEquals = 2,
    NotFinished = 3
}


public record DiffResultResponse(DiffResultResponseType Result)
{
    public DiffResultResponseType Result { get; set; } = Result;
    public Difference? Difference { get; set; }

    public DiffResultResponse(DiffResultResponseType result, Difference difference) : this(result)
    {
        Difference = difference;
    }
}

public record Difference(int Offset, int Length);