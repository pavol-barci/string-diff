using StringDiff.Application.Helpers;
using StringDiff.Application.Models;
using StringDiff.Domain;
using StringDiff.Infrastructure;
using StringDiff.Objects;

namespace StringDiff.Application.Services;

public class DiffService(IDiffRepository diffRepository, IDiffCalculator diffCalculator, ILogger<DiffService> logger)
    : IDiffService
{

    public async Task<bool> UpsertLeft(int id, DiffRequest diffRequest)
    {
        return await UpsertInternal(id, diffModel => diffModel.Left = diffRequest.Input, () => new DiffModel(diffRequest.Input));
    }

    public async Task<bool> UpsertRight(int id, DiffRequest diffRequest)
    {
        return await UpsertInternal(id, diffModel => diffModel.Right = diffRequest.Input, () => new DiffModel(right: diffRequest.Input));
    }

    public async Task<DiffResultResponse?> GetDiff(int id)
    {
        var model = await diffRepository.GetById(id);
        
        //TODO> if model null => not found, if result null, not finished?
        if (model is null)
        {
            logger.LogWarning("Diff result with id {id} does not exists.", id);
            return null;
        }
        
        
        return model.DiffResult.ToDiffResultResponse();
    }

    private async Task<bool> UpsertInternal(int id, Action<DiffModel> updateAction, Func<DiffModel> createFunction)
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
}