using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using TarokScoreBoard.Shared.DTO;

namespace TarokScoreBoard.Api
{
  public static class StartupExtensions
  {
    public static IServiceCollection ConfigureValidationModel(this IServiceCollection services)
    {
      services.Configure<ApiBehaviorOptions>(options =>
      {
        var previous = options.InvalidModelStateResponseFactory;
        options.InvalidModelStateResponseFactory = context =>
        {
          var result = (BadRequestObjectResult)previous(context);
          result.Value = ResponseDTO.Create<object>(null, "There were validation errors", context.ModelState.Values.SelectMany(v => v.Errors));

          return result;
        };
      });
      return services;
    }
  }
}
