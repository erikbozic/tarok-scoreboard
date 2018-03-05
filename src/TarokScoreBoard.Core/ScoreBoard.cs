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
      {
        score.Value.AddRadelc();
      }
    }

    public void ResetScores()
    {
      Scores = new Dictionary<Guid, PlayerScore>();

      foreach (var player in players)
      {
        Scores.Add(player, new PlayerScore());
      }
    }

    public void ApplyTarokRound(TarokRound round)
    {
      if (round is KlopRound klop)
      {
        foreach (var klopScore in klop.KlopScores)
        {
          Scores[klopScore.Key].ChangeScore(klopScore.Value.Score * (klopScore.Value.HasRadelc() ? 2: 1));
        }       
      }
      else
      {
        var hasRadelc = Scores[round.LeadPlayer].HasRadelc();
        var roundScore = round.GetScore() * (hasRadelc ? 2 : 1);

        if (hasRadelc && round.Won)
          Scores[round.LeadPlayer].RemoveRadelc();

        Scores[round.LeadPlayer].ChangeScore(roundScore);
        if (round.SupportingPLayer != Guid.Empty)
          Scores[round.SupportingPLayer].ChangeScore(roundScore);
      }

      if((int)round.Game >= 70)
      {
        foreach (var score in Scores)
        {
          score.Value.AddRadelc();
        }
      }

      if (round.MondFangPlayer != Guid.Empty)
        Scores[round.MondFangPlayer].ChangeScore(-25);
      // TODO pagat ultimo fang, če je nenapovedan, je to osebno. mislim, da ne?
    }

    public static ScoreBoard FromRound(IEnumerable<RoundResult> roundResults)
    {
      var sb = new ScoreBoard
      {
        GameId = roundResults.First().GameId,
        Scores = new Dictionary<Guid, PlayerScore>()
      };
      foreach (var result in roundResults)
      {
        sb.Scores.Add(result.PlayerId, PlayerScore.FromRoundResult(result));
      }
      
      return sb;
    }
  }
}
