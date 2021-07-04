using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace AutoCode.Test
{
    public class UseCase06_FixExceptionRazorPageSimple
    {
        [Fact]
        public async Task AddCommentToFile()
        {
            var constance_dot_cs = Path.Combine("CodeExample", "UseCase06.cs");

            Automaticly.Add.Comment.To.File(constance_dot_cs);

            var result = await File.ReadAllTextAsync(constance_dot_cs);

            Assert.Equal(Expected, result);
        }

        [Fact]
        public async Task AddCommentToCode()
        {
            var constance_dot_cs = Path.Combine("CodeExample", "UseCase06.cs");

            var result = Automaticly.Add.Comment.To.Code(await File.ReadAllTextAsync(constance_dot_cs));

            Assert.Equal(Expected, result);
        }

        private const string Expected =
@"using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace AzureDevOpsTeamMembersVelocity.Areas.Identity.Pages.Account
{
    /// <summary>
    /// The confirm email change page model
    /// </summary>
    [AllowAnonymous]
    public class ConfirmEmailChangeModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        /// <summary>
        /// Constructors with dependencies
        /// </summary>
        public ConfirmEmailChangeModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [TempData]
        public string? StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string email, string code)
        {
            if (userId == null || email == null || code == null)
            {
                return RedirectToPage(""/Index"");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($""Unable to load user with ID '{userId}'."");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ChangeEmailAsync(user, email, code);
            if (!result.Succeeded)
            {
                StatusMessage = ""Error changing email."";
                return Page();
            }

            // In our UI email and user name are one and the same, so when we update the email
            // we need to update the user name.
            var setUserNameResult = await _userManager.SetUserNameAsync(user, email);
            if (!setUserNameResult.Succeeded)
            {
                StatusMessage = ""Error changing user name."";
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = ""Thank you for confirming your email change."";
            return Page();
        }
    }
}
";
    }
}
