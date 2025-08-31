using StringDiff.Domain;
using StringDiff.Infrastructure.Exceptions;

namespace StringDiff.Infrastructure.Repositories.InMemory;

public class DiffInMemoryRepository : IDiffRepository
{
    private Dictionary<int, DiffModel> Data { get; set; } = new();


    public Task<DiffModel?> GetById(int id)
    {
        return Data.TryGetValue(id, out var model) is false
            ? Task.FromResult<DiffModel?>(null)
            : Task.FromResult(model);
    }

    public Task<DiffModel> Update(DiffModel model)
    {
        if (Data.ContainsKey(model.Id) is false)
        {
            throw new NotFoundException($"Model with id {model.Id} does not exists.");
        }
        
        Data[model.Id] = model;

        return Task.FromResult(model);
    }

    public Task<DiffModel> Create(DiffModel model)
    {
        var nextKey = Data.Keys.Max() + 1;
        model.Id = nextKey;
        
        if (Data.TryAdd(model.Id, model) is false)
        {
            throw new ConflictException($"Model with id {model.Id} already exists");
        }

        return Task.FromResult(model);
    }
}