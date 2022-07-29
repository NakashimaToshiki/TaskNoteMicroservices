using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Job.WebApp.Pages
{
    public class UsersModel : PageModel
    {
        private readonly UserSession _session;

        public UserModel UserModel { get; set; } = NullUserModel.Instance;

        public UsersModel(UserSession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public async Task OnGet(string userId)
        {
            UserModel = await _session.ReadById(userId) ?? NullUserModel.Instance;
        }

        public async Task OnPost(UserModel UserModel)
        {
            this.UserModel = await _session.Update(UserModel) ?? NullUserModel.Instance;
        }
    }
}
