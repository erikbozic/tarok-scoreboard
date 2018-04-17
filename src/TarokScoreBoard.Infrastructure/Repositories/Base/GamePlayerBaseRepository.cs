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
				player_id, 
				position";

    protected override string BaseSelect { get; set; }

    public GamePlayerBaseRepository(NpgsqlConnection conn) : base(conn)
    {    
      BaseSelect = $@"
        SELECT
          {selectFields}
        FROM game_player";
    }

            
    public GamePlayer Get(Guid gameid)
    {
      return conn.QueryFirst<GamePlayer>($@"
      {BaseSelect}
      WHERE game_id = :gameid",
      new { gameid });
    }

    public async Task<GamePlayer> GetAsync(Guid gameid)
    {
      return await conn.QueryFirstAsync<GamePlayer>($@"
      {BaseSelect}
      WHERE game_id = :gameid",
      new { gameid });
    }

    public GamePlayer Update(GamePlayer entity)
    {
      return conn.QueryFirst<GamePlayer>($@"
      UPDATE game_player SET
        game_id = :GameId, 
				name = :Name, 
				player_id = :PlayerId, 
				position = :Position
      WHERE game_id = :GameId
      RETURNING
        {selectFields}",
      entity);
    }

    public async Task<GamePlayer> UpdateAsync(GamePlayer entity)
    {
      return await conn.QueryFirstAsync<GamePlayer>($@"
      UPDATE game_player SET
        game_id = :GameId, 
				name = :Name, 
				player_id = :PlayerId, 
				position = :Position
      WHERE game_id = :GameId
      RETURNING            
        {selectFields}",
      entity);
    }
            
    public bool Delete(Guid gameid)
    {
      return conn.Execute(@"
      DELETE FROM 
        game_player
      WHERE game_id = :gameid",
      new { gameid }) == 1;
    }

    public async Task<bool> DeleteAsync(Guid gameid)
    {
      return await conn.ExecuteAsync(@"
      DELETE FROM 
        game_player
      WHERE game_id = :gameid",
      new { gameid }) == 1;
    }
    
    public GamePlayer Add(GamePlayer entity)
    {
      return conn.QueryFirst<GamePlayer>($@"
      INSERT INTO game_player
      (
        game_id, 
				name, 
				player_id, 
				position
      )
      VALUES (:GameId, :Name, :PlayerId, :Position)
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
				player_id, 
				position
      )
      VALUES (:GameId, :Name, :PlayerId, :Position)
      RETURNING
        {selectFields}
      ",
      entity);
    }
  }
}
