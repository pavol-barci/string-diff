using StringDiff.Domain;
using StringDiff.Domain.Models;

namespace StringDiff.Application;

/// <summary>
/// Service to calculate diffs for models
/// </summary>
public interface IDiffCalculator
{
    /// <summary>
    /// Calculates the difference of the input model. 
    /// </summary>
    /// <param name="model">Model with Left and Right strings to compare</param>
    /// <returns>Calculated difference value
    ///     - null => when at least one of the strings are null
    ///     - <see cref="DiffResult"/> => when strings are filled. This object contains the result with possible offset and length
    /// </returns>
    Task<DiffResult?> CalculateDiff(DiffModel model);
}