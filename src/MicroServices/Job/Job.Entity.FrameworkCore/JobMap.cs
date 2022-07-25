using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Job.Entity.FrameworkCore;

public class JobMap : IEntityTypeConfiguration<JobEntity>
{
    public void Configure(EntityTypeBuilder<JobEntity> builder)
    {
        builder.ToTable("t_tasks");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(p => p.Title).HasColumnName("title");
        builder.Property(p => p.Description).HasColumnName("description");
        builder.Property(p => p.CreatedDate).HasColumnName("createdDate");
        builder.Property(p => p.UpdateDate).HasColumnName("updateData");

        builder
            .HasOne(task => task.User)
            .WithMany(user => user.Jobs)
            .HasForeignKey(user => user.UserId)
            ;

#if DEBUG

        builder.HasData(new JobEntity[]
        {
            new JobEntity()
            {
                Id = 1,
                Title = "はじめてのタスク",
                Description = "すべてのタスクです",
                IsCompleted = false,
                UpdateDate = DateTime.Now,
                CreatedDate = DateTime.Now,
                UserId = "nakashima"
            },
        });
#endif

    }
}
