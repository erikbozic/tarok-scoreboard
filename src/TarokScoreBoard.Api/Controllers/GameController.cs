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
  [ApiController]
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
    public async Task<ActionResult<ResponseDTO<Game>>> Get(Guid gameId)
    {
      var game = await gameService.GetByGuidAsync(gameId);

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
