using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using LibraryReporter.Services;

namespace LibraryReporter.Controllers.AuthAttributes
{
    public class IsModeratorAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var authService = context
                .HttpContext
                .RequestServices
                .GetRequiredService<AuthService>();
            if (!authService.IsLibraryModerator())
            {
                context.Result = new ForbidResult();
                return;
            }

            base.OnActionExecuted(context);
        }
    }
}
