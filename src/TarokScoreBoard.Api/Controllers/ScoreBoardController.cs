using Microsoft.AspNetCore.Mvc;
using System;
using TarokScoreBoard.Shared.DTO;

namespace TarokScoreBoard.Api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ScoreBoardController : ControllerBase
  {
    public ScoreBoardController()
    {

    }

    [HttpGet("{guid}")]
    public ActionResult Get(Guid guid)
    {
      return Ok();
    }

    [HttpPost]
    public ActionResult PostRound(Round round)
    {
      return Ok();
    }

  }
}
