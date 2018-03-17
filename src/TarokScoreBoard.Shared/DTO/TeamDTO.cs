using System;
using System.Collections.Generic;

namespace TarokScoreBoard.Shared.DTO
{
  public class TeamDTO
  {
    public Guid TeamId { get; set; }

    public string TeamName { get; set; }

    public string TeamUserId { get; set; }

    public List<TeamPlayerDTO> Members { get; set; } = new List<TeamPlayerDTO>();

  }
}
