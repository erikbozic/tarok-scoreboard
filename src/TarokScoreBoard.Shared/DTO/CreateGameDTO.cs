using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TarokScoreBoard.Shared.DTO
{
  public class CreateGameDTO : IValidatableObject
  {
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public List<PlayerDTO> Players { get; set; }
    
    /// <summary>
    /// Only used internally
    /// </summary>
    public Guid? TeamId { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
       if (this.Players == null || this.Players.Count < 3 )
        yield return new ValidationResult("There must be at least three players!");
    }
  }
}
