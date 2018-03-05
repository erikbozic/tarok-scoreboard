using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TarokScoreBoard.Core.Entities;
using TarokScoreBoard.Infrastructure.Repositories;
using TarokScoreBoard.Shared.DTO;

namespace TarokScoreBoard.Infrastructure.Services
{
  public class GameService
  {
    private readonly GameRepository gameRepository;
    private readonly GamePlayerRepository playerRepository;

    public GameService(GameRepository gameRepository, GamePlayerRepository playerRepository)
    {
      this.gameRepository = gameRepository;
      this.playerRepository = playerRepository;
    }

    public async Task<Game> StartGameAsync(CreateGameRequest gameRequest)
    {      
      var game = new Game()
      {
        Name = gameRequest.Name,
        GameId = Guid.NewGuid(),
        Date = DateTime.Now
      };

      game = await gameRepository.AddAsync(game);
      game.Players = new List<GamePlayer>();

      foreach (var player in gameRequest.Players.Select(p => new GamePlayer(p.Name) { GameId = game.GameId, PlayerId = Guid.NewGuid() }))
      {
        var dbPlayer =  await playerRepository.AddAsync(player);
        game.Players.Add(dbPlayer);
      }

      return game;
    }

    public async Task<IEnumerable<Game>> GetAllAsync()
    {
      var result = await gameRepository.GetAllAsync();
      return result;
    }

    public async Task<Game> GetByGuidAsync(Guid guid)
    {
      var game = await gameRepository.GetAsync(guid);
      var id = game.GameId;
      var gamePlayers = await playerRepository.GetAllAsync(c => c.Where(p => p.GameId == id));
      game.Players = gamePlayers.ToList();

      return game;
    }
  }
}
