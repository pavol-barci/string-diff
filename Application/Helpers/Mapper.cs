using StringDiff.Contracts;
using StringDiff.Domain.Models;
using DiffResult = StringDiff.Domain.Models.DiffResult;

namespace StringDiff.Application.Helpers;

public static class Mapper
{
    /// <summary>
    /// Transform domain object into application (DTO) object
    /// </summary>
    /// <param name="diffResult"></param>
    /// <returns>Web API response DTO</returns>
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