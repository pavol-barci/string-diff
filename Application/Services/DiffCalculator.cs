using StringDiff.Domain;

namespace StringDiff.Application.Services;

public class DiffCalculator : IDiffCalculator
{
    public async Task<DiffResult> CalculateDiff(DiffModel model)
    {
        //TODO> implement;
        await Task.CompletedTask;
        model.DiffResult = new DiffResult
        {
            Result = $"Calculated at {DateTime.UtcNow}"
        };
        
        return new DiffResult();
    }
}