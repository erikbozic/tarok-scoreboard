using System;
using System.Collections.Generic;

namespace TarokScoreBoard.Shared.DTO
{
  public class RoundDTO
  {    
    public int ContraFactor { get; set; }
        
    public int? Difference { get; set; }
        
    public Guid GameId { get; set; }
        
    public int? GameType { get; set; }
        
    public bool IsKlop { get; set; }
        
    public Guid? LeadPlayerId { get; set; }
    
    public Guid? MondFangPlayerId { get; set; }
    
    public Guid? PagatFangPlayerId { get; set; }
    
    public Guid RoundId { get; set; }
    
    public int RoundNumber { get; set; }
    
    public Guid? SupportingPlayerId { get; set; }
    
    public bool Won { get; set; }

    public List<RoundResultDTO> RoundResults { get; set; } = new List<RoundResultDTO>();

    public List<RoundModifierDTO> Modifiers { get; set; } = new List<RoundModifierDTO>();
  }
}
