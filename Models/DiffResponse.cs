namespace StringDiff.Models;

public enum DiffResult
{
    Equal,
    DifferentSize,
    Different
}

public class DiffResponse
{
    public DiffResult Result { get; set; }
    public Difference? Difference { get; set; }
}

public record Difference
{
    public int LeftOffset { get; set; }
    public int RightOffset { get; set; }
    public int LeftLength { get; set; }
    public int RightLength { get; set; }
}