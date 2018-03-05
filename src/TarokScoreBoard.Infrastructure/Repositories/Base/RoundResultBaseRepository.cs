// #autogenerated
using Npgsql;
using Dapper;
using System.Threading.Tasks;
using TarokScoreBoard.Core.Entities;

namespace TarokScoreBoard.Infrastructure.Repositories
{
  public class RoundResultBaseRepository : BaseRepository<RoundResult>
  {
    protected string selectFields = @"
        game_id, 
				player_id, 
				player_radelc_count, 
				player_radelc_used, 
				player_score, 
				round_id";

    protected override string baseSelect { get; set; }

    public RoundResultBaseRepository(NpgsqlConnection conn) : base(conn)
    {    
      baseSelect = $@"
        SELECT
          {selectFields}
        FROM round_result";
    }

    
    public RoundResult Add(RoundResult entity)
    {
      return conn.QueryFirst<RoundResult>($@"
      INSERT INTO round_result
      (
        game_id, 
				player_id, 
				player_radelc_count, 
				player_radelc_used, 
				player_score, 
				round_id
      )
      VALUES (:GameId, :PlayerId, :PlayerRadelcCount, :PlayerRadelcUsed, :PlayerScore, :RoundId)
      RETURNING
        {selectFields}
      ",
      entity);
    }

    public async Task<RoundResult> AddAsync(RoundResult entity)
    {
      return await conn.QueryFirstAsync<RoundResult>($@"
      INSERT INTO round_result
      (
        game_id, 
				player_id, 
				player_radelc_count, 
				player_radelc_used, 
				player_score, 
				round_id
      )
      VALUES (:GameId, :PlayerId, :PlayerRadelcCount, :PlayerRadelcUsed, :PlayerScore, :RoundId)
      RETURNING
        {selectFields}
      ",
      entity);
    }
  }
}
