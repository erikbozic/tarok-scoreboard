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
      var hasRadelc = Scores[round.LeadPlayer].HasRadelc();
      var roundScore = round.GetScore() * (hasRadelc ? 2 : 1);

      if (hasRadelc && round.Won)
        Scores[round.LeadPlayer].RemoveRadelc();
     

      Scores[round.LeadPlayer].ChangeScore(roundScore);
      if(round.SupportingPLayer != null)
        Scores[round.SupportingPLayer].ChangeScore(roundScore);

      if((int)round.Game >= 70)
      {
        foreach (var score in Scores)
        {
          score.Value.AddRadelc();
        }
      }

      if (round.MonodFang != null)
        Scores[round.MonodFang].ChangeScore(-25);
      // TODO pagat ultimo fang, če je nenapovedan, je to osebno. mislim, da ne?
    }

  }
}
