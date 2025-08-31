using StringDiff.Application.Models;
using StringDiff.Objects;

namespace StringDiff.Application;

public interface IDiffService
{
    Task<bool> UpsertLeft(int id, DiffRequest request);
    Task<bool> UpsertRight(int id, DiffRequest request);
    Task<DiffResultResponse?> GetDiff(int id);
}