using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TarokScoreBoard.Api.Filters;
using TarokScoreBoard.Core;
using TarokScoreBoard.Core.Entities;
using TarokScoreBoard.Infrastructure.Services;
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
      var (accessToken, team) = await teamService.LoginAsync(loginDto);

      return Ok(new LoginResponseDTO(accessToken, new TeamDTO()
      {
        TeamId = team.TeamId,
        TeamName = team.TeamName,
        TeamUserId = team.TeamUserId,
        Members = team.Members.Select(p => new TeamPlayerDTO(p.Name) { PlayerId = p.PlayerId }).ToList()
      }));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TeamDTO>>> Get()
    {
      var teams = await teamService.GetTeamsAsync();
      return Ok(teams.Select(t => t.ToDto()));
    }

    [HttpGet("{teamId}")]
    public async Task<ActionResult<TeamDTO>> Get(Guid teamId)
    {
      var team = await teamService.GetTeamAsync(teamId);

      return Ok(team.ToDto());
    }

    [HttpGet("check-name/{username}")]
    [SwaggerResponse(422, Description = "If team with same username already exists, otherwise 200 OK")]
    public async Task<IActionResult> CheckTeamUsernameExists(string username)
    {
      var exists = await this.teamService.CheckNameAsync(username);

      if (exists)
        return UnprocessableEntity(username);

      return Ok(username);
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
        return StatusCode(403);

      var newPlayer = await teamService.AddPlayerToTeamAsync(addPlayerDTO, teamId);
      return Ok(newPlayer);
    }
  }
}
