using System;
using System.Collections.Generic;
using System.Linq;
using TarokScoreBoard.Core.Entities;
using TarokScoreBoard.Shared;
using TarokScoreBoard.Shared.Enums;

namespace TarokScoreBoard.Core
{
  public class TarokRound
  {
    public TarokRound()
    {
      Modifiers = new List<Modifier>();
    }

    public Guid? LeadPlayer { get; set; }

    public Guid? SupportingPLayer { get; set; }

    public bool Won { get; set; }

    public GameType Game { get; set; }

    public int? ScoreDifference { get; set; }

    public List<Modifier> Modifiers { get; set; }

    public Contra ContraFactor { get; set; } = Contra.None;

    public Guid? MondFangPlayer { get; set; }

    public Guid? PagatFangPlayer { get; set; }

    public virtual int GetScore()
    {
      var wonModifier = Won ? 1 : -1;

      var modifierIsGame = Modifiers.Any(m => m.ModifierType == ModifierType.Valat || m.ModifierType == ModifierType.BarvniValat);

      if (Game == GameType.Berac || Game == GameType.OdprtiBerac || modifierIsGame)
        ScoreDifference = 0;

      if (modifierIsGame)      
        Game = 0;

      var result = wonModifier * (int)(ScoreDifference + Game) * (int)ContraFactor;

      foreach (var mod in Modifiers)
        result += (int)mod.Team *
          (mod.Announced && mod.ModifierType != ModifierType.BarvniValat? 2 : 1) *
          (int)mod.ModifierType *
          (int)mod.ContraFactor;   

      return result;
    }

    public static TarokRound FromRound(Round round)
    {
      if (round.IsKlop)
      {
        return new KlopRound
        {
          Game = GameType.Klop,
          KlopScores = round.RoundResults.ToDictionary(r => r.PlayerId,
            p => new PlayerScore(-p.PlayerScore))            
        };
      }

      return new TarokRound()
      {
        Won = round.Won,
        ScoreDifference = round.Difference,
        ContraFactor = (Contra)round.ContraFactor,
        LeadPlayer = round.LeadPlayerId,
        SupportingPLayer = round.SupportingPlayerId,
        MondFangPlayer = round.MondFangPlayerId,
        Game = (GameType)round.GameType,
        Modifiers = round.Modifiers.Select(m =>
        {
          ModifierType modType;

          switch (m.ModifierType)
          {
            case ModifierTypeDbEnum.TRULA:
              modType = ModifierType.Trula;
              break;
            case ModifierTypeDbEnum.KRALJI:
              modType = ModifierType.Kralji;
              break;
            case ModifierTypeDbEnum.KRALJ_ULTIMO:
              modType = ModifierType.KraljUltimo;
              break;
            case ModifierTypeDbEnum.PAGAT_ULTIMO:
              modType = ModifierType.PagatUltimo;
              break;
            case ModifierTypeDbEnum.BARVNI_VALAT:
              modType = ModifierType.BarvniValat;
              break;
            case ModifierTypeDbEnum.VALAT:
              modType = ModifierType.Valat;
              break;
            default:
              throw new Exception($"Can't. Ni '{m.ModifierType}'.");
          }
          return new Modifier(modType, (Shared.Enums.TeamModifier)m.Team, m.Announced, (Contra)m.Contra);
        }).ToList()
      };
    }
  }
}
