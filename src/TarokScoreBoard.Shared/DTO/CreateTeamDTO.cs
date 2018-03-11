using System.Collections.Generic;

namespace TarokScoreBoard.Shared.DTO
{
  public class CreateTeamDTO
  {
    public string Name { get; set; }

    public string Passphrase { get; set; }

    public List<TeamPlayerDTO> Members { get; set; }
  }
}
