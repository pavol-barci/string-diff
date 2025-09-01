namespace StringDiff.Domain.Models;

public class DiffModel
{
    public int Id { get; set; }
    public string? Left { get; set; }
    public string? Right { get; set; }
    public DiffResult? DiffResult { get; set; }

    public DiffModel(string? left = null, string? right = null)
    {
        Left = left;
        Right = right;
    }

    public bool BothSidesFilled() => Left is not null && Right is not null;
}