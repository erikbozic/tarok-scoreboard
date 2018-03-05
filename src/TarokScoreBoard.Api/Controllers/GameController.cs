using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TarokScoreBoard.Core.Entities;
using TarokScoreBoard.Infrastructure.Services;
using TarokScoreBoard.Shared.DTO;

namespace TarokScoreBoard.Api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class GameController : ControllerBase
  {
    private readonly GameService gameService;

    public GameController(GameService gameService)
    {
      this.gameService = gameService;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Game>>> Get()
    {
      var games = await  gameService.GetAllAsync();
      return Ok(games);
    }
    
    [HttpGet("{guid}")]
    public async Task<ActionResult<Game>> Get(Guid guid)
    {
      var game = await gameService.GetByGuidAsync(guid);

      if (game == null)
        return NotFound();

      return Ok(game);
    }
    
    [HttpPost]
    public async Task<ActionResult<Game>> AddGame(CreateGameRequest gameRequest)
    {
      var game = await gameService.StartGameAsync(gameRequest);
      return Ok(game);
    }
  }
}
