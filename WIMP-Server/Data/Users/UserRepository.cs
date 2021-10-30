using System;
using System.Collections.Generic;
using System.Linq;
using WIMP_Server.Models.Users;

namespace WIMP_Server.Data.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly WimpDbContext _dbContext;

        public UserRepository(WimpDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void CreateInvitationKey(InvitationKey invitationKey)
        {
            if (invitationKey == null)
            {
                throw new NullReferenceException(nameof(invitationKey));
            }

            _dbContext.InvitationKeys.Add(invitationKey);
        }

        public InvitationKey FindInvitationKey(string key)
        {
            return _dbContext.InvitationKeys.FirstOrDefault(ik => ik.Key == key);
        }

        public IEnumerable<InvitationKey> GetAllInvitationKeys()
        {
            return _dbContext.InvitationKeys
                .AsEnumerable();
        }

        public IEnumerable<User> GetUsers()
        {
            return _dbContext.Users
                .AsEnumerable();
        }

        public IEnumerable<User> GetPaginatedUsers(int skip, int count)
        {
            return _dbContext.Users
                .Skip(skip)
                .Take(count)
                .AsEnumerable();
        }

        public IEnumerable<InvitationKey> GetInvitationKeysByUserId(string userId)
        {
            return _dbContext.InvitationKeys
                .Where(ik => ik.GeneratedByUserId == userId)
                .AsEnumerable();
        }

        public InvitationKey GetInvitationKeyWithId(int id)
        {
            return _dbContext.InvitationKeys
                .FirstOrDefault(ik => ik.Id == id);
        }

        public bool SaveChanges()
        {
            return _dbContext.SaveChanges() > 0;
        }
    }
}