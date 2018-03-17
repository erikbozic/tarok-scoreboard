using System.Collections.Generic;
using System.Linq;
using TarokScoreBoard.Shared.DTO;

namespace TarokScoreBoard.Core.Entities
{
  public partial class Team
  {
    public List<TeamPlayer> Members { get; set; } = new List<TeamPlayer>();

    public TeamDTO ToDto()
    {
      return new TeamDTO()
      {
        TeamUserId = TeamUserId,
        TeamName = TeamName,
        TeamId = TeamId,
        Members = Members.Select(m => new TeamPlayerDTO(m.Name) {PlayerId = m.PlayerId }).ToList()
      };
    }
  }
}
