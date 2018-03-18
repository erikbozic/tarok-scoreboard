using System;
using System.Collections.Generic;

namespace TarokScoreBoard.Shared.DTO
{
  public class GameDTO
  {
    public DateTime Date { get; set; }

    public Guid GameId { get; set; }
    
    public string Name { get; set; }

    public Guid? TeamId { get; set; }

    public List<GamePlayerDTO> Players { get; set; }
  }
}
