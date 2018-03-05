using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TarokScoreBoard.Core;
using TarokScoreBoard.Core.Entities;
using TarokScoreBoard.Infrastructure.Repositories;

namespace TarokScoreBoard.Infrastructure.Services
{
  public class ScoreBoardService
  {
    private readonly GameRepository gameRepository;
    private readonly GamePlayerRepository playerRepository;
    private readonly RoundRepository roundRepository;
    private readonly RoundResultRepository roundResultRepository;

    public ScoreBoardService(GameRepository gameRepo, GamePlayerRepository playerRepo, RoundRepository roundRepo, RoundResultRepository roundResultRepo)
    {
      this.gameRepository = gameRepo;
      this.playerRepository = playerRepo;
      this.roundRepository = roundRepo;
      this.roundResultRepository = roundResultRepo;
    }

    public async Task<IEnumerable<Round>> GetGameRounds(Guid gameId)
    {
      var rounds = await roundRepository.GetAllAsync(c => c.Where(r => r.GameId == gameId));

      return rounds;
    }

    public async Task<IDictionary<Guid, PlayerScore>> AddRound(Round round)
    {
      var gameId = round.GameId;

      var gameRounds = await roundRepository.GetAllAsync(c => c.Where(r => r.GameId == gameId).OrderBy(r => r.RoundNumber));

      var lastRound = gameRounds.LastOrDefault();
      var roundId = Guid.NewGuid();
      round.RoundId = roundId;
      round.RoundNumber = (lastRound?.RoundNumber ?? 0) + 1;
      
      var dbRound = await roundRepository.AddAsync(round);
      
      // update state of the scoreboard

      var game = await gameRepository.GetAsync(gameId);
      var players = (await playerRepository.GetAllAsync(c => c.Where(p => p.GameId == gameId))).ToDictionary(item => item.PlayerId, item => item);
      ScoreBoard scoreBoard = null;
      if(lastRound == null)
      {
        var gameinit = new GameInitializer(players.Select(p => p.Key));
        scoreBoard = gameinit.StartGame();
      }
      else
      {
        var lastRoundResults = await roundResultRepository.GetAllAsync(c => c.Where(r => r.RoundId == roundId));

        foreach (var result in lastRoundResults)
        {
          result.Player = players[result.PlayerId];
        }
        
        scoreBoard = ScoreBoard.FromRound(lastRoundResults); // TODO
      }

      var tarokRound =TarokRound.FromRound(round);

      scoreBoard.ApplyTarokRound(tarokRound);

      return scoreBoard.Scores;
    }

    public async Task<object> GetGameScoreboard(Guid gameId)
    {
      var rounds = await roundRepository.GetAllAsync(c => c.Where(r => r.GameId == gameId));


      var result = rounds.Select(r => new
      {
        
      });
      return result;
    }
  }
}
