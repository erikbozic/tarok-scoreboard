using System;

namespace TarokScoreBoard.Shared.DTO
{
  public class RoundResultDTO
  {        
    public Guid PlayerId { get; set; }
        
    public int PlayerRadelcCount { get; set; }
        
    public int PlayerRadelcUsed { get; set; }
        
    public int PlayerScore { get; set; }
        
    public int? RoundScoreChange { get; set; }
  }
}
