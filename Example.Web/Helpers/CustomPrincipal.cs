using System.Collections.Generic;
using System.Security.Principal;

namespace Web.Helpers
{
    public class CustomPrincipal : IPrincipal
    {
        readonly CustomIdentity _identity;
        readonly List<string> _roles;
        
        public CustomPrincipal(CustomIdentity identity)
        {
            _roles = identity.Roles;            
            _identity = identity;
        }

        #region IPrincipal Members
       
        public IIdentity Identity
        {
            get { return _identity; }
        }

        public bool IsInRole(string role)
        {
            return (_roles.Contains(role));
        }

        #endregion
    }
}