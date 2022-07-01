using Microsoft.AspNetCore.Mvc;
using Job.Entity;
using Shared.Http;

namespace TaskNote.Bff.Controllers;

[ApiController]
[Route("[controller]")]
public class JobController : ControllerBase
{
    private readonly ILogger<JobController> _logger;
    private readonly JobClient _client;

    public JobController(ILogger<JobController> logger, JobClient client)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<JobModel>> Get(int id)
    {
        return await _client.GetById(id);
    }

    [HttpPost()]
    public async Task<IActionResult> Post(JobModel input)
    {
        try
        {
            var result = await _client.Post(input);
            if (result) return Ok();
            else return Forbid();
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.BadResponse);
        }
    }
}
