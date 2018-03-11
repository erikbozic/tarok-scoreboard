using Npgsql;

namespace TarokScoreBoard.Infrastructure.Repositories
{
  public class TeamPlayerRepository : TeamPlayerBaseRepository
  {
    public TeamPlayerRepository(NpgsqlConnection conn) : base(conn)
    {
    }
  }
}
