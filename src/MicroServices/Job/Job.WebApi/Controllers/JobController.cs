namespace Job.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobController : Controller
{
    private readonly JobSession _session;

    public JobController(JobSession session)
    {
        _session = session ?? throw new ArgumentNullException(nameof(session));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<JobModel>> GetById(int id)
    {
        var record = await _session.ReadById(id);
        if (record == null) return NoContent();
        return record;
    }

    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Patch(JobModel input)
    {
        if (!await _session.Update(input)) return NotFound();
        else return CreatedAtAction(nameof(GetById), new { input.Id }, input);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await _session.DeleteAsync(id)) return NoContent();
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

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Post(JobModel input)
    {
        if (!await _session.Create(input)) return Conflict();
        return CreatedAtAction(nameof(GetById), new { input.Id }, input);
    }
}
