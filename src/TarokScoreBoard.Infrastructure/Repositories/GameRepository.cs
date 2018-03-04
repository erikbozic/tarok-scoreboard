using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TarokScoreBoard.Shared.DTO;

namespace TarokScoreBoard.Infrastructure.Repositories
{
  public class GameRepository
  {
    private readonly IConfiguration configuration;
    private readonly NpgsqlConnection conn;

    public GameRepository(IConfiguration configuration)
    {
      this.configuration = configuration;
      this.conn = new NpgsqlConnection(configuration.GetConnectionString("tarok"));
    }
    public async Task<IEnumerable<Game>> GetAllAsync()
    {
      var result = await conn.QueryAsync<Game>(@"SELECT id ""Guid"", game_name ""Name"" FROM game");
      return result;
    }
  }
}
