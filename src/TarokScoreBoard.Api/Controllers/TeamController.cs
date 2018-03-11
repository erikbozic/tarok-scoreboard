using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TarokScoreBoard.Core.Entities;
using TarokScoreBoard.Infrastructure.Services;
using TarokScoreBoard.Shared.DTO;

namespace TarokScoreBoard.Api.Controllers
{
  [Route("api/[controller]")]
  public class TeamController : BaseController
  {
    private readonly TeamService teamService;

    public TeamController(TeamService teamService)
    {
      this.teamService = teamService;
    }
    [HttpPost]
    public async Task<ActionResult<Team>> CreateTeam(CreateTeamDTO createTeamDTO)
    {
      var team = await teamService.CreateTeamAsync(createTeamDTO);
      return Ok(team);
    }

    [HttpPost("/login")]
    public IActionResult Login(string passphrase)
    {
      Response.Cookies.Append("access-token", "some-token-guid", new Microsoft.AspNetCore.Http.CookieOptions() { Expires = DateTimeOffset.MaxValue });
      return Ok("not supported yet");
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
    public IActionResult UpdateTeam(Guid teamId)
    {
      // can only edit own team, probably?
      return Ok("not supported yet");
    }

  }
}
