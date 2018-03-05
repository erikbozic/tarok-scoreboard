using Npgsql;

namespace TarokScoreBoard.Infrastructure.Repositories
{
  public class RoundModifierRepository : RoundModifierBaseRepository
  {
    public RoundModifierRepository(NpgsqlConnection conn) : base(conn)
    {
    }
  }
}
