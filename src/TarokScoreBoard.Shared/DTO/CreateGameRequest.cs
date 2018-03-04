using System.Collections.Generic;

namespace TarokScoreBoard.Shared.DTO
{
  public class CreateGameRequest
  {
    public string Name { get; set; } = string.Empty;

    public List<Player> Players { get; set; }
  }
}
