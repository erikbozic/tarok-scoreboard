using System;

namespace TarokScoreBoard.Shared.DTO
{
  public class GamePlayerDTO
  {
    public string Name { get; set; }

    public Guid PlayerId { get; set; }
    public int? Position { get; set; }
  }
}