using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TarokScoreBoard.Core;
using TarokScoreBoard.Core.Entities;
using TarokScoreBoard.Infrastructure.Services;

namespace TarokScoreBoard.Api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ScoreBoardController : ControllerBase
  {
    private readonly ScoreBoardService scoreboardService;

    public ScoreBoardController(ScoreBoardService scoreboardService)
    {
      this.scoreboardService = scoreboardService;
    }

    [HttpGet("{gameId}")]
    public async Task<ActionResult<IEnumerable<Round>>> Get(Guid gameId)
    {
      var rounds = await scoreboardService.GetGameRounds(gameId);
      return Ok(rounds);
    }

    [HttpPost]
    public async Task<ActionResult<IDictionary<Guid, PlayerScore>>> PostRound(Round round)
    {
      var score = await scoreboardService.AddRound(round);
      return Created("",score); // TODO
    }
  }
}
