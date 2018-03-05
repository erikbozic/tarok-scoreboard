using Npgsql;

namespace TarokScoreBoard.Infrastructure.Repositories
{
  public class GamePlayerRepository : GamePlayerBaseRepository
  {
    public GamePlayerRepository(NpgsqlConnection conn) : base(conn)
    {
    } 
  }
}
