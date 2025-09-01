using System.Text;
using Microsoft.Extensions.Logging;
using StringDiff.Application.Abstraction.Services;
using StringDiff.Application.Helpers;
using StringDiff.Contracts;
using StringDiff.Domain.Exceptions;
using StringDiff.Domain.Models;
using StringDiff.Domain.Repositories;

namespace StringDiff.Application.Services;

///<inheritdoc/>
public class DiffService(IDiffRepository diffRepository, IDiffCalculator diffCalculator, ILogger<DiffService> logger)
    : IDiffService
{
    ///<inheritdoc/>
    public async Task<bool> UpsertLeft(Guid id, DiffRequest diffRequest)
    {
        var decodedInput = DecodeInput(diffRequest);
        return await UpsertInternal(id, diffModel => diffModel.Left = decodedInput, () => new DiffModel(decodedInput));
    }

    ///<inheritdoc/>
    public async Task<bool> UpsertRight(Guid id, DiffRequest diffRequest)
    {
        var decodedInput = DecodeInput(diffRequest);
        return await UpsertInternal(id, diffModel => diffModel.Right = decodedInput, () => new DiffModel(right: decodedInput));
    }

    ///<inheritdoc/>
    public async Task<DiffResultResponse?> GetDiff(Guid id)
    {
        var model = await diffRepository.GetById(id);
        
        if (model is null)
        {
            logger.LogWarning("Diff result with id {id} does not exists.", id);
            throw new NotFoundException($"Model with id {id} does not exists.");
        }
        
        return model.DiffResult.ToDiffResultResponse();
    }

    /// <summary>
    /// Upsert the specified model.
    /// </summary>
    /// <param name="id">Id of the model to be updated</param>
    /// <param name="updateAction">Action how the model should be updated</param>
    /// <param name="createFunction">Function how the model should be created if it does not exist</param>
    /// <returns>
    ///     <c>true</c> if the model with specific id was created, <c>false</c> if it was only updated.
    /// </returns>
    /// <exception cref="NotFoundException">Thrown when the model with id does not exist, was not created and attempt to update was made.</exception>
    /// <exception cref="ConflictException">Thrown when the model with id already exists and attempt to create was made.</exception>
    private async Task<bool> UpsertInternal(Guid id, Action<DiffModel> updateAction, Func<DiffModel> createFunction)
    {
        var model = await diffRepository.GetById(id);
        var created = false;
        
        if (model == null)
        {
            created = true;
            model = createFunction();
            model.Id = id;
            
            await diffRepository.Create(model);
        }
        else
        {
            updateAction(model);
        }

        if (model.BothSidesFilled())
        {
            var diffResult = await diffCalculator.CalculateDiff(model);
            model.DiffResult = diffResult;
        }

        await diffRepository.Update(model);

        return created;
    }
    
    /// <summary>
    /// Decode the input base64 string to string
    /// </summary>
    /// <param name="diffRequest">Request containing base64 input </param>
    /// <returns>Decoded string from base64</returns>
    private static string DecodeInput(DiffRequest diffRequest) =>
        Encoding.UTF8.GetString(Convert.FromBase64String(diffRequest.Input));
}