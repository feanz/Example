using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftwareApproach.TestingExtensions;
using System.Reflection;
using System.Web;
using Example.UnitTest.Fakes;
using System.Collections.Specialized;
using Moq;

namespace Example.UnitTest
{
    public static class RouteExtensions
    {
        /// <summary>
        /// A way to start the fluent interface and and which method to use
        /// since you have a method constraint in the route.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="httpMethod"></param>
        /// <returns></returns>
        public static RouteData WithMethod(this string url, string httpMethod)
        {
            return Route(url, httpMethod);
        }

        /// <summary>
        /// A way to start the fluent interface and and which method to use
        /// since you have a method constraint in the route.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="verb"></param>
        /// <returns></returns>
        public static RouteData WithMethod(this string url, HttpVerbs verb)
        {
            return WithMethod(url, verb.ToString("g"));
        }

        /// <summary>
        /// Find the route for a URL and an Http Method
        /// because you have a method contraint on the route 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="httpMethod"></param>
        /// <returns></returns>
        public static RouteData Route(string url, string httpMethod)
        {
            var context = FakeHttpContext(url, httpMethod);
            return RouteTable.Routes.GetRouteData(context);
        }

        /// <summary>
        /// Returns the corresponding route for the URL.  Returns null if no route was found.
        /// </summary>
        /// <param name="url">The app relative url to test.</param>
        /// <returns>A matching <see cref="RouteData" />, or null.</returns>
        public static RouteData Route(this string url)
        {
            var context = FakeHttpContext(url);
            return RouteTable.Routes.GetRouteData(context);
        }

        /// <summary>
        /// Asserts that the route matches the expression specified.  Checks controller, action, and any method arguments
        /// into the action as route values.
        /// </summary>
        /// <typeparam name="TController">The controller.</typeparam>
        /// <param name="routeData">The routeData to check</param>
        /// <param name="action">The action to call on TController.</param>
        public static RouteData ShouldMapTo<TController>(this RouteData routeData, Expression<Func<TController, ActionResult>> action)
            where TController : Controller
        {
            routeData.ShouldNotBeNull("The URL did not match any route");

            //check controller
            routeData.ShouldMapTo<TController>();

            //check action
            var methodCall = (MethodCallExpression)action.Body;
            string actualAction = routeData.Values.GetValue("action").ToString();
            string expectedAction = methodCall.Method.ActionName();
            actualAction.ShouldEqual(expectedAction);

            //check parameters
            for (int i = 0; i < methodCall.Arguments.Count; i++)
            {
                ParameterInfo param = methodCall.Method.GetParameters()[i];
                bool isReferenceType = !param.ParameterType.IsValueType;
                bool isNullable = isReferenceType ||
                    (param.ParameterType.UnderlyingSystemType.IsGenericType && param.ParameterType.UnderlyingSystemType.GetGenericTypeDefinition() == typeof(Nullable<>));

                string controllerParameterName = param.Name;
                bool routeDataContainsValueForParameterName = routeData.Values.ContainsKey(controllerParameterName);
                object actualValue = routeData.Values.GetValue(controllerParameterName);
                object expectedValue = null;
                Expression expressionToEvaluate = methodCall.Arguments[i];

                // If the parameter is nullable and the expression is a Convert UnaryExpression, 
                // we actually want to test against the value of the expression's operand.
                if (expressionToEvaluate.NodeType == ExpressionType.Convert
                    && expressionToEvaluate is UnaryExpression)
                {
                    expressionToEvaluate = ((UnaryExpression)expressionToEvaluate).Operand;
                }

                switch (expressionToEvaluate.NodeType)
                {
                    case ExpressionType.Constant:
                        expectedValue = ((ConstantExpression)expressionToEvaluate).Value;
                        break;

                    case ExpressionType.New:
                    case ExpressionType.MemberAccess:
                        expectedValue = Expression.Lambda(expressionToEvaluate).Compile().DynamicInvoke();
                        break;
                }

                if (isNullable && (string)actualValue == String.Empty && expectedValue == null)
                {
                    // The parameter is nullable so an expected value of '' is equivalent to null;
                    continue;
                }

                // HACK: this is only sufficient while System.Web.Mvc.UrlParameter has only a single value.
                if (actualValue == UrlParameter.Optional ||
                    (actualValue != null && actualValue.ToString().Equals("System.Web.Mvc.UrlParameter")))
                {
                    actualValue = null;
                }

                if (expectedValue is DateTime)
                {
                    actualValue = Convert.ToDateTime(actualValue);
                }
                else
                {
                    expectedValue = (expectedValue == null ? expectedValue : expectedValue.ToString());
                }

                string errorMsgFmt = "Value for parameter '{0}' did not match: expected '{1}' but was '{2}'";
                if (routeDataContainsValueForParameterName)
                {
                    errorMsgFmt += ".";
                }
                else
                {
                    errorMsgFmt += "; no value found in the route context action parameter named '{0}' - does your matching route contain a token called '{0}'?";
                }

                actualValue.ShouldEqual(expectedValue, String.Format(errorMsgFmt, controllerParameterName, expectedValue, actualValue));
            }

            return routeData;
        }

        /// <summary>
        /// Converts the URL to matching RouteData and verifies that it will match a route with the values specified by the expression.
        /// </summary>
        /// <typeparam name="TController">The type of controller</typeparam>
        /// <param name="relativeUrl">The ~/ based url</param>
        /// <param name="action">The expression that defines what action gets called (and with which parameters)</param>
        /// <returns></returns>
        public static RouteData ShouldMapTo<TController>(this string relativeUrl, Expression<Func<TController, ActionResult>> action) where TController : Controller
        {
            return relativeUrl.Route().ShouldMapTo(action);
        }

        /// <summary>
        /// Verifies the <see cref="RouteData">routeData</see> maps to the controller type specified.
        /// </summary>
        /// <typeparam name="TController"></typeparam>
        /// <param name="routeData"></param>
        /// <returns></returns>
        public static RouteData ShouldMapTo<TController>(this RouteData routeData) where TController : Controller
        {
            //strip out the word 'Controller' from the type
            string expected = typeof(TController).Name;
            if (expected.EndsWith("Controller"))
            {
                expected = expected.Substring(0, expected.LastIndexOf("Controller"));
            }

            //get the key (case insensitive)
            string actual = routeData.Values.GetValue("controller").ToString();

            actual.ShouldEqual(expected);
            return routeData;
        }

        /// <summary>
        /// Gets a value from the <see cref="RouteValueDictionary" /> by key.  Does a
        /// case-insensitive search on the keys.
        /// </summary>
        /// <param name="routeValues"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object GetValue(this RouteValueDictionary routeValues, string key)
        {
            foreach (var routeValueKey in routeValues.Keys)
            {
                if (string.Equals(routeValueKey, key, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (routeValues[routeValueKey] == null)
                        return null;
                    return routeValues[routeValueKey].ToString();
                }
            }

            return null;
        }

        private static HttpContextBase FakeHttpContext(string url, string method)
        {
            var httpMethod = (HttpVerbs)Enum.Parse(typeof(HttpVerbs), method);
            return FakeHttpContext(url, httpMethod, null);
        }

        private static HttpContextBase FakeHttpContext(string url, HttpVerbs? httpMethod)
        {
            return FakeHttpContext(url, httpMethod,null);
        }

        private static HttpContextBase FakeHttpContext(string url)
        {
            return FakeHttpContext(url,null,null);
        }

        private static HttpContextBase FakeHttpContext(string url, HttpVerbs? httpMethod, HttpVerbs? formMethod)
        {
            NameValueCollection form = null;

            if (formMethod.HasValue)
                form = new NameValueCollection { { "_method", formMethod.Value.ToString().ToUpper() } };

            if (!httpMethod.HasValue)
                httpMethod = HttpVerbs.Get;

            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.AppRelativeCurrentExecutionFilePath).Returns(url);
            request.SetupGet(x => x.PathInfo).Returns(string.Empty);
            request.SetupGet(x => x.Form).Returns(form);
            request.SetupGet(x => x.HttpMethod).Returns(httpMethod.Value.ToString().ToUpper());

            var context = new FakeHttpContext(url);
            context.SetRequest(request.Object);

            return context;
        }
    }
}
