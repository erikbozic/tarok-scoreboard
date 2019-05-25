using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using TarokScoreBoard.Api.Filters;

namespace TarokScoreBoard.Api.Swagger
{
  public class AuthResponsesOperationFilter : IOperationFilter
  {
    public void Apply(Operation operation, OperationFilterContext context)
    {


      var authAttributes = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
      .Union(context.MethodInfo.GetCustomAttributes(true))
      .OfType<AuthorizeAttribute>();


      //everything is json
      operation.Produces = new[] { "application/json" };
      
      if (authAttributes.Any())
      {
        operation.Responses.Add("401", new Response { Description = "Unauthorized" });
        operation.Security = new List<IDictionary<string, IEnumerable<string>>>
        {
          new Dictionary<string, IEnumerable<string>>
          {
            { "access-token", new string[] { } }
          }
        };
      }
    }
  }
}
