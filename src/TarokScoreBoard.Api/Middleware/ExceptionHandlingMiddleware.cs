using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters.Json.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;
using TarokScoreBoard.Shared.DTO;

namespace TarokScoreBoard.Api.Middleware
{
  public class ErrorHandlingMiddleware
  {
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
      _next = next;
    }

    public async Task Invoke(HttpContext context, ILogger<ErrorHandlingMiddleware> logger)
    {
      try
      {
        await _next(context);
      }
      catch (Exception ex)
      {
        await HandleExceptionAsync(context, ex, logger);
      }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger<ErrorHandlingMiddleware> logger)
    {
      logger.LogError(exception.HResult, exception, "Error returned to the client.");
      var code = HttpStatusCode.InternalServerError; // 500 if unexpected

      // TODO In production, backend shouldn't return an error message with  internal implementation details.
      var error = ResponseDTO.Create<object>(null, exception.Message);

      // TODO if an expected exception type, then ne more detailed.
      //if (exception is TarokBaseException tbe)
      //{
      //  code = tbe.StatusCode;
      //  error.Message = tbe.Message;
      //  error.UserFriendlyMessage = tbe.UserFriendyMessage;
      //  error.AdditionalData = tbe.AdditionalData;
      //  error.ErrorCode = tbe.ErrorCode;
      //}


      // TODO use global serilaization settings
      var result = JsonConvert.SerializeObject(error);
      context.Response.ContentType = "application/json";
      context.Response.StatusCode = (int)code;
      return context.Response.WriteAsync(result);
    }
  }
}
