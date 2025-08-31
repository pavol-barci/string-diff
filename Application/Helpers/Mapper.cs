using StringDiff.Application.Models;
using StringDiff.Domain;
using DiffResult = StringDiff.Domain.DiffResult;

namespace StringDiff.Application.Helpers;

public static class Mapper
{
    public static DiffResultResponse ToDiffResultResponse(this DiffResult? diffResult)
    {
        if (diffResult is null)
        {
            return new DiffResultResponse(DiffResultResponseType.NotFinished);
        }

        var response = diffResult.Result switch
        {
            DiffResultType.Equals => new DiffResultResponse(DiffResultResponseType.Equal),
            DiffResultType.DifferentSizes => new DiffResultResponse(DiffResultResponseType.DifferentSize),
            DiffResultType.NotEquals => new DiffResultResponse(DiffResultResponseType.NotEquals,
                new Difference(diffResult.Offset, diffResult.Length)),
            _ => new DiffResultResponse(DiffResultResponseType.NotFinished)
        };
        
        return response;
    }
}