using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web.Mvc;

namespace Example.UnitTest
{
    public static class MethodInfoExtensions
    {
        /// <summary>
        /// Will return the name of the action specified in the ActionNameAttribute for a method if it has an ActionNameAttribute.
        /// Will return the name of the method otherwise.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static string ActionName(this MethodInfo method)
        {
            if (method.IsDecoratedWith<ActionNameAttribute>()) return method.GetAttribute<ActionNameAttribute>().Name;

            return method.Name;
        }
    }
}
