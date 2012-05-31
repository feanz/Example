using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Web.Helpers
{
    /// <summary>
    /// Custom implmentation of IIdentity that uses an IMembershipProvider 
    /// To Validate user and check permissions.  This uses a windows authentication system with seperate store to manage roles
    /// </summary>
    [Serializable]
    [ComVisible(true)]
    public class CustomIdentity : MarshalByRefObject, IIdentity
    {
        private string _name;
        private readonly bool _authenticated;

        public CustomIdentity()
        {}

        public CustomIdentity(System.Web.Security.FormsAuthenticationTicket ticket, bool auth, List<string> roles)
        {
            var split = ticket.Name.Split('\\');
            _name = split.Length == 1 ? split[0] : split[1];
            _authenticated = auth;
            Roles = roles;
        }

        public CustomIdentity(string name, bool auth, List<string> roles)
        {
            var split = name.Split('\\');
            _name = split.Length == 1 ? split[0] : split[1];
            _authenticated = auth;
            Roles = roles;
        }

        public List<string> Roles { get; set; }

        #region IIdentity Members
        public string AuthenticationType
        {
            get { return "Custom"; }
        }

        public bool IsAuthenticated
        {
            get { return _authenticated; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        #endregion
    }
}
