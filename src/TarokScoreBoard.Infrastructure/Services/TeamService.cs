﻿using System;
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
    private readonly TeamRepository teamRepository;
    private readonly TeamPlayerRepository teamPlayerRepository;
    private readonly TarokDbContext dbContext;

    public TeamService(TeamRepository teamRepository, TeamPlayerRepository teamPlayerRepository, TarokDbContext dbContext)
    {
      this.teamRepository = teamRepository;
      this.teamPlayerRepository = teamPlayerRepository;
      this.dbContext = dbContext;
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

      var token = await this.teamRepository.GetAccessToken(team.TeamId);

      team.Members = await dbContext.TeamPlayer.AsNoTracking().Where(tp => tp.TeamId == team.TeamId).ToListAsync();
             
      return (token,team);
    }

    public async Task<bool> CheckNameAsync(string username)
    {
      var team = await teamRepository.GetAllAsync(c => c.Where(t => t.TeamUserId == username));

      return team.FirstOrDefault() != null;
    }

    public async Task<TeamDTO> CreateTeamAsync(CreateTeamDTO createTeamDTO)
    {
      var teamId = Guid.NewGuid();
      var (key, salt) = CreateKeyAndSalt(createTeamDTO.Passphrase);
      var team = await teamRepository.AddAsync(new Team()
      {
        TeamUserId = createTeamDTO.TeamId,
        TeamName = createTeamDTO.Name,
        TeamId = teamId,
        Passphrase = key,
        Salt = salt
      });

      foreach (var member in createTeamDTO.Members)
      {
        var dbMember = await teamPlayerRepository.AddAsync(new TeamPlayer()
        {
          PlayerId = Guid.NewGuid(),          
          TeamId = teamId,
          Name = member.Name
        });
        team.Members.Add(dbMember);
      }
      return team.ToDto();
    }

    public async Task<TeamPlayer> AddPlayerToTeamAsync(AddPlayerToTeamDTO addPlayerDTO, Guid teamId)
    {
      var newPlayer = await teamPlayerRepository.AddAsync(new TeamPlayer()
      {
        Name = addPlayerDTO.Name,
        TeamId = teamId,
        PlayerId = Guid.NewGuid()
      });

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
