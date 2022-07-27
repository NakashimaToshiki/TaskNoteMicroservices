using AutoMapper;
using Job.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job.Entity.FrameworkCore.Sessions;

public class JobSession
{
    private readonly IDbContextFactory<JobDbContext> _dbFactory;
    private readonly IMapper _mapper;

    public JobSession(IDbContextFactory<JobDbContext> dbFactory, IMapper mapper)
    {
        _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<JobModel?> ReadById(int id)
    {
        try
        {
            using var db = _dbFactory.CreateDbContext();
            var record = await db.Jobs.FirstOrDefaultAsync(r => r.Id == id);
            return _mapper.Map<JobModel>(record);
        }
        catch (Exception e)
        {
            throw new DatabaseException(e);
        }
    }

    public async Task<IEnumerable<JobModel>> Search(JobSearchModel search)
    {
        try
        {
            using var db = _dbFactory.CreateDbContext();
            var records = await db.Jobs.Where(r => r.IsCompleted == search.IsCompleted).Skip(search.SkipCount).Take(search.TakeCount).ToListAsync();
            return _mapper.Map<IEnumerable<JobModel>>(records);
        }
        catch (Exception e)
        {
            throw new DatabaseException(e);
        }
    }

    public async Task<JobModel?> Create(JobModel input)
    {
        try
        {
            using var db = _dbFactory.CreateDbContext();
            var find = await db.Jobs.FindAsync(input.Id);
            if (find != null) return null;
            var record = _mapper.Map<JobEntity>(input);
            db.Jobs.Add(record);
            await db.SaveChangesAsync();
            return _mapper.Map<JobModel>(record);
        }
        catch (Exception e)
        {
            throw new DatabaseException(e);
        }
    }

    public async Task<JobModel?> Update(JobModel input)
    {
        try
        {
            using var db = _dbFactory.CreateDbContext();
            var find = await db.Jobs.FirstOrDefaultAsync(r => r.UserId == input.UserId);
            if (find == null) return null;

            find.Description = input.Description;
            find.IsCompleted = input.IsCompleted;
            find.UpdateDate = DateTime.Now;
            await db.SaveChangesAsync();
            return _mapper.Map<JobModel>(find);
        }
        catch (Exception e)
        {
            throw new DatabaseException(e);
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            using var db = _dbFactory.CreateDbContext();
            var find = await db.Jobs.FindAsync(id);
            if (find == null) return false;
            db.Remove(find);
            await db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            throw new DatabaseException(e);
        }
    }
}
