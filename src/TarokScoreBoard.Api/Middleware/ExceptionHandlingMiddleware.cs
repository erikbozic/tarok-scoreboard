using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using TarokScoreBoard.Core.Exceptions;
using TarokScoreBoard.Shared.DTO;

namespace TarokScoreBoard.Api.Middleware
{
  public class ErrorHandlingMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly JsonSerializer _serializer;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
      _next = next;
      _serializer = new JsonSerializer
      {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
      };
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

    private async Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger<ErrorHandlingMiddleware> logger)
    { 
      var code = HttpStatusCode.InternalServerError;
      ResponseDTO<object> responseObj;

      // TODO figure out a way to propery solve exception handling and reporting for the 99% case...
      responseObj = new ResponseDTO<object>()
      {
        Data = null,
        Errors = Array.Empty<string>(),
        Message = exception.Message
      };

      if (exception is TarokBaseException tbe)
      {
        code = tbe.StatusCode;
        responseObj.Errors = tbe.AdditionalData;
      }
        
      logger.LogError(exception, exception.Message, code);
      context.Response.ContentType = "application/json";
      context.Response.StatusCode = (int)code;

      using (var writer = new StreamWriter(context.Response.Body))
      {
        _serializer.Serialize(writer, responseObj);
        await writer.FlushAsync();
      }
    }
  }
}
