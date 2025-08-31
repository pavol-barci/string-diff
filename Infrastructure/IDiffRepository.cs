using StringDiff.Domain;

namespace StringDiff.Infrastructure;

public interface IDiffRepository
{
    Task<DiffModel?> GetById(int id);
    Task<DiffModel> Update(DiffModel model);
    Task<DiffModel> Create(DiffModel model);
}