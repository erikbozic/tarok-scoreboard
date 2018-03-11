using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TarokScoreBoard.Core.Entities;
using TarokScoreBoard.Infrastructure.Repositories;
using TarokScoreBoard.Shared.DTO;

namespace TarokScoreBoard.Infrastructure.Services
{
  public class TeamService
  {
    private readonly TeamRepository teamRepository;
    private readonly TeamPlayerRepository teamPlayerRepository;

    public TeamService(TeamRepository teamRepository, TeamPlayerRepository teamPlayerRepository)
    {
      this.teamRepository = teamRepository;
      this.teamPlayerRepository = teamPlayerRepository;
    }

    public async Task<IEnumerable<Team>> GetTeamsAsync()
    {
      var teams = await teamRepository.GetAllAsync();
      return teams;
    }

    public async Task<Team> GetTeamAsync(Guid teamId)
    {
      var team = await teamRepository.GetAsync(teamId);
      var members = await teamPlayerRepository.GetAllAsync(c => c.Where(t => t.TeamId == teamId));
      team.Members.AddRange(members);
      return team;
    }

    public async Task<Team> CreateTeamAsync(CreateTeamDTO createTeamDTO)
    {
      var teamId = Guid.NewGuid();
      var team = await teamRepository.AddAsync(new Team()
      {
        TeamName = createTeamDTO.Name,
        TeamId = teamId,
        Passphrase = createTeamDTO.Passphrase // TODO SHA-256  it
      });

      foreach (var member in createTeamDTO.Members)
      {
        var dbMember = await teamPlayerRepository.AddAsync(new TeamPlayer()
        {
          TeamId = teamId,
          Name = member.Name
        });
        team.Members.Add(dbMember);
      }
      return team;
    }
  }
}
