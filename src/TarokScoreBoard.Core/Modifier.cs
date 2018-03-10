using TarokScoreBoard.Shared.Enums;

namespace TarokScoreBoard.Core
{
  public class Modifier
  {
    public Modifier(ModifierType modifierType, Team team, bool announced, Contra contra = Contra.None)
    {
      ModifierType = modifierType;
      Team = team;
      Announced = announced;
      ContraFactor = contra;
    }
    public ModifierType ModifierType { get; }

    public Team Team { get; }

    public bool Announced { get; }

    public Contra ContraFactor { get; }
  }
}