using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using StringDiff.Application.Abstraction.Services;
using StringDiff.Contracts;

namespace StringDiff.Controllers;

[ApiController]
[Route("diff/v1")]
public class DiffV1Controller(IDiffService diffService, ILogger<DiffV1Controller> logger) : ControllerBase
{
    [HttpPut("{id:guid}/left")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> AddLeftString([FromRoute] Guid id, [FromBody] DiffRequest diffRequest)
    {
        var created = await diffService.UpsertLeft(id, diffRequest);
        return HandleUpsertResponse(created);
    }

    [HttpPut("{id:guid}/right")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> AddRightString([FromRoute] Guid id, [FromBody] DiffRequest diffRequest)
    {
        var created = await diffService.UpsertRight(id, diffRequest);
        return HandleUpsertResponse(created);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DiffResultResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetDiffResult([FromRoute] Guid id)
    {
        var result = await diffService.GetDiff(id);
        return Ok(result);
    }

    private IActionResult HandleUpsertResponse(bool created) => created ? StatusCode(StatusCodes.Status201Created) : Ok();
}