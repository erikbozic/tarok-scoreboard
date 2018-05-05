using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TarokScoreBoard.Api.Filters;
using TarokScoreBoard.Core;
using TarokScoreBoard.Infrastructure.Services;
using TarokScoreBoard.Shared.DTO;

namespace TarokScoreBoard.Api.Controllers
{
  [Route("api/[controller]")]
  [RequestContext]
  public class GameController : BaseController
  {
    private readonly GameService gameService;
    private readonly RequestContext context;

    public GameController(GameService gameService, RequestContext context)
    {
      this.gameService = gameService;
      this.context = context;
    }

    [HttpGet]
    public async Task<ActionResult<ResponseDTO<IEnumerable<GameDTO>>>> Get(int? limit = 5, int? offset = 0)
    {
      var games = await gameService.GetAllAsync(limit.Value, offset.Value);
      return Ok(games);
    }
    
    [HttpGet("{gameId}")]
    public async Task<ActionResult<ResponseDTO<GameDTO>>> Get(Guid gameId)
    {
      var game = await gameService.GetAsync(gameId);
      if (game == null)
        return NotFound();

      return Ok(game);
    }
    
    [HttpPost]
    [TransactionFilter]
    public async Task<ActionResult<ResponseDTO<GameDTO>>> AddGame(CreateGameDTO gameRequest)
    {
      if (context.TeamId != null)
        gameRequest.TeamId = context.TeamId;
      else
        gameRequest.Players.ForEach(p => p.PlayerId = null);
      
      var game = await gameService.StartGameAsync(gameRequest);
      return Ok(game);
    }
  }
}
