using System.Collections.Generic;

namespace TarokScoreBoard.Core
{
  public class TarokRound
  {
    // TODO klop
    public TarokRound()
    {
      Modifiers = new List<Modifier>();      
    }

    public Player LeadPlayer { get; set; }

    public Player SupportingPLayer { get; set; }

    public bool Won { get; set; }

    public Game Game { get; set; }

    public int ScoreDifference { get; set; }

    public List<Modifier> Modifiers { get; set; }

    public Contra ContraFactor { get; set; } = Contra.None;

    public Player MonodFang { get; set; }
    
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
  }
}
