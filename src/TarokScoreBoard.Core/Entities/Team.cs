using System.Collections.Generic;

namespace TarokScoreBoard.Core.Entities
{
  public partial class Team
  {
    public List<TeamPlayer> Members { get; set; } = new List<TeamPlayer>();
  }
}
