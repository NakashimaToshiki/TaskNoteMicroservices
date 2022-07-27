using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Job.Http;
using System.Linq;

namespace Job.WebApp.Pages;

public class SearchModel : PageModel
{
    private readonly JobSession _session;

    /// <summary>
    /// éÊìæç≈ëÂåèêî
    /// </summary>
    private int takeCount = 10;

    public List<JobModel> JobModels { get; set; } = new List<JobModel>();

    public SearchModel(JobSession session)
    {
        _session = session ?? throw new ArgumentNullException(nameof(session));
    }

    public async Task OnGet([FromQuery(Name = "p")] int page)
    {
        JobModels = (await _session.Search(new JobSearchModel()
        {
            IsCompleted = false,
            TakeCount = takeCount,
            SkipCount = page * takeCount,
        })).ToList();
    }
}
