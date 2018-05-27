using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TarokScoreBoard.Core;
using TarokScoreBoard.Core.Entities;
using TarokScoreBoard.Shared.DTO;
using Microsoft.EntityFrameworkCore;

namespace TarokScoreBoard.Infrastructure.Services
{
  public class ScoreBoardService
  {
    private readonly TarokDbContext dbContext;

    public ScoreBoardService(TarokDbContext dbContext)
    {
      this.dbContext = dbContext;
    }

    public async Task<IEnumerable<RoundDTO>> GetGameRounds(Guid gameId)
    {
      var rounds = await this.dbContext.Round
      .AsNoTracking()
      .Include(r => r.RoundResult)
      .Include(r => r.RoundModifier)
      .Where(r => r.GameId == gameId)
      .OrderBy(r => r.RoundNumber)
      .ToListAsync();

      return rounds.Select(r => r.ToDto());
    }

    public async Task<RoundDTO> AddRound(CreateRoundDTO createRoundRequest)
    {
      var round = Round.FromCreateRoundRequest(createRoundRequest);
      var gameId = round.GameId;

      var lastRound = await dbContext.Round
      .AsNoTracking()
      .OrderBy(r => r.RoundNumber)
      .LastOrDefaultAsync(r => r.GameId == gameId);

      round.RoundNumber = (lastRound?.RoundNumber ?? 0) + 1;

      var gamePlayers = await dbContext.GamePlayer
          .AsNoTracking()
          .Where(g => g.GameId == gameId)
          .OrderBy(p => p.Position)
          .ToListAsync();
          
      ScoreBoard scoreBoard;

      if(lastRound == null)
      {
        var gameinit = new GameInitializer(gamePlayers.Select(p => p.PlayerId));
        scoreBoard = gameinit.StartGame(gameId);
      }
      else
      {
        var lastRoundResults = await dbContext.RoundResult
        .Where(r => r.RoundId == lastRound.RoundId)
        .ToListAsync();
  
        scoreBoard = ScoreBoard.FromRound(lastRoundResults);
      }

      var tarokRound = TarokRound.FromRound(round);

      var scores = scoreBoard.ApplyTarokRound(tarokRound);
      // TODO in case of Klop the client sent results are already here. No like.
      round.RoundResult.Clear();
      foreach (var player in gamePlayers)
      {
        var playerScore = scores[player.PlayerId];
        
        round.RoundResult.Add(new RoundResult(){
          RoundId = round.RoundId,
          GameId = gameId,
          PlayerId = player.PlayerId,
          PlayerScore = playerScore.Score,
          PlayerRadelcCount = playerScore.RadelcCount,
          PlayerRadelcUsed = playerScore.UsedRadelcCount,
          RoundScoreChange = playerScore.RoundScoreChange          
        });
      }

      dbContext.Round.Add(round);
      await dbContext.SaveChangesAsync();

      return round.ToDto();
    }

    public async Task<Round> GetLastRound(Guid gameId)
    {
      var lastRound= await dbContext.Round
      .AsNoTracking()
      .Include(r => r.RoundResult)
      .OrderBy(r => r.RoundNumber)      
      .LastOrDefaultAsync(r => r.GameId == gameId);
      
      return lastRound;
    }

    public async Task<RoundDTO> EndGame(Guid gameId)
    {
      var lastRound = await this.GetLastRound(gameId);
      var scoreBoard = ScoreBoard.FromRound(lastRound.RoundResult);
      var scores = scoreBoard.EndGame();
      var roundId = Guid.NewGuid();
      var endRound = new Round()
      {
        GameId = gameId,
        RoundId = roundId,
        GameType = 0,
        RoundNumber = lastRound.RoundNumber + 1,
      };

      var gamePlayers = await dbContext.GamePlayer
        .AsNoTracking()
        .Where(g => g.GameId == gameId)
        .OrderBy(p => p.Position)
        .ToListAsync();

      foreach (var player in gamePlayers)
      {
        var playerScore = scores[player.PlayerId];
        
        endRound.RoundResult.Add(new RoundResult(){
          RoundId = endRound.RoundId,
          GameId = gameId,
          PlayerId = player.PlayerId,
          PlayerScore = playerScore.Score,
          PlayerRadelcCount = playerScore.RadelcCount,
          PlayerRadelcUsed = playerScore.RadelcCount
        });
      }
      
      dbContext.Round.Add(endRound);
      
      await dbContext.SaveChangesAsync();

      return endRound.ToDto();
    }

    public async Task<RoundDTO> DeleteLastRound(Guid gameId)
    {
      var lastRound = await dbContext.Round
      .Where(r => r.GameId == gameId)
      .OrderByDescending(r => r.RoundNumber)
      .FirstOrDefaultAsync();

      dbContext.Round.Remove(lastRound);

      await dbContext.SaveChangesAsync();

      return lastRound.ToDto();
    }
  }
}
