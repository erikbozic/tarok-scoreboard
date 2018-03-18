using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Sockets.Http.Features;
using System;
using System.Threading.Tasks;

namespace TarokScoreBoard.Api.Controllers
{
  public class ScoreBoardHub : Hub
  {
    public async override Task OnConnectedAsync()
    {
      var ctx = this.Context.Connection.Features.Get<IHttpContextFeature>();
      var gameId =ctx.HttpContext.Request.Query["gameId"];

      await Groups.AddAsync(this.Context.ConnectionId, gameId);

      await base.OnConnectedAsync();
    }
  }
}
