using System;

namespace TarokScoreBoard.Core
{
  public class RequestContext
  {
    public Guid AccessToken { get; set; }

    public Guid TeamId { get; set; }
  }
}
