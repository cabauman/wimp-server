using System.Collections.Generic;
using WIMP_Server.Models.Users;

namespace WIMP_Server.Data.Users;

public interface IUserRepository
{
    void CreateInvitationKey(InvitationKey invitationKey);

    InvitationKey GetInvitationKeyWithId(int id);

    IEnumerable<InvitationKey> GetInvitationKeysByUserId(string userId);

    IEnumerable<InvitationKey> GetAllInvitationKeys();

    InvitationKey FindInvitationKey(string key);

    IEnumerable<User> GetUsers();

    IEnumerable<User> GetPaginatedUsers(int skip, int count);

    bool SaveChanges();
}
