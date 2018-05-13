using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TarokScoreBoard.Core.Entities;
using TarokScoreBoard.Core.Exceptions;
using TarokScoreBoard.Infrastructure.Repositories;
using TarokScoreBoard.Shared.DTO;

namespace TarokScoreBoard.Infrastructure.Services
{
  public class TeamService
  {
    private readonly TarokDbContext dbContext;

    public TeamService(TarokDbContext dbContext)
    {
      this.dbContext = dbContext;
    }

    public async Task<IEnumerable<Team>> GetTeamsAsync()
    {
      var teams = await dbContext.Team
      .AsNoTracking()
      .ToListAsync();
      return teams;
    }

    public async Task<Team> GetTeamAsync(Guid teamId)
    {
      var team = await dbContext.Team
      .AsNoTracking()
      .FirstOrDefaultAsync(t => t.TeamId == teamId);
      
      var members = await dbContext.TeamPlayer
      .AsNoTracking()
      .Where(t => t.TeamId == teamId)
      .ToListAsync();
      
      team.Members.AddRange(members);
      return team;
    }

    public async Task<(Guid, Team)> LoginAsync(LoginDTO loginDto)
    {
      var teamId = loginDto.TeamId;
      var passphrase = loginDto.Passphrase;

      var team = await dbContext.Team
      .AsNoTracking()
      .FirstOrDefaultAsync(t => t.TeamUserId == teamId);
      
      if (team == null)
        throw new LoginFailedException("Credentials invalid!");

      using (var deriveBytes = new Rfc2898DeriveBytes(passphrase, team.Salt))
      {
        byte[] newKey = deriveBytes.GetBytes(32);

        if (!newKey.SequenceEqual(team.Passphrase))
          throw new LoginFailedException("Credentials invalid!");
      }

      var token = await this.dbContext.GetAccessToken(team.TeamId);

      team.Members = await dbContext.TeamPlayer
      .AsNoTracking()
      .Where(tp => tp.TeamId == team.TeamId)
      .ToListAsync();

      return (token, team);
    }

    public async Task<bool> CheckNameAsync(string username)
    {
      var team = await dbContext.Team
      .AsNoTracking()
      .FirstOrDefaultAsync(t => t.TeamUserId == username);

      return team != null;
    }

    public async Task<TeamDTO> CreateTeamAsync(CreateTeamDTO createTeamDTO)
    {
      var teamId = Guid.NewGuid();
      var (key, salt) = CreateKeyAndSalt(createTeamDTO.Passphrase);
      var team = new Team()
      {
        TeamUserId = createTeamDTO.TeamId,
        TeamName = createTeamDTO.Name,
        TeamId = teamId,
        Passphrase = key,
        Salt = salt
      };
      dbContext.Team.Add(team);

      foreach (var member in createTeamDTO.Members)
      {
        var teamPlayer = new TeamPlayer()
        {
          PlayerId = Guid.NewGuid(),
          TeamId = teamId,
          Name = member.Name
        };
        dbContext.TeamPlayer.Add(teamPlayer);

        team.Members.Add(teamPlayer);
      }

      await dbContext.SaveChangesAsync();

      return team.ToDto();
    }

    public async Task<TeamPlayer> AddPlayerToTeamAsync(AddPlayerToTeamDTO addPlayerDTO, Guid teamId)
    {

      var newPlayer =new TeamPlayer()
      {
        Name = addPlayerDTO.Name,
        TeamId = teamId,
        PlayerId = Guid.NewGuid()
      };
      dbContext.TeamPlayer.Add(newPlayer);
      await dbContext.SaveChangesAsync();
      return newPlayer;
    }

    private (byte[] key, byte[] salt) CreateKeyAndSalt(string passphrase)
    {
      byte[] salt;
      byte[] key;
      using (var hash = new Rfc2898DeriveBytes(passphrase, 32))
      {
        salt = hash.Salt;
        key = hash.GetBytes(32);
      }
      return (key, salt);
    }
  }
}
