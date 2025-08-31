using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using StringDiff.Application;
using StringDiff.Objects;

namespace StringDiff.Controllers;

[ApiController]
[Route("[controller]/v1")]
public class DiffV1Controller(IDiffService diffService, ILogger<DiffV1Controller> logger) : ControllerBase
{
    [HttpPut("{id:int}/left")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddLeftString([FromRoute] int id, [FromBody] DiffRequest diffRequest)
    {
        var created = await diffService.UpsertLeft(id, diffRequest);
        return HandleUpsertResponse(created);
    }

    [HttpPut("{id:int}/right")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddRightString([FromRoute] int id, [FromBody] DiffRequest diffRequest)
    {
        var created = await diffService.UpsertRight(id, diffRequest);
        return HandleUpsertResponse(created);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetDiffResult([FromRoute] int id)
    {
        var result = await diffService.GetDiff(id);

        //TODO> here or exception?
        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    private IActionResult HandleUpsertResponse(bool created) => created ? StatusCode(StatusCodes.Status201Created) : Ok();
}