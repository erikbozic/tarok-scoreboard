using System.Collections.Generic;
using TarokScoreBoard.Shared.DTO;

namespace TarokScoreBoard.Core.Entities
{
  public partial class Round
  {
    public List<RoundResult> RoundResults { get; set; }

    public static Round FromCreateRoundRequest(CreateRoundRequest round)
    {
      return new Round
      {
         ContraFactor = round.ContraFactor,
         Difference = round.ScoreDifference,
         GameId = round.GameId,
         GameType = round.GameType,
         IsKlop  = round.IsKlop,
         LeadPlayerId = round.LeadPlayerId,
         SupportingPlayerId = round.SupportingPlayerId,
         MondFangPlayerId = round.MondFangPlayerId,
         Won = round.Won
      };
    }
  }
}
