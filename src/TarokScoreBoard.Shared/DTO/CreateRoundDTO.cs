using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TarokScoreBoard.Shared.Enums;

namespace TarokScoreBoard.Shared.DTO
{
  public class CreateRoundDTO : IValidatableObject
  {
    public bool IsKlop { get; set; }

    public Guid GameId { get; set; }

    public Guid? LeadPlayerId { get; set; }

    public Guid? SupportingPlayerId { get; set; }

    public bool Won { get; set; }

    public int GameType { get; set; }

    public int ScoreDifference { get; set; }

    public List<ModifierDTO> Modifiers { get; set; } = new List<ModifierDTO>();

    public int ContraFactor { get; set; } = 1;

    public Guid? MondFangPlayerId { get; set; }

    public Guid? PagatFangPlayerId { get; set; }

    public List<KlopResultDTO> KlopResults { get; set; } = new List<KlopResultDTO>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
      var contraFactorsAllowed = new[] { 1, 2, 4, 8, 16 };

      if (ScoreDifference % 5 != 0)
        yield return new ValidationResult("Score must be divisable by 5.");

      if (!contraFactorsAllowed.Contains(ContraFactor))
        yield return new ValidationResult("ContraFactor must be one of 1, 2, 4, 8, or 16.");

      if (!Modifiers.All(m => contraFactorsAllowed.Contains((int)m.ContraFactor)))
        yield return new ValidationResult("Modifier contraFactors must be one of 1, 2, 4, 8 or 16.");
      
      if (!IsKlop && !Enum.IsDefined(typeof(GameType), GameType))
        yield return new ValidationResult($"Dont suppor't game type: {GameType.ToString()}");

    }
  }
}
