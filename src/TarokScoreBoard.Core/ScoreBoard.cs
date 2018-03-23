using System;
using System.Collections.Generic;
using System.Linq;
using TarokScoreBoard.Core.Entities;

namespace TarokScoreBoard.Core
{
  public class ScoreBoard
  {
    private IEnumerable<Guid> players;

    public IDictionary<Guid, PlayerScore>  Scores { get; set; }

    public Guid GameId { get; set; }

    public ScoreBoard(IEnumerable<Guid> players)
    {
      this.players = players;      
    }

    private ScoreBoard()
    {
    }

    public void AddRadelc()
    {
      foreach (var score in Scores)
        score.Value.AddRadelc();
    }

    public void ResetScores()
    {
      Scores = new Dictionary<Guid, PlayerScore>();

      foreach (var player in players)
        Scores.Add(player, new PlayerScore());      
    }

    private void ChangeScore(Guid playerId, int baseScore) => 
      Scores[playerId].ChangeScore(baseScore * (Scores[playerId].HasRadelc() ? 2 : 1));

    public void ApplyTarokRound(TarokRound round)
    {
      if (round is KlopRound klop)
      { 
        round.Game = Shared.Enums.GameType.Klop;

        var fullPlayer = klop.KlopScores.FirstOrDefault(s => s.Value.Score < -35);
        var emptyPlayers = klop.KlopScores.Where(s => s.Value.Score == 0);

        if (emptyPlayers.Any() && fullPlayer.Value != null)
        {
          ChangeScore(fullPlayer.Key, -35);
          ChangeScore(emptyPlayers.First().Key, 35);

          if (Scores[emptyPlayers.First().Key].HasRadelc())
            Scores[emptyPlayers.First().Key].RemoveRadelc();
        }
        else if (!emptyPlayers.Any() && fullPlayer.Value != null)
          ChangeScore(fullPlayer.Key, -70);
        else if (emptyPlayers.Any() && fullPlayer.Value == null)
        {
          foreach (var emptyPlayer in emptyPlayers)
          {
            ChangeScore(emptyPlayer.Key, (70/emptyPlayers.Count()));

            if (Scores[emptyPlayer.Key].HasRadelc())
              Scores[emptyPlayer.Key].RemoveRadelc(); 
          }
        }
        else
        {
          foreach (var klopScore in klop.KlopScores)
          {
            var score = klopScore.Value.Score == -1 ? 0 : klopScore.Value.Score; // TODO ugly workaround. Solve this better.
            Scores[klopScore.Key].ChangeScore(score * (Scores[klopScore.Key].HasRadelc() ? 2 : 1));
          }
        }    
      }
      else
      {
        var hasRadelc = Scores[round.LeadPlayer.Value].HasRadelc();
        var roundScore = round.GetScore() * (hasRadelc ? 2 : 1);

        if (hasRadelc && round.Won)
          Scores[round.LeadPlayer.Value].RemoveRadelc();

        Scores[round.LeadPlayer.Value].ChangeScore(roundScore);
        if (round.SupportingPlayer != null)
          Scores[round.SupportingPlayer.Value].ChangeScore(roundScore);
      }

      if((int)round.Game >= 70)
      {
        foreach (var score in Scores)
        {
          score.Value.AddRadelc();
        }
      }

      if (round.MondFangPlayer != null)
        Scores[round.MondFangPlayer.Value].ChangeScore(-20);

      if (round.PagatFangPlayer != null)
        Scores[round.PagatFangPlayer.Value].ChangeScore(-25);
    }

    public void EndGame()
    {
      foreach (var score in Scores)
      {
        var leftRadelc = score.Value.RadelcCount - score.Value.UsedRadelcCount;
        
        score.Value.ChangeScore(leftRadelc * -100);
      }
    }

    public static ScoreBoard FromRound(IEnumerable<RoundResult> roundResults)
    {
      var sb = new ScoreBoard
      {
        GameId = roundResults.First().GameId,
        Scores = new Dictionary<Guid, PlayerScore>()
      };

      foreach (var result in roundResults)
        sb.Scores.Add(result.PlayerId, PlayerScore.FromRoundResult(result));      
      
      return sb;
    }
  }
}
