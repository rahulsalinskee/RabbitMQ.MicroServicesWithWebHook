using Microsoft.AspNetCore.Identity;

namespace Shared.Data.Models.AuthenticationModel
{
    public class User : IdentityUser
    {
        public new int Id { get; set; }

        public new string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = "User";
    }
}
