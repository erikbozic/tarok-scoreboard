using System;

namespace TarokScoreBoard.Shared.DTO
{
  public class TeamPlayerDTO
  {
    public TeamPlayerDTO(string name)
    {
      this.Name = name;
    }

    public string Name { get; set; }

    public Guid PlayerId { get; set; }

  }
}