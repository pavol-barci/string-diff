using StringDiff.Domain;
using StringDiff.Domain.Models;

namespace StringDiff.Application.Services;

///<inheritdoc/>
public class DiffCalculator(ILogger<DiffCalculator> logger) : IDiffCalculator
{
    private record ComparisonResult(bool Same, int Offset = 0, int Length = 0);

    ///<inheritdoc/>
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

    /// <summary>
    /// Compare the models left and right strings. 
    /// </summary>
    /// <param name="model">Input model to compare with already filled Left and Right strings</param>
    /// <returns>
    ///     <see cref="ComparisonResult"/> containing the result of compare. Same = <c>true</c> when they are same,
    ///     <c>false</c> with Offset and Length otherwise
    /// </returns>
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