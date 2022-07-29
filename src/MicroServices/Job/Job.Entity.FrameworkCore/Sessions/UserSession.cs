using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job.Entity.FrameworkCore.Sessions;

public class UserSession
{
    private readonly IDbContextFactory<JobDbContext> _dbFactory;
    private readonly IMapper _mapper;

    public UserSession(IDbContextFactory<JobDbContext> dbFactory, IMapper mapper)
    {
        _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<UserModel?> ReadById(string id)
    {
        try
        {
            using var db = _dbFactory.CreateDbContext();
            var record = await db.Users.FirstOrDefaultAsync(x => x.Id == id);
            return _mapper.Map<UserModel>(record);
        }
        catch (Exception e)
        {
            throw new DatabaseException(e);
        }
    }

    public async Task<UserModel?> Create(UserModel input)
    {
        try
        {
            using var db = _dbFactory.CreateDbContext();
            var find = await db.Users.FindAsync(input.Id);
            if (find != null) return null;
            var record = _mapper.Map<UserEntity>(input);
            db.Users.Add(record);
            await db.SaveChangesAsync();
            return _mapper.Map<UserModel>(record);
        }
        catch (Exception e)
        {
            throw new DatabaseException(e);
        }
    }

    public async Task<UserModel?> Update(UserModel input)
    {
        try
        {
            using var db = _dbFactory.CreateDbContext();
            var find = await db.Users.FirstOrDefaultAsync(r => r.Id == input.Id);
            if (find == null) return null;

            find.Name = input.Name;
            find.SexId = input.SexId;
            await db.SaveChangesAsync();
            return _mapper.Map<UserModel>(find);
        }
        catch (Exception e)
        {
            throw new DatabaseException(e);
        }
    }

    public async Task<bool> DeleteAsync(string id)
    {
        try
        {
            using var db = _dbFactory.CreateDbContext();
            var find = await db.Users.FindAsync(id);
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
