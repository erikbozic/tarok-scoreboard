using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TarokScoreBoard.Api.Filters;
using TarokScoreBoard.Core.Entities;
using TarokScoreBoard.Infrastructure.Services;
using TarokScoreBoard.Shared.DTO;

namespace TarokScoreBoard.Api.Controllers
{
  [Route("api/[controller]")]
  public class GameController : BaseController
  {
    private readonly GameService gameService;

    public GameController(GameService gameService)
    {
      this.gameService = gameService;
    }

    [HttpGet]
    public async Task<ActionResult<ResponseDTO<IEnumerable<Game>>>> Get()
    {
      var games = await gameService.GetAllAsync();
      return Ok(games);
    }
    
    [HttpGet("{gameId}")]
    [ResponseCache(Duration = 72000, Location = ResponseCacheLocation.Any)]
    public async Task<ActionResult<ResponseDTO<Game>>> Get(Guid gameId)
    {
      var game = await gameService.GetAsync(gameId);

      if (game == null)
        return NotFound();

      return Ok(game);
    }
    
    [HttpPost]
    [TransactionFilter]
    public async Task<ActionResult<ResponseDTO<Game>>> AddGame(CreateGameDTO gameRequest)
    {
      var game = await gameService.StartGameAsync(gameRequest);
      return Ok(game);
    }
  }
}
