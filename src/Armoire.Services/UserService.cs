using System;
using Armoire.Common;
using Armoire.Entities;
using NHibernate;

namespace Armoire.Services
{
    public class UserService: IUserService
    {
        private readonly ISession _nhSession;

        public UserService(ISession nhSession)
        {
            _nhSession = nhSession;
        }

        public PatronDto Register(PatronDto p)
        {
            var newUser = new Patron()
            {
                FirstName = p.FirstName,
                LastName = p.LastName,
                Username = p.Username,
                Password = p.Password,
                Email = p.Email,
                Phone = p.Phone,
                DateCreated = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };
            _nhSession.Save(newUser);
            p.Id = newUser.Id;
            return p;
        }
    }
}
