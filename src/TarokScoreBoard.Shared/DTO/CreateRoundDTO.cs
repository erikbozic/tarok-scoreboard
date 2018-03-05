using System;
using System.Collections.Generic;

namespace TarokScoreBoard.Shared.DTO
{
  public class CreateRoundDTO
  {
    public bool IsKlop { get; set; }

    public Guid GameId { get; set; }

    public Guid LeadPlayerId { get; set; }

    public Guid SupportingPlayerId { get; set; }

    public bool Won { get; set; }

    public int GameType { get; set; }

    public int ScoreDifference { get; set; }

    public List<Modifier> Modifiers { get; set; }

    public int ContraFactor { get; set; } = 1;

    public Guid MondFangPlayerId { get; set; }
  }
}
