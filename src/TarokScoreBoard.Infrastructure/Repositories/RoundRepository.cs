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
        '0' ""rr_id"",
        rr.*,
        '0' ""rm_id"",
        rm.*
      FROM round r
        LEFT JOIN round_result rr ON rr.round_id = r.round_id
        LEFT JOIN round_modifier rm ON rm.round_id = r.round_id
      WHERE rr.game_id = :gameId
      ORDER BY r.round_number ASC, rr.player_id ASC
      ";

      var lookup = new Dictionary<Guid, Round>();
      await this.conn.QueryAsync<Round, RoundResult, RoundModifier, Round>(
        sql, 
        (r,rr, rm) => {
          if (!lookup.TryGetValue(r.RoundId, out Round round))
            lookup.Add(r.RoundId, round = r);          

          round.RoundResults.Add(rr);

          var modlookup = new Dictionary<string, RoundModifier>();

          if (!String.IsNullOrEmpty(rm.ModifierType) && !modlookup.TryGetValue(rm.ModifierType, out var mod))
          {
            modlookup.Add(rm.ModifierType, mod = rm);
            round.Modifiers.Add(mod);
          }

          return round;
        }, new { gameId }, splitOn: "rr_id,rm_id");

      return lookup.Values;
    }
  }
}
