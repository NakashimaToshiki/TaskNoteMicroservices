namespace Job.WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserSession _session;

    public UserController(UserSession session)
    {
        _session = session ?? throw new ArgumentNullException(nameof(session));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserModel>> GetById(string id)
    {
        var record = await _session.ReadById(id);
        if (record == null) return NotFound();
        return record;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Post(UserModel input)
    {
        var record = await _session.Create(input);
        if (record == null) return Conflict();
        return CreatedAtAction(nameof(GetById), new { input.Id }, record);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Put(UserModel input)
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
    public async Task<ActionResult<UserModel>> Patch(UserModel input)
    {
        var record = await _session.Update(input);
        if (record == null) return NotFound();
        else return CreatedAtAction(nameof(GetById), new { input.Id }, record);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        if (!await _session.DeleteAsync(id)) return NotFound();
        return NoContent();
    }
}
