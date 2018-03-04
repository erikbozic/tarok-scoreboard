using System;
using System.Collections.Generic;

namespace TarokScoreBoard.Shared.DTO
{
  public class Game
  {
    public Guid Guid { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    public List<Player> Players { get; set; }
  }
}
