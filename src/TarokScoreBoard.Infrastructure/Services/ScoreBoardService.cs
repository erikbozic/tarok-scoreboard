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
  public class ScoreBoardService
  {
    private readonly GameRepository gameRepository;
    private readonly GamePlayerRepository playerRepository;
    private readonly RoundRepository roundRepository;
    private readonly RoundResultRepository roundResultRepository;
    private readonly RoundModifierRepository modifierRepository;

    public ScoreBoardService(GameRepository gameRepo,
      GamePlayerRepository playerRepo,
      RoundRepository roundRepo,
      RoundResultRepository roundResultRepo,
      RoundModifierRepository modifierRepo)
    {
      this.gameRepository = gameRepo;
      this.playerRepository = playerRepo;
      this.roundRepository = roundRepo;
      this.roundResultRepository = roundResultRepo;
      this.modifierRepository = modifierRepo;
    }

    public async Task<IEnumerable<Round>> GetGameRounds(Guid gameId)
    {
      var rounds = await roundRepository.GetScoreboard(gameId);

      return rounds;
    }

    public async Task<Round> AddRound(CreateRoundDTO createRoundRequest)
    {
      var round = Round.FromCreateRoundRequest(createRoundRequest);
      var gameId = round.GameId;

      var gameRounds = await roundRepository.GetAllAsync(c => c.Where(r => r.GameId == gameId).OrderBy(r => r.RoundNumber));

      var lastRound = gameRounds.LastOrDefault();

      round.RoundId = Guid.NewGuid(); 
      round.RoundNumber = (lastRound?.RoundNumber ?? 0) + 1;

      await roundRepository.AddAsync(round);
      // TODO move to repo, bulk insert
      foreach (var mod in round.Modifiers)
        await modifierRepository.AddAsync(mod);

      var game = await gameRepository.GetAsync(gameId);
      var players = (await playerRepository.GetAllAsync(c => c.Where(p => p.GameId == gameId))).ToDictionary(item => item.PlayerId, item => item);
      ScoreBoard scoreBoard = null;

      if(lastRound == null)
      {
        var gameinit = new GameInitializer(players.Select(p => p.Key));
        scoreBoard = gameinit.StartGame(gameId);
      }
      else
      {
        var lastRoundId = lastRound.RoundId;
        var lastRoundResults = await roundResultRepository.GetAllAsync(c => c.Where(r => r.RoundId == lastRoundId));
  
        scoreBoard = ScoreBoard.FromRound(lastRoundResults);
      }

      var tarokRound = TarokRound.FromRound(round);

      scoreBoard.ApplyTarokRound(tarokRound);
      await this.AddRoundResults(scoreBoard, round.RoundId);

      round.RoundResults = new List<RoundResult>();
      round.RoundResults.AddRange(scoreBoard.Scores.Select(s =>
      {
        return new RoundResult()
        {
          GameId = gameId,
          PlayerId = s.Key,
          PlayerScore = s.Value.Score,
          PlayerRadelcCount = s.Value.RadelcCount,
          PlayerRadelcUsed = s.Value.UsedRadelcCount
        };
      }));

      return round;
    }

    private async Task AddRoundResults(ScoreBoard scoreBoard, Guid roundId)
    {
      foreach (var score in scoreBoard.Scores)
      {
        await roundResultRepository.AddAsync(new RoundResult()
        {
           RoundId = roundId,
           GameId = scoreBoard.GameId,
           PlayerId = score.Key,
           PlayerScore = score.Value.Score,
           PlayerRadelcCount = score.Value.RadelcCount,
           PlayerRadelcUsed = score.Value.UsedRadelcCount
        });
      }
    }
  }
}
