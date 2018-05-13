using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TarokScoreBoard.Core;
using TarokScoreBoard.Core.Entities;
using TarokScoreBoard.Infrastructure.Repositories;
using TarokScoreBoard.Shared.DTO;
using Microsoft.EntityFrameworkCore;

namespace TarokScoreBoard.Infrastructure.Services
{
  public class ScoreBoardService
  {
    private readonly TarokDbContext dbContext;

    public ScoreBoardService(TarokDbContext dbContext)
    {;
      this.dbContext = dbContext;
    }

    public async Task<IEnumerable<RoundDTO>> GetGameRounds(Guid gameId)
    {
      var rounds = await this.dbContext.Round
      .AsNoTracking()
      .Include(r => r.RoundResult)
      .Include(r => r.RoundModifier)
      .Where(r => r.GameId == gameId)
      .ToListAsync();

      return rounds.Select(r => r.ToDto());
    }

    public async Task<RoundDTO> AddRound(CreateRoundDTO createRoundRequest)
    {
      var round = Round.FromCreateRoundRequest(createRoundRequest);
      var gameId = round.GameId;

      var gameRounds = dbContext.Round.Where(r => r.GameId == gameId).OrderBy(r => r.RoundNumber);

      var lastRound = await gameRounds.LastOrDefaultAsync();
      round.RoundNumber = (lastRound?.RoundNumber ?? 0) + 1;

      await dbContext.Round.AddAsync(round);

      foreach (var mod in round.RoundModifier)
        await dbContext.RoundModifier.AddAsync(mod);

      var game = await dbContext.Game
          .Include(g => g.GamePlayer)
          .FirstOrDefaultAsync(g => g.GameId == gameId);
          
      var players = game.GamePlayer.ToDictionary(item => item.PlayerId, item => item);
      ScoreBoard scoreBoard = null;

      if(lastRound == null)
      {
        var gameinit = new GameInitializer(players.Select(p => p.Key));
        scoreBoard = gameinit.StartGame(gameId);
      }
      else
      {
        var lastRoundId = lastRound.RoundId;
        var lastRoundResults = await dbContext.RoundResult.Where(r => r.RoundId == lastRoundId).ToListAsync();
  
        scoreBoard = ScoreBoard.FromRound(lastRoundResults);
      }

      var tarokRound = TarokRound.FromRound(round);

      scoreBoard.ApplyTarokRound(tarokRound);

      round.RoundResult = new List<RoundResult>();
      var scores =scoreBoard.Scores.OrderBy(s => s.Key).Select(s =>
      {
        return new RoundResult()
        {
          RoundId = round.RoundId,
          GameId = gameId,
          PlayerId = s.Key,
          PlayerScore = s.Value.Score,
          PlayerRadelcCount = s.Value.RadelcCount,
          PlayerRadelcUsed = s.Value.RadelcCount // should we leave as is?
        };
      });
      foreach(var score in scores)
        round.RoundResult.Add(score);

      foreach (var roundResult in round.RoundResult)
        await dbContext.RoundResult.AddAsync(roundResult);


      await dbContext.SaveChangesAsync();

      return round.ToDto();
    }

    public async Task<Round> GetLastRound(Guid gameId)
    {
      var gameRounds = dbContext.Round
      .AsNoTracking()
      .Include(r => r.RoundResult)
      .Where(r => r.GameId == gameId).OrderBy(r => r.RoundNumber);

      var lastRound = await gameRounds.LastOrDefaultAsync();
      
      return lastRound;
    }

    public async Task<RoundDTO> EndGame(Guid gameId)
    {
      var lastRound = await this.GetLastRound(gameId);
      var scoreBoard = ScoreBoard.FromRound(lastRound.RoundResult);
      scoreBoard.EndGame();
      var roundId = Guid.NewGuid();
      var endRound = new Round()
      {
        GameId = gameId,
        RoundId = roundId,
        GameType = 0,
        RoundNumber = lastRound.RoundNumber + 1,
      };
      

      var scores =scoreBoard.Scores.OrderBy(s => s.Key).Select(s =>
      {
        return new RoundResult()
        {
          RoundId = roundId,
          GameId = gameId,
          PlayerId = s.Key,
          PlayerScore = s.Value.Score,
          PlayerRadelcCount = s.Value.RadelcCount,
          PlayerRadelcUsed = s.Value.RadelcCount // should we leave as is?
        };
      });
      foreach(var score in scores)
        endRound.RoundResult.Add(score);

      dbContext.Round.Add(endRound);
      
      await dbContext.SaveChangesAsync();

      return endRound.ToDto();
    }

    private async Task AddRoundResults(ScoreBoard scoreBoard, Guid roundId)
    {
      foreach (var score in scoreBoard.Scores)
      {
         dbContext.RoundResult.Add(new RoundResult()
        {
           RoundId = roundId,
           GameId = scoreBoard.GameId,
           PlayerId = score.Key,
           PlayerScore = score.Value.Score,
           PlayerRadelcCount = score.Value.RadelcCount,
           PlayerRadelcUsed = score.Value.UsedRadelcCount
        });
      }

      await dbContext.SaveChangesAsync();
    }

    public async Task<RoundDTO> DeleteLastRound(Guid gameId)
    {
      var lastRound = await dbContext.Round.Where(r => r.GameId == gameId)
      .OrderByDescending(r => r.RoundNumber)
      .FirstOrDefaultAsync();

      dbContext.Round.Remove(lastRound);

      await dbContext.SaveChangesAsync();

      return lastRound.ToDto();
    }
  }
}
