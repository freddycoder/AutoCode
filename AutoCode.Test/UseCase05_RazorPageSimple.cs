using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace AutoCode.Test
{
    public class UseCase05_RazorPageSimple
    {
        [Fact]
        public async Task AddCommentToFile()
        {
            var constance_dot_cs = Path.Combine("CodeExample", "UseCase05.cs");

            Automaticly.Add.Comment.To.File(constance_dot_cs);

            var result = await File.ReadAllTextAsync(constance_dot_cs);

            Assert.Equal(Expected, result);
        }

        [Fact]
        public async Task AddCommentToCode()
        {
            var constance_dot_cs = Path.Combine("CodeExample", "UseCase05.cs");

            var result = Automaticly.Add.Comment.To.Code(await File.ReadAllTextAsync(constance_dot_cs));

            Assert.Equal(Expected, result);
        }

        private const string Expected =
@"using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace AzureDevOpsTeamMembersVelocity.Areas.Identity.Pages.Account.Manage
{
    /// <summary>
    /// The change password page model
    /// </summary>
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;

        /// <summary>
        /// Constructor with dependencies
        /// </summary>
        public ChangePasswordModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<ChangePasswordModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        /// <summary>
        /// Append on GET of the change password page
        /// </summary>
        public void OnGet()
        {

        }
    }
}
";
    }
}
