using System.Net;

namespace TarokScoreBoard.Core.Exceptions
{
  public class GameNotExistsException : TarokBaseException
  {
    public GameNotExistsException(string message, string userFriendlyMessage = "Game doesn't exist", object additionalData = null) : base(message, userFriendlyMessage, additionalData)
    {
    }

    public override string ErrorCode => "GAME_NOT_EXISTS";

    public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
  }
}
