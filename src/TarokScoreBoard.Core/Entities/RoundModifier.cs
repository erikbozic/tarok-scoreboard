using System;
using TarokScoreBoard.Shared.Enums;

namespace TarokScoreBoard.Core.Entities
{
  public partial class RoundModifier
  {
    public RoundModifier()
    {

    }

    public RoundModifier(string modifierType, Team team, Guid roundId, Announced announced = Shared.Enums.Announced.NotAnnounced, Contra contra = Shared.Enums.Contra.None)
    {
      this.ModifierType = modifierType;
      this.Team = (int)team;
      this.Announced = (int)announced;
      this.Contra = (int)contra;
      this.RoundId = roundId;
    }
  }
}
