using Microsoft.AspNetCore.Mvc;
using Job.Entity;
using Shared.Http;
using Job.Http;

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

    /*
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobModel>> GetById(int id)
    {
        var record = await _client.GetById(id);
        if (record == null) return NotFound();
        return record;
    }

    [HttpPost()]
    public async Task<IActionResult> Post(JobModel input)
    {
        try
        {
            var result = await _client.Create(input);
            if (result) return Ok();
            else return Forbid();
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.BadResponse);
        }
    }*/

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobModel>> GetById(int id)
    {
        var record = await _client.GetById(id);
        if (record == null) return NotFound();
        return record;
    }

    [HttpPost("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<IEnumerable<JobModel>>> PostBySearch([FromBody] JobSearchModel search)
    {
        var records = await _client.Search(search);
        if (records == null) return NoContent();
        return Ok(records);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Post(JobModel input)
    {
        var record = await _client.Create(input);
        if (record == null) return Conflict();
        return CreatedAtAction(nameof(GetById), new { input.Id }, record);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Put(JobModel input)
    {
        var record = await _client.CreateOrUpdate(input);
        if (record.Item1 != null) return CreatedAtAction(nameof(GetById), new { input.Id }, record.Item1);
        else return Conflict();
    }

    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobModel>> Patch(JobModel input)
    {
        var record = await _client.Update(input);
        if (record == null) return NotFound();
        else return CreatedAtAction(nameof(GetById), new { input.Id }, record);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await _client.Delete(id)) return NotFound();
        return NoContent();
    }
}
