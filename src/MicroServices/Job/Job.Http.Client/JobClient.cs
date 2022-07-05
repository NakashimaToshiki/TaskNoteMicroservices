using Job.Entity;
using Shared.Http;
using Microsoft.AspNetCore.Http;
using System.Net.Mime;

namespace Job.Http.Client;

public class JobClient : BaseApiClient<JobModel>
{
    private readonly string _scheme = "api/job";

    public JobClient(HttpClient client) : base(client)
    {
    }

    public async Task<JobModel?> GetById(int id)
    {
        return await Get($"{_scheme}/{id}");
    }

    public async Task<IEnumerable<JobModel>> Search(JobSearchModel searchModel)
    {
        return await Search($"{_scheme}/search", searchModel);
    }

    public async Task<JobModel?> Create(JobModel input)
    {
        return await Create($"{_scheme}", input);
    }

    public async Task<(JobModel?, bool)> CreateOrUpdate(JobModel input)
    {
        return await CreateOrUpdate($"{_scheme}", input);
    }
    public async Task<JobModel?> Update(JobModel input)
    {
        return await Update($"{_scheme}", input);
    }

    public async Task<bool> Delete(int id)
    {
        return await Delete($"{_scheme}/{id}");
    }

}
