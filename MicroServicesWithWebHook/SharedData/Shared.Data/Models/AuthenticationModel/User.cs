using Microsoft.AspNetCore.Identity;

namespace Shared.Data.Models.AuthenticationModel
{
    public class User : IdentityUser
    {
        /* IdentityUser automatically provides Id (as a string), UserName, Email, etc.
        *  It also handles password hashing automatically.
        */
        public string Role { get; set; } = "User";
    }
}
