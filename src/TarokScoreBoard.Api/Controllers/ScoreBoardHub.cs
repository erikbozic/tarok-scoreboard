﻿using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace TarokScoreBoard.Api.Controllers
{
  public class ScoreBoardHub : Hub
  {
    public async override Task OnConnectedAsync()
    {
      var ctx = this.Context.GetHttpContext();
      var gameId =ctx.Request.Query["gameId"];

      await Groups.AddToGroupAsync(this.Context.ConnectionId, gameId);

      await base.OnConnectedAsync();
    }
  }
}
