using System;
using System.Web.Mvc;
using NHibernate;
using Ninject;
using Web.Controllers;

namespace Web.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class NHibernateActionFilterAttribute : ActionFilterAttribute
    {
        [Inject]
        public ISessionFactory SessionFactory { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var sessionController = filterContext.Controller as BaseController;

            if (sessionController == null)
                return;
            
            sessionController.Session.BeginTransaction();
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var sessionController = filterContext.Controller as BaseController;

            if (sessionController == null)
                return;

            using (var session = sessionController.Session)
            {
                if (session == null)
                    return;

                if (!session.Transaction.IsActive)
                    return;

                if (filterContext.Exception != null)
                    session.Transaction.Rollback();
                else
                    session.Transaction.Commit();
            }
        }
    }
}