using System;
using System.Collections.Generic;
using TarokScoreBoard.Core.Entities;

namespace TarokScoreBoard.Core
{
  public class TarokRound
  {
    // TODO klop
    public TarokRound()
    {
      Modifiers = new List<Modifier>();
    }

    public Guid LeadPlayer { get; set; }

    public Guid SupportingPLayer { get; set; }

    public bool Won { get; set; }

    public Game Game { get; set; }

    public int ScoreDifference { get; set; }

    public List<Modifier> Modifiers { get; set; }

    public Contra ContraFactor { get; set; } = Contra.None;

    public Guid MondFangPlayer { get; set; }

    // TODO MondFang

    public virtual int GetScore()
    {
      var wonModifier = Won ? 1 : -1;

      var result = wonModifier * (int)(ScoreDifference + Game) * (int)ContraFactor;

      foreach (var mod in Modifiers)
      {
        result += (int)mod.Team * (int)mod.Announced * (int)mod.ModifierType * (int)mod.ContraFactor;
      }

      return result;
    }

    public static TarokRound FromRound(Round round)
    {
      var tarokRound = new TarokRound()
      {
        Won = round.Won,
        ScoreDifference = round.Difference,
        ContraFactor = (Contra)round.ContraFactor,
        LeadPlayer = round.LeadPlayerId,
        SupportingPLayer = round.SupportingPlayerId,
        MondFangPlayer = round.MondFangPlayerId,
        Game = (Game)round.GameType,        
      };

      return tarokRound;
    }
  }
}
