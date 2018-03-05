using Npgsql;

namespace TarokScoreBoard.Infrastructure.Repositories
{
  public class GameRepository : GameBaseRepository
  {
    public GameRepository(NpgsqlConnection conn) : base(conn)
    {

    }
  }
}
