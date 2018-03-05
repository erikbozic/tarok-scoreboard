using System;
using System.Collections.Generic;
using System.Text;
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
