using StringDiff.Domain;

namespace StringDiff.Application;

public interface IDiffCalculator
{
    Task<DiffResult?> CalculateDiff(DiffModel model);
}