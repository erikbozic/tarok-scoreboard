namespace TarokScoreBoard.Core
{
  public class Modifier
  {

    public Modifier(ModifierType modifierType, Team team, Announced announced = Announced.NotAnnounced, Contra contra = Contra.None)
    {
      ModifierType = modifierType;
      Team = team;
      Announced = announced;
      ContraFactor = contra;
    }
    public ModifierType ModifierType { get; }

    public Team Team { get; }

    public Announced Announced { get; }

    public Contra ContraFactor { get; }
  }
}