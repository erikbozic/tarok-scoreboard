using System;
using System.Threading.Tasks;
using TarokScoreBoard.Infrastructure.Repositories;

namespace TarokScoreBoard.Infrastructure.Services
{
  public class StatisticsService
  {
    private readonly TarokDbContext dbContext;

    public StatisticsService(TarokDbContext dbContext)
    {
      this.dbContext = dbContext;
    }

    public async Task<object> GetTeamPlayersStatistics(Guid? teamId = null, Guid? gameId = null, Guid? playerId = null)
    {
      // this gets averages, round played, round lost etc....
      var results = await dbContext.GetTeamPlayersStatisticsAsync(teamId, gameId, playerId);
      return results;
    }
  }
}
