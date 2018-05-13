using System.Net;

namespace TarokScoreBoard.Core.Exceptions
{
  public class RoundNotExistsException : TarokBaseException
  {
    public RoundNotExistsException(string message, string userFriendlyMessage = "Round doesn't exist", object additionalData = null) : base(message, userFriendlyMessage, additionalData)
    {
    }

    public override string ErrorCode => "ROUND_NOT_EXISTS";

    public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
  }
}
