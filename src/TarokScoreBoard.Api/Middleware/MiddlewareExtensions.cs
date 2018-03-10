using Microsoft.AspNetCore.Builder;

namespace TarokScoreBoard.Api.Middleware
{
  public static class MiddlewareExtensions
  {
    public static IApplicationBuilder UseErrorHandling(
          this IApplicationBuilder builder)
    {
      return builder.UseMiddleware<ErrorHandlingMiddleware>();
    }
  }
}
