using System;
using System.Threading.Tasks;
using TarokScoreBoard.Core;
using TarokScoreBoard.Infrastructure.Repositories;

namespace TarokScoreBoard.Infrastructure.Services
{
  public class AuthorizationService
  {
    private readonly TarokDbContext dbContext;
    private readonly RequestContext context;

    public AuthorizationService(TarokDbContext dbContext, RequestContext context)
    {
      this.dbContext = dbContext;
      this.context = context;
    }

    public async Task<bool> CheckAuthenticated(Guid accessToken)
    {
      var teamId = await this.dbContext.GetTeamId(accessToken);

      if (teamId == null)
        return false;

      context.TeamId = teamId.Value;
      context.AccessToken = accessToken;
      return true;
    }
  }
}
