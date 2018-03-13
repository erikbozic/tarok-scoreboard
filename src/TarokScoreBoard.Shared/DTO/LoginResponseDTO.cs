using System;

namespace TarokScoreBoard.Shared.DTO
{
  public class LoginResponseDTO
  {
    public LoginResponseDTO(Guid accessToken)
    {
      this.AccessToken = accessToken;
    }

    public Guid AccessToken { get; set; }    
  }
}
