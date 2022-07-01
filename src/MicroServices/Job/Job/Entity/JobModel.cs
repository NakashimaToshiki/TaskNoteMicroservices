using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Job.Entity;

public record JobModel
{
    public int Id { get; set; }

    [Display(Name = "タイトル")]
    [StringLength(10)]
    public string Title { get; set; } = default!;

    public string Description { get; set; } = default!;

    public bool IsCompleted { get; set; } = false;

    [DisplayFormat(DataFormatString = "{0:G}")]
    public DateTime UpdateDate { get; set; } = default!;

    public DateTime CreatedDate { get; set; } = default!;

    [JsonPropertyName("user_id")]
    public string UserId { get; set; } = default!;
}
