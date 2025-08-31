using StringDiff.Application.Models;
using StringDiff.Domain;

namespace StringDiff.Application.Helpers;

public static class Mapper
{
    public static DiffResultResponse ToDiffResultResponse(this DiffResult? diffResult)
    {
        var response = new DiffResultResponse(diffResult?.Result ?? null);
        
        return response;
    }
}