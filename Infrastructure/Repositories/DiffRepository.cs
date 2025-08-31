using StringDiff.Domain;

namespace StringDiff.Infrastructure.Repositories;

public class DiffRepository : IDiffRepository
{
    public async Task<DiffModel?> GetById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<DiffModel> Update(DiffModel model)
    {
        throw new NotImplementedException();
    }

    public async Task<DiffModel> Create(DiffModel model)
    {
        throw new NotImplementedException();
    }
}