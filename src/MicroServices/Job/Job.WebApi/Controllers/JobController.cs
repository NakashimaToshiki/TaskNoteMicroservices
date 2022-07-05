using Job.Http;

namespace Job.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobController : ControllerBase
{
    private readonly JobSession _session;

    public JobController(JobSession session)
    {
        _session = session ?? throw new ArgumentNullException(nameof(session));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobModel>> GetById(int id, [FromQuery] Header header)
    {
        var record = await _session.ReadById(id);
        if (record == null) return NotFound();
        return record;
    }

    [HttpPost("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<IEnumerable<JobModel>>> PostBySearch([FromBody] JobSearchModel search)
    {
        var records = await _session.Search(search);
        if (records == null) return NoContent();
        return Ok(records);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Post(JobModel input)
    {
        var record = await _session.Create(input);
        if (record == null) return Conflict();
        return CreatedAtAction(nameof(GetById), new { input.Id }, record);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Put(JobModel input)
    {
        var record = await _session.Create(input);
        if (record != null) return CreatedAtAction(nameof(GetById), new { input.Id }, record);

        record = await _session.Update(input);
        if (record == null) return Conflict();
        return CreatedAtAction(nameof(GetById), new { input.Id }, record);
    }

    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobModel>> Patch(JobModel input, [FromHeader] Header header)
    {
        var record = await _session.Update(input);
        if (record == null) return NotFound();
        else return CreatedAtAction(nameof(GetById), new { input.Id }, record);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await _session.DeleteAsync(id)) return NotFound();
        return NoContent();
    }

    /*
    [HttpGet("userid/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<IList<TaskShortModel>>> GetsByUserId(string id)
    {
        var records = await _session.GetTasksByUserId(id);
        if (records.Count == 0) return NoContent();
        return records.ToList();
    }*/
}
