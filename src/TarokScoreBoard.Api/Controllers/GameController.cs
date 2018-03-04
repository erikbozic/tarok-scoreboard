using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TarokScoreBoard.Infrastructure.Services;
using TarokScoreBoard.Shared.DTO;

namespace TarokScoreBoard.Api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class GameController : ControllerBase
  {
    private readonly GameService gameService;
    private readonly IConfiguration config;

    public GameController(GameService gameService, IConfiguration config)
    {
      this.gameService = gameService;
      this.config = config;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Game>>> Get()
    {
      var games = await  gameService.GetAllAsync();
      return Ok(games);
    }
    
    [HttpGet("{guid}")]
    public ActionResult<Game> Get(Guid guid)
    {
      var game = gameService.GetByGuid(guid);

      if (game == null)
        return NotFound();

      return Ok(game);
    }
    
    [HttpPost]
    public ActionResult<Game> AddGame(CreateGameRequest game)
    {
      var newGame = gameService.StartGame(game);
      return Ok(newGame);
    }
  }
}
