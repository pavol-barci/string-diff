namespace StringDiff.Domain.Models;

public enum DiffResultType
{
    Equals,
    DifferentSizes,
    NotEquals
}

public class DiffResult
{
    public DiffResultType Result { get; set; }
    public int Offset { get; set; }
    public int Length { get; set; }

    public DiffResult(DiffResultType result)
    {
        Result = result;
    }

    public DiffResult(DiffResultType type, int offset, int length) : this(type)
    {
        Offset = offset;
        Length = length;
    }
}