using System;
using System.Collections.Generic;

namespace TarokScoreBoard.Core
{
  public class ScoreBoard
  {
    private Player[] players;

    public IDictionary<Player, PlayerScore>  Scores { get; set; }

    public ScoreBoard(Player[] players)
    {
      this.players = players;      
    }

    public void AddRadelc()
    {
      foreach (var score in Scores)
      {
        score.Value.AddRadelc();
      }
    }

    public void Reset()
    {
      Scores = new Dictionary<Player, PlayerScore>();

      foreach (var player in players)
      {
        Scores.Add(player, new PlayerScore());
      }
    }

    public void ApplyTarokRound(TarokRound round)
    {
      var roundScore = round.GetScore();
      if (Scores[round.LeadPlayer].HasRadelc())
      { 
        roundScore *= 2;
        if (round.Won)
          Scores[round.LeadPlayer].RemoveRadelc();
      }

      Scores[round.LeadPlayer].ChangeScore(roundScore);
      if(round.SupportingPLayer != null)
        Scores[round.SupportingPLayer].ChangeScore(roundScore);

      // TODO Add additional player based modifigers // mond fang, pagat fang
    }
  }
}
