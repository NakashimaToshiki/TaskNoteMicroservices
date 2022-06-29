using System.Text.Json.Serialization;

namespace Job.WebApi;

public class Header
{
    [FromHeader]
    public string UserId { get; set; } = string.Empty;
}
