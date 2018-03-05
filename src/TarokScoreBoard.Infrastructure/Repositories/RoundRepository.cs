using Npgsql;

namespace TarokScoreBoard.Infrastructure.Repositories
{
  public class RoundRepository : RoundBaseRepository
  {
    public RoundRepository(NpgsqlConnection conn) : base(conn)
    {
    }


  }
}
