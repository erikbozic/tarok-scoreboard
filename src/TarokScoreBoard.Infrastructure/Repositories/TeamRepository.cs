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
                   rr.player_id ""playerId"",
                   avg(rr.round_score_change)       ""averagePerRound"",
                   avg(lead.round_score_change)     ""averagePerLead"",
                   avg(prikolca.round_score_change) ""averagePerPrikolica"",
                   avg(playing.round_score_change)  ""averagePerPlayed"",
                   max(rr.round_score_change)       ""maxPosScoreChange"",
                   min(rr.round_score_change)       ""maxNegScoreChange"",
                   count(lead.player_id)            ""countLead"",
                   count(prikolca.player_id)        ""countPrikolica"",
                   count(playing.player_id)         ""countPlayed"",
                   count(lead_lost.player_id)       ""countLeadLost"",
                   count(lead_won.player_id)        ""countLeadWon"",
                   sum(played_lost.round_score_change) ""sumScoreLost"",
                   sum(played_won.round_score_change) ""sumScoreWon""
                 FROM round_result rr
                   INNER JOIN team_player tp ON tp.player_id = rr.player_id AND team_id = :teamId
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
            LEFT JOIN team_player tp ON tp.player_id = stat.""playerId"";";

      return await this.conn.QueryAsync(sql, new { teamId });
    }

    public async Task<Guid> GetAccessToken(Guid teamId)
    {
      var accessToken = await this.conn.QueryFirstOrDefaultAsync<Guid>(@"
        SELECT access_token FROM team_access_token
        WHERE team_id = :teamId", new { teamId });

      if(accessToken == Guid.Empty)
      {
        var token = Guid.NewGuid();

        accessToken = await conn.QueryFirstAsync<Guid>(@"
          INSERT INTO team_access_token (team_id, access_token) VALUES (:teamId, :token)
          RETURNiNG access_token", new { teamId, token });
      }

      return accessToken;
    }

    public async Task<Guid?> GetTeamId(Guid accessToken)
    {
      var teamId = await this.conn.QueryFirstOrDefaultAsync<Guid?>(@"
        SELECT team_id FROM team_access_token
        WHERE access_token = :accessToken", new { accessToken });
      
      

      return teamId;
    }
  }
}
