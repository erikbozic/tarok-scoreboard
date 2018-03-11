using System;
using TarokScoreBoard.Shared.Enums;

namespace TarokScoreBoard.Core.Entities
{
  public partial class RoundModifier
  {
    public RoundModifier()
    {

    }

    public RoundModifier(string modifierType, Shared.Enums.TeamModifier team, Guid roundId, bool announced = false, Contra contra = Shared.Enums.Contra.None)
    {
      this.ModifierType = modifierType;
      this.Team = (int)team;
      this.Announced = announced;
      this.Contra = (int)contra;
      this.RoundId = roundId;
    }
  }
}
