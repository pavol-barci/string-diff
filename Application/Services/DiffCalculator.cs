using StringDiff.Domain;

namespace StringDiff.Application.Services;

public class DiffCalculator(ILogger<DiffCalculator> logger) : IDiffCalculator
{
    private record ComparisonResult(bool Same, int Offset = 0, int Length = 0);

    public Task<DiffResult?> CalculateDiff(DiffModel model)
    {
        if (model.Left is null || model.Right is null)
        {
            logger.LogInformation("One or both values of model are not filled, Left = {left}, Right = {right}",
                model.Left, model.Right);
            return Task.FromResult<DiffResult?>(null);
        }

        if (model.Left.Length != model.Right.Length)
        {
            return Task.FromResult<DiffResult?>(new DiffResult(DiffResultType.DifferentSizes));
        }

        var compareResult = CompareModel(model);
        var diffResult = compareResult.Same
            ? new DiffResult(DiffResultType.Equals)
            : new DiffResult(DiffResultType.NotEquals, compareResult.Offset, compareResult.Length);

        return Task.FromResult<DiffResult?>(diffResult);
}

    private static ComparisonResult CompareModel(DiffModel model)
    {
        var length = model.Left!.Length;

        for (var index = 0; index < length; index++)
        {
            if (model.Left[index] != model.Right![index])
            {
                return new ComparisonResult(false, index, length - index);
            }
        }

        return new ComparisonResult(true);
    }
}