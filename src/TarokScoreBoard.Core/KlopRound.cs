using System;
using System.Collections.Generic;
using TarokScoreBoard.Core.Entities;

namespace TarokScoreBoard.Core
{
  public class KlopRound : TarokRound
  {
    public KlopRound() : base()
    {
      KlopScores = new Dictionary<Guid, PlayerScore>();
    }

    public Dictionary<Guid, PlayerScore> KlopScores { get; set; }
    
  }
}
