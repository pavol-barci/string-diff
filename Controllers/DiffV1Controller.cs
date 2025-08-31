using Microsoft.AspNetCore.Mvc;
using StringDiff.Objects;

namespace StringDiff.Controllers;

[ApiController]
[Route("[controller]/v1")]
public class DiffV1Controller : ControllerBase
{
    private readonly ILogger<DiffV1Controller> _logger;

    public DiffV1Controller(ILogger<DiffV1Controller> logger)
    {
        _logger = logger;
    }

    [HttpPut("left")]
    public async Task<IActionResult> AddLeftString([FromBody] DiffRequest diffRequest)
    {
        await Task.CompletedTask;
        return Ok(diffRequest);
    }

    [HttpPut("right")]
    public async Task<IActionResult> AddRightString([FromBody] DiffRequest diffRequest)
    {
        await Task.CompletedTask;
        return Ok(diffRequest);
    }

    [HttpGet]
    public async Task<IActionResult> GetDiffResult()
    {
        await Task.CompletedTask;
        return Ok();
    }
}