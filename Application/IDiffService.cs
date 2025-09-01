using StringDiff.Application.Models;
using StringDiff.Domain.Exceptions;
using StringDiff.Models;

namespace StringDiff.Application;

/// <summary>
/// Service to process input strings and diffs
/// </summary>
public interface IDiffService
{
    /// <summary>
    /// Upsert the left input string to the model. If both models are already filled then calculate the diff
    /// </summary>
    /// <param name="id">Id of the model to be updated</param>
    /// <param name="request">Request containing left input string to update</param>
    /// <returns>
    ///     <c>true</c> if the model with specific id was created, <c>false</c> if it was only updated.
    /// </returns>
    /// <exception cref="NotFoundException">Thrown when the model with id does not exist, was not created and attempt to update was made.</exception>
    /// <exception cref="ConflictException">Thrown when the model with id already exists and attempt to create was made.</exception>
    Task<bool> UpsertLeft(int id, DiffRequest request);
    
    /// <summary>
    /// Upsert the right input string to the model. If both models are already filled then calculate the diff
    /// </summary>
    /// <param name="id">Id of the model to be updated</param>
    /// <param name="request">Request containing right input string to update</param>
    /// <returns>
    ///     <c>true</c> if the model with specific id was created, <c>false</c> if it was only updated.
    /// </returns>
    /// <exception cref="NotFoundException">Thrown when the model with id does not exist, was not created and attempt to update was made.</exception>
    /// <exception cref="ConflictException">Thrown when the model with id already exists and attempt to create was made.</exception>
    Task<bool> UpsertRight(int id, DiffRequest request);
    
    /// <summary>
    /// Get the calculated difference of strings
    /// </summary>
    /// <param name="id">Id of the model to get the diff</param>
    /// <returns>Calculated difference value
    ///     - null => when diff was not calculated yet
    ///     - <see cref="DiffResultResponse"/> => when diff already calculated. This object contains the result with possible offset and length
    /// </returns>
    /// <exception cref="NotFoundException">Thrown when the model with id does not exist.</exception>
    Task<DiffResultResponse?> GetDiff(int id);
}