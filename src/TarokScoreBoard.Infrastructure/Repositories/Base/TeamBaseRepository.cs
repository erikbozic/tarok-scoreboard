// #autogenerated
using Npgsql;
using Dapper;
using System.Threading.Tasks;
using System;
using TarokScoreBoard.Core.Entities;

namespace TarokScoreBoard.Infrastructure.Repositories
{
  public class TeamBaseRepository : BaseRepository<Team>
  {
    protected string selectFields = @"
        passphrase, 
				salt, 
				team_id, 
				team_name, 
				team_user_id";

    protected override string BaseSelect { get; set; }

    public TeamBaseRepository(NpgsqlConnection conn) : base(conn)
    {    
      BaseSelect = $@"
        SELECT
          {selectFields}
        FROM team";
    }

            
    public Team Get(Guid teamid)
    {
      return conn.QueryFirst<Team>($@"
      {BaseSelect}
      WHERE team_id = :teamid",
      new { teamid });
    }

    public async Task<Team> GetAsync(Guid teamid)
    {
      return await conn.QueryFirstAsync<Team>($@"
      {BaseSelect}
      WHERE team_id = :teamid",
      new { teamid });
    }

    public Team Update(Team entity)
    {
      return conn.QueryFirst<Team>($@"
      UPDATE team SET
        passphrase = :Passphrase, 
				salt = :Salt, 
				team_id = :TeamId, 
				team_name = :TeamName, 
				team_user_id = :TeamUserId
      WHERE team_id = :TeamId
      RETURNING
        {selectFields}",
      entity);
    }

    public async Task<Team> UpdateAsync(Team entity)
    {
      return await conn.QueryFirstAsync<Team>($@"
      UPDATE team SET
        passphrase = :Passphrase, 
				salt = :Salt, 
				team_id = :TeamId, 
				team_name = :TeamName, 
				team_user_id = :TeamUserId
      WHERE team_id = :TeamId
      RETURNING            
        {selectFields}",
      entity);
    }
            
    public bool Delete(Guid teamid)
    {
      return conn.Execute(@"
      DELETE FROM 
        team
      WHERE team_id = :teamid",
      new { teamid }) == 1;
    }

    public async Task<bool> DeleteAsync(Guid teamid)
    {
      return await conn.ExecuteAsync(@"
      DELETE FROM 
        team
      WHERE team_id = :teamid",
      new { teamid }) == 1;
    }
    
    public Team Add(Team entity)
    {
      return conn.QueryFirst<Team>($@"
      INSERT INTO team
      (
        passphrase, 
				salt, 
				team_id, 
				team_name, 
				team_user_id
      )
      VALUES (:Passphrase, :Salt, :TeamId, :TeamName, :TeamUserId)
      RETURNING
        {selectFields}
      ",
      entity);
    }

    public async Task<Team> AddAsync(Team entity)
    {
      return await conn.QueryFirstAsync<Team>($@"
      INSERT INTO team
      (
        passphrase, 
				salt, 
				team_id, 
				team_name, 
				team_user_id
      )
      VALUES (:Passphrase, :Salt, :TeamId, :TeamName, :TeamUserId)
      RETURNING
        {selectFields}
      ",
      entity);
    }
  }
}
