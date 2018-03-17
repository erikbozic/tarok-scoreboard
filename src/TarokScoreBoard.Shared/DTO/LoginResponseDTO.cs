using System;

namespace TarokScoreBoard.Shared.DTO
{
  public class LoginResponseDTO
  {
    public LoginResponseDTO(Guid accessToken, TeamDTO team)
    {
      this.AccessToken = accessToken;
      this.Team = team;
    }

    public Guid AccessToken { get; set; }

    public TeamDTO Team { get; set; }
  }
}
