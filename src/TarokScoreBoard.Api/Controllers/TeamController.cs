using Microsoft.AspNetCore.Mvc;
using System;
using TarokScoreBoard.Shared.DTO;

namespace TarokScoreBoard.Api.Controllers
{
  [Route("api/[controller]")]
  public class TeamController : BaseController
  {
    [HttpPost]
    public IActionResult CreateTeam(CreateTeamDTO createTeamDTO)
    {
      return Ok("ohai thaere");
    }

    [HttpPost("/login")]
    public IActionResult Login(string passphrase)
    {
      Response.Cookies.Append("access-token", "some-token-guid", new Microsoft.AspNetCore.Http.CookieOptions() { Expires = DateTimeOffset.MaxValue });
      return Ok();
    }

    [HttpGet]
    public IActionResult Get()
    {
      //gets top teams
      return Ok();
    }

    [HttpGet("{teamId}")]
    public IActionResult Get(Guid teamId)
    {
      // returns team details
      return Ok();
    }

    [HttpPut]
    public IActionResult UpdateTeam(Guid teamId)
    {
      // can only edit own team, probably?
      return Ok();
    }

  }
}
