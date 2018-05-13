using System.Net;

namespace TarokScoreBoard.Core.Exceptions
{
  public class OperationUnauthorizedException : TarokBaseException
  {
    public OperationUnauthorizedException(string message, string userFriendlyMessage = null, object additionalData = null) : base(message, userFriendlyMessage ?? message, additionalData)
    {
    }

    public override string ErrorCode => "OPERATION_UNAUTHORIZED";

    public override HttpStatusCode StatusCode => HttpStatusCode.Unauthorized;
  }
}
