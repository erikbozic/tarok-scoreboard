using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;
using TarokScoreBoard.Infrastructure.Services;

namespace TarokScoreBoard.Api.Filters
{
  public class RequestContextAttribute : TypeFilterAttribute
  {
    public RequestContextAttribute() : base(typeof(RequestContextFilter))
    {
    }

    private class RequestContextFilter : IAsyncAuthorizationFilter, IOrderedFilter
    {
      private readonly AuthorizationService authorizationService;

      public RequestContextFilter(AuthorizationService authorizationService)
      {
        this.authorizationService = authorizationService;
      }

      public int Order => 10;

      public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
      {
        if (!context.HttpContext.Request.Headers.TryGetValue("access-token", out var token))
        {
          return;
        }
        if (!Guid.TryParse(token, out var accessToken))
        {
          return;
        }
        await this.authorizationService.CheckAuthenticated(accessToken);
      }
    }
  }


}
