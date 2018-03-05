using System.Collections.Generic;
using System.Linq;
using TarokScoreBoard.Shared.DTO;

namespace TarokScoreBoard.Core.Entities
{
  public partial class Round
  {
    public List<RoundResult> RoundResults { get; set; }
    public List<RoundModifier> Modifiers { get; set; }

    public static Round FromCreateRoundRequest(CreateRoundDTO round)
    {
      return new Round
      {
        ContraFactor = round.ContraFactor,
        Difference = round.ScoreDifference,
        GameId = round.GameId,
        GameType = round.GameType,
        IsKlop = round.IsKlop,
        LeadPlayerId = round.LeadPlayerId,
        SupportingPlayerId = round.SupportingPlayerId,
        MondFangPlayerId = round.MondFangPlayerId,
        Won = round.Won,
        Modifiers = round.Modifiers?.Select(m => 
          new RoundModifier(m.ModifierType, m.Team, m.Announced, m.ContraFactor)).ToList()
      };
    }
  }
}
