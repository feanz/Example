using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Example.Core.Model;
using NHibernate;
using NHibernate.Linq;
using Web.Helpers;
using Web.Mappings;
using Web.Models;
using Utilities.Extensions;

namespace Web
{
    public class MvcApplication : HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorWithElmahAttribute());
            filters.Add(new NHibernateActionFilterAttribute());
            filters.Add(new CompressFilterAttribute());			
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //Use attribute based routing see action methods decorated with Route attribute
            RouteAttribute.MapDecoratedRoutes(routes);

            //Default route
            routes.MapRoute(
                "Default", // Route taskName
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new[]{"Web.Controllers"} // Parameter defaults
            );
        }

        public override void Init()
        {
            AuthenticateRequest += MvcApplication_AuthenticateRequest;                        
            base.Init();
        }

        protected void Application_Start()
        {
            //Clear view engines then re add razor this is done for perf reasons
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            //Setup all global filters and action filters
            RegisterGlobalFilters(GlobalFilters.Filters);

            //Register diff areas of site
            AreaRegistration.RegisterAllAreas();

            //Setup routing behaviour
            RegisterRoutes(RouteTable.Routes);

            //Setup application model binders
            AddModelBinders();

            //Setup auto mapper configurations
            AutoMapperConfiguration.ConfigureMappings();

            //Add example scheduled task
            //AddTask("DoStuff", DooStuff, 10);
        }

        private static void AddModelBinders()
        {
            ModelBinders.Binders.Add(typeof(Bookmark), new BookmarkModelBinder());
        }

        private static CacheItemRemovedCallback OnCacheRemove;

        private readonly Dictionary<string,Action> _scheduledTasks = new Dictionary<string, Action>();

        #region Example Scheduled task
        private static void DooStuff()
        {
            System.Diagnostics.Debug.WriteLine("Stuff:  "  + DateTime.Now.TimeOfDay);
        }

        private void AddTask(string taskName, Action task, int seconds)
        {
            OnCacheRemove = CacheItemRemoved;

            Action addedTask;
            if (!_scheduledTasks.TryGetValue(taskName, out addedTask))
                _scheduledTasks.Add(taskName, task);

            HttpRuntime.Cache.Insert(taskName, seconds, null, DateTime.Now.AddSeconds(seconds), Cache.NoSlidingExpiration,
                CacheItemPriority.NotRemovable, OnCacheRemove);
        }

        public void CacheItemRemoved(string k, object v, CacheItemRemovedReason r)
        {
            // do stuff here if it matches our taskname, like WebRequest
            var action = _scheduledTasks[k];

            action.Invoke();

            // re-add our task so it recurs
            AddTask(k, action, Convert.ToInt32(v));
        } 
        #endregion

        internal void MvcApplication_AuthenticateRequest(object sender, EventArgs e)
        {
            if (User.IsNotNull())
            {
                var cookieName = "Example-" + User.Identity.Name;
                var authCookie = Request.Cookies[cookieName];

                //If an auth cookie has already been created use it for the Principal and identitiy
                if (authCookie != null)
                {
                    //Get auth cookie
                    var authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                    //Create a custom identity based on auth cookie and user role and set as principal for thread                    
                    var id = new CustomIdentity(authTicket, true, authTicket.UserData.ToList());
                    Context.User = new CustomPrincipal(id);
                    Thread.CurrentPrincipal = Context.User;
                }
                else
                {
                    //Check request is domain authenticated for windows auth
                    if (Request.IsAuthenticated)
                    {
                        //Get current database session 
                        var session = DependencyResolver.Current.GetService<ISession>();

                        //Get user from datbase so we can get the users roles
                        var user = session.Query<User>().SingleOrDefault(x => x.UserName == User.Identity.Name.ToUserName());
                        CustomIdentity id;

                        if (user != null)
                        {
                            //Create identity from user details 
                            id = new CustomIdentity(User.Identity.Name, true, user.Roles.Select(x => x.RoleName).ToList());

                            if (id.IsAuthenticated)
                            {
                                //Create auth ticket encrypt and add to cookie
                                var authTicket = new
                                        FormsAuthenticationTicket(1, //version
                                        User.Identity.Name, // user taskName
                                        DateTime.Now,             //creation
                                        DateTime.Now.AddMinutes(30), //Expiration
                                        true, //Persistent
                                        id.Roles.ToCommaSeparatedString(),
                                        User.Identity.Name); //since Classic logins don't have a "Friendly Name"

                                var encTicket = FormsAuthentication.Encrypt(authTicket);
                                Response.Cookies.Add(new HttpCookie(cookieName, encTicket));
                            }
                        }
                        else
                        {
                            //User is not authenticated so create unauthed id with no roles
                            id = new CustomIdentity(User.Identity.Name, false, new List<string>());
                        }

                        //Create principal from identity and set for current thread
                        Context.User = new CustomPrincipal(id);
                        Thread.CurrentPrincipal = Context.User;
                    }
                }
            }
            else
            {
                //try and use basic auth if this fails this is used for the rest API may replace with token based system at some point. 
                var authHeader = Request.ServerVariables["HTTP_AUTHORIZATION"];
                if (authHeader.IsNotNull())
                {
                    if (authHeader.StartsWith("Basic ", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var authParams =
                            Encoding.Default.GetString(Convert.FromBase64String(authHeader.Substring("Basic ".Length)));
                        var arr = authParams.Split(':');
                        var username = arr[0];

                        //Get current database session 
                        var session = DependencyResolver.Current.GetService<ISession>();

                        //Get user from datbase so we can get the users roles
                        var user = session.Query<User>().SingleOrDefault(x => x.UserName == username);

                        //Create identity from user details 
                        if (user != null)
                        {
                            var id = new CustomIdentity(user.UserName, true, user.Roles.Select(x => x.RoleName).ToList());

                            //Create principal from identity and set for current thread
                            Context.User = new CustomPrincipal(id);
                        }
                        Thread.CurrentPrincipal = Context.User;
                    }
                }
            }
        }
    }
}