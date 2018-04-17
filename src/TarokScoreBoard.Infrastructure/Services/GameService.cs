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

    public async Task<IEnumerable<GameDTO>> GetAllAsync()
    {
      var result = await gameRepository.GetAllAsync();
      return result.Select(g => g.ToDto());
    }

    public async Task<GameDTO> GetAsync(Guid guid)
    {
      var game = await gameRepository.GetAsync(guid);
      var id = game.GameId;
      var gamePlayers = await playerRepository.GetAllAsync(c => c.Where(p => p.GameId == id));
      game.Players = gamePlayers.ToList();

      return game.ToDto();
    }
  }
}
