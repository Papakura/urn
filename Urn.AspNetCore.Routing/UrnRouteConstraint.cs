using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Urn.AspNetCore.Routing
{
    /// <summary>
    ///     A WEB API Http route constraint
    /// </summary>
    /// <example>
    /// services.Configure{RouteOptions}(routeOptions =>
    /// {
    ///  routeOptions.ConstraintMap.Add(System.Urn.Urn.Prefix, typeof(UrnRouteConstraint));
    /// });
    /// ...
    /// *Use in controller action*
    /// [HttpPost()]
    /// [Route("[action]/{id:urn}")]
    /// public async  Task{IActionResult} UrnInRoute(string id)
    /// {
    ///  Urn urn = new Urn(id,UriFormat.UriEscaped);
    /// }
    /// </example>
    public class UrnRouteConstraint : IRouteConstraint
    {
        /// <inheritdoc />
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values,
            RouteDirection routeDirection)
        {
            if (values.TryGetValue(routeKey, out var routeValue))
            {
                if (routeValue is string value)
                {
                    return System.Urn.Urn.IsWellFormedUrnString(value, UriFormat.Unescaped);
                }

            }

            return false;
        }
    }
}