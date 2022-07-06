using Job.Entity;
using Shared.Http;
using Microsoft.AspNetCore.Http;
using System.Net.Mime;

namespace Job.Http.Client;

public class JobClient : BaseApiClient
{
    private readonly string _scheme = "api/job";

    public JobClient(HttpClient client) : base(client)
    {
    }

    public async Task<JobModel?> GetById(int id)
    {
        return await Get<JobModel>($"{_scheme}/{id}");
    }

    public async Task<IEnumerable<JobModel>> Search(JobSearchModel searchModel)
    {
        return await Search<JobModel, JobSearchModel>($"{_scheme}/search", searchModel);
    }

    public async Task<JobModel?> Create(JobModel input)
    {
        return await CreateEcho<JobModel, JobModel>($"{_scheme}", input);
    }

    public async Task<(JobModel?, bool)> CreateOrUpdate(JobModel input)
    {
        return await CreateOrUpdateEcho<JobModel, JobModel>($"{_scheme}", input);
    }

    public async Task<JobModel?> Update(JobModel input)
    {
        return await UpdateEcho<JobModel, JobModel>($"{_scheme}", input);
    }

    public async Task<bool> Delete(int id)
    {
        return await Delete($"{_scheme}/{id}");
    }

}
