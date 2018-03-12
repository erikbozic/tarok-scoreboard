using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TarokScoreBoard.Infrastructure.Repositories
{
  public class TeamRepository : TeamBaseRepository
  {
    public TeamRepository(NpgsqlConnection conn) : base(conn)
    {
    }

    public async Task<object> GetTeamPlayersStatisticsAsync(Guid teamId)
    {
      var sql = @"
          SELECT
            tp.name,
            stat.*
          FROM (
                 SELECT
                   rr.player_id,
                   avg(rr.round_score_change)       average_per_round,
                   avg(lead.round_score_change)     average_per_lead,
                   avg(prikolca.round_score_change) average_per_prikolica,
                   avg(playing.round_score_change)  average_per_played,
                   max(rr.round_score_change)       max_pos_score_change,
                   min(rr.round_score_change)       max_neg_score_change,
                   count(lead.player_id)            count_lead,
                   count(prikolca.player_id)        count_prikolica,
                   count(playing.player_id)         count_played,
                   count(lead_lost.player_id)       count_lead_lost,
                   count(lead_won.player_id)        count_lead_won,
                   sum(played_lost.round_score_change) sum_score_lost,
                   sum(played_won.round_score_change) sum_score_won
                 FROM round_result rr
                   INNER JOIN team_player tp ON tp.player_id = rr.player_id --AND team_id = :teamId
                   LEFT JOIN round r ON rr.round_id = r.round_id
                   LEFT JOIN round_result lead
                     ON r.round_id = lead.round_id AND lead.player_id = r.lead_player_id AND lead.player_id = rr.player_id
                   LEFT JOIN round_result prikolca
                     ON r.round_id = prikolca.round_id AND prikolca.player_id = r.supporting_player_id AND
                        prikolca.player_id = rr.player_id
                   LEFT JOIN round_result playing ON r.round_id = playing.round_id AND
                                                     (playing.player_id = r.supporting_player_id OR
                                                      playing.player_id = r.lead_player_id) AND playing.player_id = rr.player_id
                   LEFT JOIN round_result lead_lost ON r.round_id = lead_lost.round_id AND lead_lost.player_id = r.lead_player_id AND lead_lost.player_id = rr.player_id
                      AND lead_lost.round_score_change < 0
                   LEFT JOIN round_result lead_won   ON r.round_id = lead_won.round_id AND lead_won.player_id = r.lead_player_id AND lead_won.player_id = rr.player_id
                      AND lead_won.round_score_change > 0
                   LEFT JOIN round_result played_won   ON r.round_id = played_won.round_id AND played_won.player_id = rr.player_id
                      AND played_won.round_score_change > 0
                   LEFT JOIN round_result played_lost   ON r.round_id = played_lost.round_id AND played_lost.player_id = rr.player_id
                      AND played_lost.round_score_change < 0
                 GROUP BY rr.player_id
               ) stat
            LEFT JOIN team_player tp ON tp.player_id = stat.player_id;";

      return await this.conn.QueryAsync(sql, new { teamId });
    }
  }
}
