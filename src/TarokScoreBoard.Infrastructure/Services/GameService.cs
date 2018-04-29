using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TarokScoreBoard.Core;
using TarokScoreBoard.Core.Entities;
using TarokScoreBoard.Infrastructure.Repositories;
using TarokScoreBoard.Shared.DTO;

namespace TarokScoreBoard.Infrastructure.Services
{
  public class GameService
  {
    private readonly GameRepository gameRepository;
    private readonly GamePlayerRepository playerRepository;
    private readonly RequestContext context;
    private readonly ScoreBoardService scoreBoardService;

    public GameService(GameRepository gameRepository, GamePlayerRepository playerRepository, RequestContext context, ScoreBoardService scoreBoardService)
    {
      this.gameRepository = gameRepository;
      this.playerRepository = playerRepository;
      this.context = context;
      this.scoreBoardService = scoreBoardService;
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

      game = await gameRepository.AddAsync(game);
      game.Players = new List<GamePlayer>();
      
      var gamePlayers = gameRequest.Players.Select(p => new GamePlayer(p.Name) { GameId = game.GameId, PlayerId = p.PlayerId ?? Guid.NewGuid() }).ToList();
      RandomizePlayerPosition(gamePlayers);
      // TODO Check the players actually belong the the specified team
      foreach (var player in gamePlayers)
      {
        var dbPlayer =  await playerRepository.AddAsync(player);
        game.Players.Add(dbPlayer);
      }

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
      var result = await gameRepository.GetAllAsync(limit, offset);
      return result.Select(g => g.ToDto());
    }

    public async Task<GameDTO> GetPreviousGame(Guid? teamId)
    {
      if (teamId == null)
        return null;

      var lastGame = (await gameRepository
        .GetAllAsync(c => c.Where(g => g.TeamId == teamId)
        .OrderByDescending(g => g.Date), 1, 1))
        .FirstOrDefault();

      return lastGame.ToDto();
    }

    public async Task<GameDTO> GetAsync(Guid guid)
    {
      var game = await gameRepository.GetAsync(guid);
      var id = game.GameId;
      var gamePlayers = await playerRepository.GetAllAsync(c => c.Where(p => p.GameId == id));
      game.Players = gamePlayers.ToList();

      var previousGame = await GetPreviousGame(game.TeamId);

      if(previousGame != null)
      {
        var lastRound = await this.scoreBoardService.GetLastRound(previousGame.GameId);

        var highestScore = lastRound.RoundResults.OrderByDescending(r => r.PlayerScore).FirstOrDefault();
        var bestPlayerPreviousGame = game.Players.FirstOrDefault(g => g.PlayerId == highestScore.PlayerId);
        bestPlayerPreviousGame.IsMaestro = true;
      }

      return game.ToDto();
    }
  }
}
