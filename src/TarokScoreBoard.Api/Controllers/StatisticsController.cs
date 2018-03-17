using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TarokScoreBoard.Infrastructure.Services;

namespace TarokScoreBoard.Api.Controllers
{
  [Route("api/[controller]")]
  public class StatisticsController : BaseController
  {
    private readonly StatisticsService statisticsService;

    public StatisticsController(StatisticsService statisticsService)
    {
      this.statisticsService = statisticsService;
    }

    [HttpGet("{teamId}")]
    public async Task<IActionResult> GetTeamStatistcs(Guid teamId)
    {
      var stat = await statisticsService.GetTeamPlayersStatistics(teamId);
      return Ok(stat);
    }

    [HttpGet()]
    public async Task<IActionResult> GetTeamStatistcs(Guid? teamId, Guid? gameId, Guid? playerId)
    {
      var stat = await statisticsService.GetTeamPlayersStatistics(teamId, gameId, playerId);
      return Ok(stat);
    }
  }
}
