using System;
using System.Net;

namespace TarokScoreBoard.Core.Exceptions
{
  public abstract class TarokBaseException : Exception
  {
    private string _userFriendyMessage;

    public TarokBaseException(string message, string userFriendlyMessage = "", object additionalData = null)
      : base(message)
    {
      UserFriendyMessage = userFriendlyMessage;
      AdditionalData = additionalData;
    }

    public abstract string ErrorCode { get; }

    public abstract HttpStatusCode StatusCode { get; }

    public string UserFriendyMessage
    {
      get
      {
        return String.IsNullOrWhiteSpace(_userFriendyMessage) ?
          this.Message 
          : this.UserFriendyMessage;
      }
      set => _userFriendyMessage = value; }

    public object AdditionalData { get; set; }
  }
}
