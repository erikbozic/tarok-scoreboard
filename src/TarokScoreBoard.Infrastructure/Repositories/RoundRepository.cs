using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TarokScoreBoard.Core.Entities;

namespace TarokScoreBoard.Infrastructure.Repositories
{
  public class RoundRepository : RoundBaseRepository
  {
    public RoundRepository(NpgsqlConnection conn) : base(conn)
    {
    }


    public async Task<IEnumerable<Round>> GetScoreboard(Guid gameId)
    {
      var sql = @"
      SELECT
        r.*,
        '0' ""round_result_id"",
        rr.*
      FROM round r
        LEFT JOIN round_result rr ON rr.round_id = r.round_id
      WHERE rr.game_id = :gameId
      ORDER BY r.round_number ASC
      ";

      var lookup = new Dictionary<Guid, Round>();
      await this.conn.QueryAsync<Round, RoundResult, Round>(
        sql, 
        (r,rr) => {
          if (!lookup.TryGetValue(r.RoundId, out Round round))
          {
            lookup.Add(r.RoundId, round = r);
          }
          if (round.RoundResults == null)
            round.RoundResults = new List<RoundResult>();
          round.RoundResults.Add(rr);
          return round;
        }, new { gameId }, splitOn: "round_result_id");

      return lookup.Values;
    }
  }
}
