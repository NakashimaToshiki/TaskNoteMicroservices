using System.Text.Json.Serialization;

namespace Job.WebApp;

public class Header
{
    [FromHeader]
    public string UserId { get; set; } = string.Empty;
}
