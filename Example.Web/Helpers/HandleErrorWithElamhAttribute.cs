﻿using System;
using System.Web;
using System.Web.Mvc;
using Elmah;

namespace Web.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	public sealed class HandleErrorWithElmahAttribute : HandleErrorAttribute
	{
		public override void OnException(ExceptionContext filterContext)
		{
			base.OnException(filterContext);

			var e = filterContext.Exception;
			if (!filterContext.ExceptionHandled   // if unhandled, will be logged anyhow
					|| RaiseErrorSignal(e)      // prefer signaling, if possible
					|| IsFiltered(filterContext))     // filtered?
				return;

			LogException(e);
		}

		private static bool RaiseErrorSignal(Exception e)
		{
			var context = HttpContext.Current;
			if (context == null)
				return false;
			var signal = ErrorSignal.FromContext(context);
			if (signal == null)
				return false;
			signal.Raise(e, context);
			return true;
		}

		private static bool IsFiltered(ExceptionContext context)
		{
			var config = context.HttpContext.GetSection("elmah/errorFilter")
									 as ErrorFilterConfiguration;

			if (config == null)
				return false;

			var testContext = new ErrorFilterModule.AssertionHelperContext(
																context.Exception, HttpContext.Current);

			return config.Assertion.Test(testContext);
		}
		
        public static void LogException(Exception e)
        {
            var context = HttpContext.Current;
            ErrorLog.GetDefault(context).Log(new Error(e, context));            
        }
	}
}
