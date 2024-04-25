using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

public class LoggedinOnly : ActionFilterAttribute, IAuthorizationFilter
{
    private IHttpContextAccessor _httpContextAccessor;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Retrieve IHttpContextAccessor from the service provider
        var serviceProvider = context.HttpContext.RequestServices;
        _httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();

        var accessToken = _httpContextAccessor.HttpContext.Session.GetString("AccessToken");

        if (string.IsNullOrEmpty(accessToken))
        {
            // If the access token is not present in the session, redirect to the login page
            context.Result = new RedirectToActionResult("Login", "Account", null);
        }
        return;
    }
}
