namespace Job.Entity.FrameworkCore
{
    public class JobDbContext : DbContext
    {
        public DbSet<JobEntity> Jobs => Set<JobEntity>();

        public DbSet<UserEntity> Users => Set<UserEntity>();

        public JobDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new JobMap());
            modelBuilder.ApplyConfiguration(new UserMap());

            base.OnModelCreating(modelBuilder);
        }

    }
}