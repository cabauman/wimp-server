using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace WIMP_Server.Models.Users;

public class User : IdentityUser
{
    public ICollection<InvitationKey> InvitationKeys { get; set; }
}
