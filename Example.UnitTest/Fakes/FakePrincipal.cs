using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;

namespace Example.UnitTest.Fakes
{
    public class FakePrincipal : IPrincipal
    {
        private readonly IIdentity _identity;
        private readonly string[] _roles;

        public FakePrincipal(IIdentity identity, string[] roles)
        {
            _identity = identity;
            _roles = roles;
        }


        public IIdentity Identity
        {
            get { return _identity; }
        }

        public bool IsInRole(string role)
        {
            return _roles != null && _roles.Contains(role);
        }
    }
}
