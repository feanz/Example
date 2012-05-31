using System.Collections.Generic;
using System.Linq;
using Example.Data.Repositories.Interfaces;
using NHibernate;
using NHibernate.Linq;
using Example.Core.Model;

namespace Example.Data.Repositories
{
    public class AccountRepository : GenericRepository<User>, IAccountRepository
    {
        public AccountRepository()
            : this(null)
        {

        }

        public AccountRepository(ISession session)
            : base(session)
        {
        }

        public override IEnumerable<User> GetAll()
        {
            return Session.Query<User>().Fetch(x => x.Roles).ToList();
        }

        public IEnumerable<Role> GetRoles()
        {
            return Session.Query<Role>().ToList();
        }

        public IEnumerable<Role> GetRoles(int[] roleIds)
        {
            return Session.Query<Role>().Where(x => roleIds.Contains(x.Id));
        }
    }
}
