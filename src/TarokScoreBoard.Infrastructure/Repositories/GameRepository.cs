using Npgsql;

namespace TarokScoreBoard.Infrastructure.Repositories
{
  public class GameRepository : GameBaseRepository
  {
    public GameRepository(TarokDbContext dbContext) : base(dbContext)
    {

    }
  }
}
