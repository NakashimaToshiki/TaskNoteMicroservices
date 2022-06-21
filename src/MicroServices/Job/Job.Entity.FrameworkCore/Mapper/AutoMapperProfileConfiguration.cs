using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job.Entity.FrameworkCore.Mapper;

public class AutoMapperProfileConfiguration : Profile
{
    public AutoMapperProfileConfiguration()
    {
        CreateMap<JobModel, JobEntity>();
        CreateMap<JobEntity, JobModel>();
        CreateMap<UserModel, UserEntity>();
        CreateMap<UserEntity, UserModel>();
    }
}
