using Microsoft.AspNetCore.Mvc;
using StringDiff.Application;
using StringDiff.Objects;

namespace StringDiff.Controllers;

[ApiController]
[Route("[controller]/v1")]
public class DiffV1Controller(IDiffService diffService, ILogger<DiffV1Controller> logger) : ControllerBase
{
    [HttpPut("{id:int}/left")]
    public async Task<IActionResult> AddLeftString([FromQuery] int id, [FromBody] DiffRequest diffRequest)
    {
        var created = await diffService.UpsertLeft(id, diffRequest);

        if (created)
        {
            return Created();
        }
        
        return Ok();
    }

    [HttpPut("{id:int}/right")]
    public async Task<IActionResult> AddRightString([FromQuery] int id, [FromBody] DiffRequest diffRequest)
    {
        var created = await diffService.UpsertRight(id, diffRequest);

        if (created)
        {
            return Created();
        }
        
        return Ok();
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetDiffResult([FromQuery] int id)
    {
        var result = await diffService.GetDiff(id);

        //TODO> here or exception?
        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}