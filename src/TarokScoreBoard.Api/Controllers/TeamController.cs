using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TarokScoreBoard.Api.Filters;
using TarokScoreBoard.Core;
using TarokScoreBoard.Core.Entities;
using TarokScoreBoard.Infrastructure.Services;
using TarokScoreBoard.Shared;
using TarokScoreBoard.Shared.DTO;

namespace TarokScoreBoard.Api.Controllers
{
  [Route("api/[controller]")]
  public class TeamController : BaseController
  {
    private readonly TeamService teamService;
    private readonly RequestContext context;

    public TeamController(TeamService teamService, RequestContext context)
    {
      this.teamService = teamService;
      this.context = context;
    }
    [HttpPost]
    public async Task<ActionResult<Team>> CreateTeam(CreateTeamDTO createTeamDTO)
    {
      var team = await teamService.CreateTeamAsync(createTeamDTO);
      return Ok(team);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDTO>> Login(LoginDTO loginDto)
    {
      var accessToken = await teamService.LoginAsync(loginDto);
      return Ok(new LoginResponseDTO(accessToken));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Team>>> Get()
    {
      var teams = await teamService.GetTeamsAsync();
      return Ok(teams);
    }

    [HttpGet("{teamId}")]
    public async Task<ActionResult<Team>> Get(Guid teamId)
    {
      var team = await teamService.GetTeamAsync(teamId);

      return Ok(team);
    }

    [HttpPut]
    [Authorize]
    public IActionResult UpdateTeam(Guid teamId)
    {
      // can only edit own team, probably?
      return Ok("not supported yet");
    }

    [HttpPost("{teamId}/player")]
    [Authorize]
    public async Task<ActionResult> AddPlayerToTeam(Guid teamId, AddPlayerToTeamDTO addPlayerDTO)
    {
      if (teamId != context.TeamId)
        return Forbid();

      var newPlayer = await teamService.AddPlayerToTeamAsync(addPlayerDTO, teamId);
      return Ok(newPlayer);
    }

  }
}
