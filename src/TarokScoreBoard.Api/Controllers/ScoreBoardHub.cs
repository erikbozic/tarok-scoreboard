using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace TarokScoreBoard.Api.Controllers
{
  public class ScoreBoardHub : Hub
  {
    public Task SendRoundUpdate(Guid gameId)
    {
      return Clients.All.SendAsync("updateScoreBoard", new { Test = "this is a test", GameId = gameId });
    }

    public Task DoStuff(object test)
    {
      return Clients.All.SendAsync("updateScoreBoard", new { sent = test });
    }



  }
}
