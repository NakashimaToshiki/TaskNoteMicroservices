namespace Job.Entity.FrameworkCore;

public record UserEntity : UserModel
{
    public List<JobEntity> Jobs { get; set; } = new List<JobEntity>();
}
