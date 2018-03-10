using TarokScoreBoard.Shared.Enums;

namespace TarokScoreBoard.Shared.DTO
{
  public class ModifierDTO
  {
    public string ModifierType { get; set; }

    public Team Team { get; set; }

    public bool Announced { get; set; }

    public Contra ContraFactor { get; set; }
  }
}