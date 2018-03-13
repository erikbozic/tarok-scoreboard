using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TarokScoreBoard.Shared.DTO
{
  public class CreateGameDTO
  {
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public List<PlayerDTO> Players { get; set; }

    public Guid TeamId { get; set; }
  }
}
