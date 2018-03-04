using System.Collections.Generic;

namespace TarokScoreBoard.Core
{
  public class KlopRound : TarokRound
  {
    public KlopRound() : base()
    {
      KlopScores = new Dictionary<Player, PlayerScore>();
    }

    public Dictionary<Player, PlayerScore> KlopScores { get; set; }
    
  }
}
