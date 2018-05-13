using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TarokScoreBoard.Core;
using TarokScoreBoard.Core.Entities;
using TarokScoreBoard.Core.Exceptions;
using TarokScoreBoard.Infrastructure.Repositories;
using TarokScoreBoard.Shared.DTO;

namespace TarokScoreBoard.Infrastructure.Services
{
  public class GameService
  {
    private readonly TarokDbContext dbContext;
    private readonly RequestContext context;
    private readonly ScoreBoardService scoreBoardService;
    private readonly ILogger<GameService> logger;

    public GameService(TarokDbContext dbContext, RequestContext context, ScoreBoardService scoreBoardService, ILogger<GameService> logger)
    {
      this.dbContext = dbContext;
      this.context = context;
      this.scoreBoardService = scoreBoardService;
      this.logger = logger;
    }

    public async Task<GameDTO> StartGameAsync(CreateGameDTO gameRequest)
    {      
      var game = new Game()
      {
        TeamId = gameRequest.TeamId,
        Name = gameRequest.Name,
        GameId = Guid.NewGuid(),
        Date = DateTime.Now
      };

      dbContext.Game.Add(game);

      if(game.TeamId != null)
      {
        var teamPlayers = await dbContext.TeamPlayer
        .AsNoTracking()
        .Where(tp => tp.TeamId == game.TeamId)
        .ToListAsync();;
      
       foreach(var player in gameRequest.Players)
       {
         if(player.PlayerId.HasValue && !teamPlayers.Select(tp => tp.PlayerId).Contains(player.PlayerId.Value))
          throw new OperationUnauthorizedException("Specified player not in team!");
       }
      }
      
      game.GamePlayer = new List<GamePlayer>();
      
      var gamePlayers = gameRequest.Players.Select(p => new GamePlayer(p.Name) { GameId = game.GameId, PlayerId = p.PlayerId ?? Guid.NewGuid() }).ToList();
      RandomizePlayerPosition(gamePlayers);

      foreach (var player in gamePlayers)
        game.GamePlayer.Add(player);

      await dbContext.SaveChangesAsync();

      return game.ToDto();
    }

    private void RandomizePlayerPosition(IList<GamePlayer> players)
    {
      var rand = new Random();
      var randomPositions = Enumerable.Range(1, players.Count).OrderBy(p => rand.Next()).ToArray();

      for (var i = 0; i < players.Count; i++)
        players[i].Position = randomPositions[i];      
    }

    public async Task<IEnumerable<GameDTO>> GetAllAsync(int limit = 5, int offset = 0)
    {
      var result = await dbContext.Game
      .OrderByDescending(g => g.Date)
      .Skip(offset)
      .Take(limit)
      .ToListAsync();
      return result.Select(g => g.ToDto());
    }

    public async Task<GameDTO> GetPreviousGame(Guid? teamId)
    {
      if (teamId == null)
        return null;

      var lastGame = await dbContext.Game
      .AsNoTracking()
      .Where(g => g.TeamId == teamId)
      .OrderByDescending(g => g.Date)
      .FirstOrDefaultAsync();

      return lastGame?.ToDto();
    }

    public async Task<GameDTO> GetAsync(Guid guid)
    {
      var game = await dbContext.Game
      .AsNoTracking()
      .Include(g => g.GamePlayer)      
      .FirstOrDefaultAsync(g => g.GameId == guid);

      var previousGame = await GetPreviousGame(game.TeamId);

      if(previousGame != null)
      {
        var lastRound = await this.scoreBoardService.GetLastRound(previousGame.GameId);
        if(lastRound != null)
        {
          var highestScore = lastRound.RoundResult.OrderByDescending(r => r.PlayerScore).FirstOrDefault();
          var bestPlayerPreviousGame = game.GamePlayer.FirstOrDefault(g => g.PlayerId == highestScore?.PlayerId);
          
          if (bestPlayerPreviousGame != null)
            bestPlayerPreviousGame.IsMaestro = true;
        }
      }

      return game.ToDto();
    }
  }
}
