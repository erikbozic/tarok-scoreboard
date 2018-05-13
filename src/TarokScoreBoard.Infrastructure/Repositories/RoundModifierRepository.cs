using Npgsql;

namespace TarokScoreBoard.Infrastructure.Repositories
{
  public class RoundModifierRepository : RoundModifierBaseRepository
  {
    public RoundModifierRepository(TarokDbContext dbContext) : base(dbContext)
    {
    }
  }
}
