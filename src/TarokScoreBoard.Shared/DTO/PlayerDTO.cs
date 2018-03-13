using System;

namespace TarokScoreBoard.Shared.DTO
{
  public class PlayerDTO
  {
    public PlayerDTO(string name)
    {
      Name = name;
    }
    public string Name { get; set; }

    public Guid? PlayerId { get; set; }
  }
}