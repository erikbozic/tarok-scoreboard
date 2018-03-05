using Npgsql;

namespace TarokScoreBoard.Infrastructure.Repositories
{
  public class RoundResultRepository : RoundResultBaseRepository
  {
    public RoundResultRepository(NpgsqlConnection conn) : base(conn)
    {
    }
  }
}
