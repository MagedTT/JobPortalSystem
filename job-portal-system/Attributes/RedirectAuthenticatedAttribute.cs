using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace job_portal_system.Attributes
{
    public class RedirectAuthenticatedAttribute : ActionFilterAttribute
    {
        private readonly string _action;
        private readonly string _controller;

        public RedirectAuthenticatedAttribute(string action = "Index", string controller = "Home")
        {
            _action = action;
            _controller = controller;
        }

        [HttpPost]
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;
            if (user?.Identity != null && user.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToActionResult(_action, _controller, null);
            }
        }
    }
}
