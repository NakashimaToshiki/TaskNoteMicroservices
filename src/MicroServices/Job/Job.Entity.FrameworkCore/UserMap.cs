using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Job.Entity.FrameworkCore;

public class UserMap : IEntityTypeConfiguration<UserEntity>
{

    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable("t_users");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(p => p.Name).HasColumnName("name");
        builder.Property(p => p.SexId).HasColumnName("sex_id");
    }
}
