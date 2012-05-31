using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using MvcMiniProfiler;
using MvcMiniProfiler.MVCHelpers;

[assembly: WebActivator.PreApplicationStartMethod(typeof(Web.App_Start.MiniProfilerPackage), "PreStart")]
[assembly: WebActivator.PostApplicationStartMethod(typeof(Web.App_Start.MiniProfilerPackage), "PostStart")]

namespace Web.App_Start
{
    public static class MiniProfilerPackage
    {
        public static void PreStart()
        {
            //Setup sql formatter
            MiniProfiler.Settings.SqlFormatter = new OracleFormatter();

            //Make sure the MiniProfiler handles BeginRequest and EndRequest
            DynamicModuleUtility.RegisterModule(typeof(MiniProfilerStartupModule));

            //Setup profiler for Controllers via a Global ActionFilter
            GlobalFilters.Filters.Add(new ProfilingActionFilter());

            //Settings            
            MiniProfiler.Settings.PopupShowTimeWithChildren = true;
            MiniProfiler.Settings.PopupShowTrivial = false;            

            //Ignore glimpse details in miniprofiler
            var ignored = MiniProfiler.Settings.IgnoredPaths.ToList();
            ignored.Add("Glimpse.axd");
            MiniProfiler.Settings.IgnoredPaths = ignored.ToArray();
        }

        public static void PostStart()
        {
            // Intercept ViewEngines to profile all partial views and regular views.
            // If you prefer to insert your profiling blocks manually you can comment this out
            var copy = ViewEngines.Engines.ToList();
            ViewEngines.Engines.Clear();
            foreach (var item in copy)
            {
                ViewEngines.Engines.Add(new ProfilingViewEngine(item));
            }
        }
    }

    public class MiniProfilerStartupModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.BeginRequest += (sender, e) => MiniProfiler.Start();

            //Profiling abadened if user is not in the admin role
            context.PostAuthorizeRequest += (sender, e) =>
            {
                if (!context.User.IsInRole("Admin"))
                    MiniProfiler.Stop(true);
            };

            context.EndRequest += (sender, e) => MiniProfiler.Stop();
        }

        public void Dispose() { }
    }
}

