using Npgsql;

namespace TarokScoreBoard.Infrastructure.Repositories
{
  public class TeamPlayerRepository : TeamPlayerBaseRepository
  {
    public TeamPlayerRepository(TarokDbContext dbContext) : base(dbContext)
    {
    }
  }
}
