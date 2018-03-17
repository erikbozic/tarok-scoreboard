using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TarokScoreBoard.Api.Filters;
using TarokScoreBoard.Core;
using TarokScoreBoard.Core.Entities;
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

    public ScoreBoardController(ScoreBoardService scoreboardService, GameService gameService, RequestContext context)
    {
      this.scoreboardService = scoreboardService;
      this.gameService = gameService;
      this.context = context;
    }

    [HttpGet("{gameId}")]
    public async Task<ActionResult<ResponseDTO<IEnumerable<Round>>>> Get(Guid gameId)
    {
      var rounds = await scoreboardService.GetGameRounds(gameId);
      return Ok(rounds);
    }

    [HttpPost]
    [Authorize]
    [TransactionFilter]
    public async Task<ActionResult<ResponseDTO<Round>>> PostRound(CreateRoundDTO createRoundRequest)
    {
      if (!await CheckTeamId(createRoundRequest.GameId))
        return StatusCode(403);

      var score = await scoreboardService.AddRound(createRoundRequest);
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
    public async Task<ActionResult<ResponseDTO<Round>>> FinishGame(Guid gameId)
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
