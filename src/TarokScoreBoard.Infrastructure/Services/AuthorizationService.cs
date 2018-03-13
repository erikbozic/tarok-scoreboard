using Npgsql;
using System;
using System.Threading.Tasks;
using TarokScoreBoard.Core;
using TarokScoreBoard.Infrastructure.Repositories;

namespace TarokScoreBoard.Infrastructure.Services
{
  public class AuthorizationService
  {
    private readonly NpgsqlConnection connection;
    private readonly TeamRepository teamRepository;
    private readonly RequestContext context;

    public AuthorizationService(TeamRepository teamRepository, RequestContext context)
    {
      this.teamRepository = teamRepository;
      this.context = context;
    }

    public async Task<bool> CheckAuthenticated(Guid accessToken)
    {
      var teamId = await this.teamRepository.GetTeamId(accessToken);

      if (teamId == null)
        return false;

      context.TeamId = teamId.Value;
      context.AccessToken = accessToken;
      return true;
    }
  }
}
