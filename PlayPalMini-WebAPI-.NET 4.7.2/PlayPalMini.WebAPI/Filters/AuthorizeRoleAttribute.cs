using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace PlayPalMini.WebAPI.Filters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class AuthorizeRoleAttribute : AuthorizeAttribute
    {
        private readonly string[] _roles;
        public AuthorizeRoleAttribute(params string[] roles)
        {
            _roles = roles;
        }
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var principal = actionContext.RequestContext.Principal as ClaimsPrincipal;

            if (principal != null)
            {
                if (_roles.Any(role => principal.HasClaim(ClaimTypes.Role, role)))
                {
                    return true;
                }
            }
            return false;
        }        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden,
                new { error = "It would seem your role does not have access." });
        }
    }
}