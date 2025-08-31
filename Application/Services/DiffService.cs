using System.Security.Cryptography;
using StringDiff.Domain;
using StringDiff.Infrastructure;

namespace StringDiff.Application.Services;

public class DiffService(IDiffRepository diffRepository, IDiffCalculator diffCalculator, ILogger<DiffService> logger)
    : IDiffService
{

    public async Task<bool> UpsertLeft(int id, string left)
    {
        return await UpsertInternal(id, diffModel => diffModel.Left = left, () => new DiffModel(left));
    }

    public async Task<bool> UpsertRight(int id, string right)
    {
        return await UpsertInternal(id, diffModel => diffModel.Right = right, () => new DiffModel(right: right));
    }

    public async Task GetDiff(int id)
    {
        var model = await diffRepository.GetById(id);
        
        //TODO> if model null => not found, if result null, not finished? 
        
        // return model.DiffResult.ToDiffResultResponse();
        
    }

    private async Task<bool> UpsertInternal(int id, Action<DiffModel> updateAction, Func<DiffModel> createFunction)
    {
        var model = await diffRepository.GetById(id);
        var created = false;
        
        if (model == null)
        {
            created = true;
            model = createFunction();
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

public class DiffResultResponse
{
    public string? Result { get; set; }

    public DiffResultResponse(string? result)
    {
        Result = result;
    }
}