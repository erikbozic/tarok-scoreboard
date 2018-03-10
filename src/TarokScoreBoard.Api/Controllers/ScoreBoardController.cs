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
  public class ScoreBoardController : BaseController
  {
    private readonly ScoreBoardService scoreboardService;

    public ScoreBoardController(ScoreBoardService scoreboardService)
    {
      this.scoreboardService = scoreboardService;
    }

    [HttpGet("{gameId}")]
    public async Task<ActionResult<ResponseDTO<IEnumerable<Round>>>> Get(Guid gameId)
    {
      var rounds = await scoreboardService.GetGameRounds(gameId);
      return Ok(rounds);
    }

    [HttpPost]
    [TransactionFilter]
    public async Task<ActionResult<ResponseDTO<Round>>> PostRound(CreateRoundDTO createRoundRequest)
    {
      var score = await scoreboardService.AddRound(createRoundRequest);
      return Ok(score);
    }

    [HttpDelete("{gameId}")]
    [TransactionFilter]
    public async Task<IActionResult> Delete(Guid gameId)
    {
      throw new Exception("aaa");
      var result = await scoreboardService.DeleteLastRound(gameId);
      return NoContent();
    }
  }
}
