using System.Collections.Generic;
using System.Linq;
using TarokScoreBoard.Shared.DTO;

namespace TarokScoreBoard.Core.Entities
{
  public partial class Round
  {
    public List<RoundResult> RoundResults { get; set; } = new List<RoundResult>();

    public List<RoundModifier> Modifiers { get; set; } = new List<RoundModifier>();

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
          new RoundModifier(m.ModifierType, m.Team, m.Announced, m.ContraFactor)).ToList(),
        RoundResults = round.KlopResults.Select(r =>
          new RoundResult() {  GameId = round.GameId, PlayerId = r.PlayerId, PlayerScore = r.Score }).ToList()
      };
    }
  }
}
