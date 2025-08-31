namespace StringDiff.Application;

public interface IDiffService
{
    Task<bool> UpsertLeft(int id, string left);
    Task<bool> UpsertRight(int id, string right);
    Task GetDiff(int id);
}