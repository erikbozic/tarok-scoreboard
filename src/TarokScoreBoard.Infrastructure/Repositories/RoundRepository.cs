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
    public RoundRepository(TarokDbContext dbContext) : base(dbContext)
    {
    }


    public async Task<IEnumerable<Round>> GetScoreboard(Guid gameId)
    {
      var sql = @"
      SELECT
        r.*,
        0 ""rr_id"",
        rr.*,
        0 ""rm_id"",
        rm.*
      FROM round r
        LEFT JOIN round_result rr ON rr.round_id = r.round_id
        LEFT JOIN round_modifier rm ON rm.round_id = r.round_id
      WHERE rr.game_id = :gameId
      ORDER BY r.round_number ASC, rr.player_id ASC
      ";

      // TODO should just add an id column on both results and modifiers, would make this easier...
      var lookup = new Dictionary<Guid, Round>();
      var resultLookup = new Dictionary<GuidGuidComposite, bool>();
      var modlookup = new Dictionary<GuidStringComposite, bool>();
      await this.conn.QueryAsync(
        sql, 
(Func<Round, RoundResult, RoundModifier, Round>)((r,rr, rm) => {
          if (!lookup.TryGetValue(r.RoundId, out Round round))
            lookup.Add(r.RoundId, round = r);

          // if a round already has this player's result, dont add...
          var ggc = new GuidGuidComposite(r.RoundId, rr.PlayerId);
          if (!resultLookup.TryGetValue(ggc, out bool value))
          {
            resultLookup.Add(ggc, value);
            round.RoundResult.Add(rr);
          }

          // if a round already has this modifier, dont add
          var gsc = new GuidStringComposite(r.RoundId, rm.ModifierType);
          if (!string.IsNullOrEmpty(rm.ModifierType) && !modlookup.TryGetValue(gsc, out bool modValue))
          {
            modlookup.Add(gsc, modValue);
            round.RoundModifier.Add(rm);
          }

          return round;
        }), new { gameId }, splitOn: "rr_id,rm_id");

      return lookup.Values;
    }
  }

  public struct GuidGuidComposite
  {
    public GuidGuidComposite(Guid roundId, Guid playerId)
    {
      this.roundId = roundId;
      this.playerId = playerId;
    }

    Guid roundId;

    Guid playerId;
  }

  public struct GuidStringComposite
  {
    public GuidStringComposite(Guid roundId, string modifierType)
    {
      this.roundId = roundId;
      this.modifierType = modifierType;
    }

    Guid roundId;

    string modifierType;
  }
}
