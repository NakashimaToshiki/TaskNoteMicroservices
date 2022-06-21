namespace Job.Entity.FrameworkCore;

public record JobEntity : JobModel
{
    public UserEntity? User { get; set; }
}
