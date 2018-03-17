using System.Net;

namespace TarokScoreBoard.Core.Exceptions
{
  public class LoginFailedException : TarokBaseException
  {
    public LoginFailedException(string message, string userFriendlyMessage = "Uporabniško me ali geslo ni pravilno", object additionalData = null) : base(message, userFriendlyMessage, additionalData)
    {
    }

    public override string ErrorCode => "LOGIN_FAILED";

    public override HttpStatusCode StatusCode => HttpStatusCode.Unauthorized;
  }
}
