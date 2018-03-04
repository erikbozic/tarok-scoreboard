using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TarokScoreBoard.Shared.DTO;

namespace TarokScoreBoard.Infrastructure.Services
{
  public class GameService
  {
    private readonly IConfiguration config;
    private readonly NpgsqlConnection conn;

    public GameService(IConfiguration config)
    {
      this.config = config;
      this.conn = new NpgsqlConnection(config.GetConnectionString("tarok"));
    }

    public Game StartGame(CreateGameRequest game)
    {
      // TODO save game
      return new Game()
      {
        Date = DateTime.Now,
        Name = game.Name,
        Guid = Guid.NewGuid(),
        Players = game.Players
      };
    }

    public async Task<List<Game>> GetAllAsync()
    {
      var result = await conn.QueryAsync<Game>(@"SELECT id ""Guid"", game_name ""Name"" FROM game");
      return result.ToList();
    }

    public Game GetByGuid(Guid guid)
    {
      return new Game();
    }
  }
}
