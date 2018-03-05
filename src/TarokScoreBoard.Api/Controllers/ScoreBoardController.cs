using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TarokScoreBoard.Api.Filters;
using TarokScoreBoard.Core.Entities;
using TarokScoreBoard.Infrastructure.Services;
using TarokScoreBoard.Shared.DTO;

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
    [TransactionFilter]
    public async Task<ActionResult<Round>> PostRound(CreateRoundDTO createRoundRequest)
    {
      var score = await scoreboardService.AddRound(createRoundRequest);
      return Ok(score);
    }
  }
}
