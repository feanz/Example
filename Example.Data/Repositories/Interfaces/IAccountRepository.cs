using System.Collections.Generic;
using Example.Core.Model;

namespace Example.Data.Repositories.Interfaces
{
    public interface IAccountRepository : IGenericRepository<User>
    {
        IEnumerable<Role> GetRoles();
        IEnumerable<Role> GetRoles(int[] roleIds);
    }
}
