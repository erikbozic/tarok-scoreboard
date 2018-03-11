using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TarokScoreBoard.Infrastructure.Repositories;

namespace TarokScoreBoard.Infrastructure.Services
{
  public class StatisticsService
  {
    private readonly RoundRepository roundRepository;
    private readonly RoundResultRepository roundResultRepository;
    private readonly TeamRepository teamRepository;
    private readonly TeamPlayerRepository teamPlayerRepository;

    public StatisticsService(RoundRepository roundRepository,
      RoundResultRepository roundResultRepository, TeamRepository teamRepository, TeamPlayerRepository teamPlayerRepository)
    {
      this.roundRepository = roundRepository;
      this.roundResultRepository = roundResultRepository;
      this.teamRepository = teamRepository;
      this.teamPlayerRepository = teamPlayerRepository;
    }

    public async Task<object> GetTeamPlayersStatistics(Guid teamId)
    {
      // this gets averages, round played, round lost etc....
      var results = await teamRepository.GetTeamPlayersStatisticsAsync(teamId);
      return results ;

    }
  }
}
