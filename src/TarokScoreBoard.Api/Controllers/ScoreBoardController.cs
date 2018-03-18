using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TarokScoreBoard.Api.Filters;
using TarokScoreBoard.Core;
using TarokScoreBoard.Infrastructure.Services;
using TarokScoreBoard.Shared.DTO;

namespace TarokScoreBoard.Api.Controllers
{
  [Route("api/[controller]")]
  public class ScoreBoardController : BaseController
  {
    private readonly ScoreBoardService scoreboardService;
    private readonly GameService gameService;
    private readonly RequestContext context;
    private readonly IHubContext<ScoreBoardHub> hub;

    public ScoreBoardController(ScoreBoardService scoreboardService, GameService gameService, RequestContext context, IHubContext<ScoreBoardHub> hub)
    {
      this.scoreboardService = scoreboardService;
      this.gameService = gameService;
      this.context = context;
      this.hub = hub;
    }

    [HttpGet("{gameId}")]
    public async Task<ActionResult<ResponseDTO<IEnumerable<RoundDTO>>>> Get(Guid gameId)
    {
      var rounds = await scoreboardService.GetGameRounds(gameId);
      return Ok(rounds);
    }

    [HttpPost]
    [Authorize]
    [TransactionFilter]
    public async Task<ActionResult<ResponseDTO<RoundDTO>>> PostRound(CreateRoundDTO createRoundRequest)
    {
      if (!await CheckTeamId(createRoundRequest.GameId))
        return StatusCode(403);

      var score = await scoreboardService.AddRound(createRoundRequest);
      await hub.Clients.All.SendAsync("updateScoreBoard", score);
      return Ok(score);
    }



    [HttpDelete("{gameId}")]
    [Authorize]
    [TransactionFilter]
    public async Task<IActionResult> Delete(Guid gameId)
    {
      if (!await CheckTeamId(gameId))
        return StatusCode(403);

      var result = await scoreboardService.DeleteLastRound(gameId);
      return NoContent();
    }


    [HttpPost("end/{gameId}")]
    [Authorize]
    [TransactionFilter]
    public async Task<ActionResult<ResponseDTO<RoundDTO>>> FinishGame(Guid gameId)
    {
      if (!await CheckTeamId(gameId))
        return Forbid();

      var round = await scoreboardService.EndGame(gameId);
      return Ok(round);
    }

    [NonAction]
    private async Task<bool> CheckTeamId(Guid gameId)
    {
      var game = await gameService.GetAsync(gameId);

      if (game.TeamId != this.context.TeamId)
        return false;

      return true;
    }
  }
}
