using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;
using TarokScoreBoard.Infrastructure.Services;

namespace TarokScoreBoard.Api.Filters
{
  public class AuthorizeAttribute : TypeFilterAttribute
  {
    public AuthorizeAttribute() : base(typeof(AuthorizationFilter))
    {
      
    }

    private class AuthorizationFilter : IAsyncAuthorizationFilter, IOrderedFilter
    {
      private readonly AuthorizationService authorizationService;

      public AuthorizationFilter(AuthorizationService authorizationService)
      {
        this.authorizationService = authorizationService;
      }

      public int Order => 5;

      public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
      {
        
        if (!context.HttpContext.Request.Headers.TryGetValue("access-token", out var token))
        {
          context.Result = new StatusCodeResult(401);
          return;
        }
        if(!Guid.TryParse(token, out var accessToken))
        {
          context.Result = new StatusCodeResult(401);
          return;
        }
        if(!await this.authorizationService.CheckAuthenticated(accessToken))
          context.Result = new StatusCodeResult(403);
      }
    }
  }
}
