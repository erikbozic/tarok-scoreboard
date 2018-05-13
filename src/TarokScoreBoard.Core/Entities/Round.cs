using System;
using System.Linq;
using TarokScoreBoard.Shared.DTO;

namespace TarokScoreBoard.Core.Entities
{
  public partial class Round
  {
    public static Round FromCreateRoundRequest(CreateRoundDTO round)
    {
      var roundId = Guid.NewGuid();

      return new Round
      {
        RoundId = roundId,
        ContraFactor = round.ContraFactor,
        Difference = round.ScoreDifference,
        GameId = round.GameId,
        GameType = round.GameType,
        IsKlop = round.IsKlop,
        LeadPlayerId = round.LeadPlayerId,
        SupportingPlayerId = round.SupportingPlayerId,
        PagatFangPlayerId = round.PagatFangPlayerId,
        MondFangPlayerId = round.MondFangPlayerId,
        Won = round.Won,
        RoundModifier = round.Modifiers?.Select(m => 
          new RoundModifier(
            m.ModifierType,
            m.Team,
            roundId, 
            m.Announced,
            m.ContraFactor)).ToList(),
        RoundResult = round.KlopResults.Select(r =>
          new RoundResult() {  GameId = round.GameId, PlayerId = r.PlayerId, PlayerScore = r.Score, RoundId = roundId }).ToList()
      };
    }

    public RoundDTO ToDto()
    {
      return new RoundDTO()
      {
        GameId = GameId,
        ContraFactor = ContraFactor,
        Difference = Difference,
        GameType = GameType,
        IsKlop = IsKlop,
        LeadPlayerId = LeadPlayerId,
        MondFangPlayerId = MondFangPlayerId,
        PagatFangPlayerId = PagatFangPlayerId,
        RoundId = RoundId,
        RoundNumber = RoundNumber,
        Won = Won,
        SupportingPlayerId = SupportingPlayerId,
        RoundResults = RoundResult?.Select(r => new RoundResultDTO()
        {
          PlayerId = r.PlayerId,
          PlayerScore = r.PlayerScore,
          PlayerRadelcCount = r.PlayerRadelcCount,
          PlayerRadelcUsed = r.PlayerRadelcUsed,
          RoundScoreChange = r.RoundScoreChange
        }).ToList(),
        Modifiers = RoundModifier?.Select(m => new RoundModifierDTO()
        {
          Team = m.Team,
          Announced = m.Announced,
          Contra = m.Contra,
          ModifierType = m.ModifierType
        }).ToList(),
      };
    }
  }
}
