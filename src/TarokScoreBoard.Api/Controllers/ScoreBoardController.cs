using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TarokScoreBoard.Api.Filters;
using TarokScoreBoard.Core;
using TarokScoreBoard.Core.Exceptions;
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
    public async Task<ActionResult<ResponseDTO<RoundDTO>>> PostRound(CreateRoundDTO createRoundRequest)
    {
      if (!await CheckTeamId(createRoundRequest.GameId))
        return StatusCode(403);

      var round = await scoreboardService.AddRound(createRoundRequest);
      await hub.Clients.Group(round.GameId.ToString()).SendAsync("updateScoreBoard", round);
      return Ok(round);
    }
    
    [HttpDelete("{gameId}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid gameId)
    {
      if (!await CheckTeamId(gameId))
        return StatusCode(403);

      var deletedRound = await scoreboardService.DeleteLastRound(gameId);

      if (deletedRound != null)
      {
        await hub.Clients.Group(gameId.ToString()).SendAsync("deleteLastRound", deletedRound);
        return NoContent();
      }
      else
      {
        throw new RoundNotExistsException("No round to delete!");
      }        
    }
    
    [HttpPost("end/{gameId}")]
    [Authorize]
    public async Task<ActionResult<ResponseDTO<RoundDTO>>> FinishGame(Guid gameId)
    {
      if (!await CheckTeamId(gameId))
        return StatusCode(403);

      var round = await scoreboardService.EndGame(gameId);
      await hub.Clients.Group(round.GameId.ToString()).SendAsync("updateScoreBoard", round);
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
