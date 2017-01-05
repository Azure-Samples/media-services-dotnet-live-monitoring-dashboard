using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;

namespace MediaDashboard.Operations.Api
{
    public class AuthorizeAttribute : System.Web.Http.AuthorizeAttribute
    {
        // Work around to convert failed authorizations to 403 instead of 302 redirect.
        protected override void HandleUnauthorizedRequest(HttpActionContext context)
        {
            base.HandleUnauthorizedRequest(context);
            // context.Response = context.Request.CreateResponse(HttpStatusCode.Forbidden, new { Message = "You are not authorized!" });
        }
    }
}
