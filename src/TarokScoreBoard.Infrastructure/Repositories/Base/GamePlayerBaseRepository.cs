// #autogenerated
using Npgsql;
using Dapper;
using System.Threading.Tasks;
using System;
using TarokScoreBoard.Core.Entities;

namespace TarokScoreBoard.Infrastructure.Repositories
{
  public class GamePlayerBaseRepository : BaseRepository<GamePlayer>
  {
    protected string selectFields = @"
        game_id, 
				name, 
				player_id";

    protected override string BaseSelect { get; set; }

    public GamePlayerBaseRepository(NpgsqlConnection conn) : base(conn)
    {    
      BaseSelect = $@"
        SELECT
          {selectFields}
        FROM game_player";
    }

    
    public GamePlayer Add(GamePlayer entity)
    {
      return conn.QueryFirst<GamePlayer>($@"
      INSERT INTO game_player
      (
        game_id, 
				name, 
				player_id
      )
      VALUES (:GameId, :Name, :PlayerId)
      RETURNING
        {selectFields}
      ",
      entity);
    }

    public async Task<GamePlayer> AddAsync(GamePlayer entity)
    {
      return await conn.QueryFirstAsync<GamePlayer>($@"
      INSERT INTO game_player
      (
        game_id, 
				name, 
				player_id
      )
      VALUES (:GameId, :Name, :PlayerId)
      RETURNING
        {selectFields}
      ",
      entity);
    }
  }
}
