namespace TarokScoreBoard.Shared.DTO
{
  public class RoundModifierDTO
  {    
    public bool Announced { get; set; }
        
    public int Contra { get; set; }
        
    public string ModifierType { get; set; }
    
    public int Team { get; set; }
  }
}
