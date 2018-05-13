using Npgsql;

namespace TarokScoreBoard.Infrastructure.Repositories
{
  public class GamePlayerRepository : GamePlayerBaseRepository
  {
    public GamePlayerRepository(TarokDbContext dbContext) : base(dbContext)
    {
    } 
  }
}
