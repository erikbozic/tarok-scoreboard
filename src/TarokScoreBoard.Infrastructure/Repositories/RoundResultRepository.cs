using Npgsql;

namespace TarokScoreBoard.Infrastructure.Repositories
{
  public class RoundResultRepository : RoundResultBaseRepository
  {
    public RoundResultRepository(TarokDbContext dbContext) : base(dbContext)
    {
    }
  }
}
