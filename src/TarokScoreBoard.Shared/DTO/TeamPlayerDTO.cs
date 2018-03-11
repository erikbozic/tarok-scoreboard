namespace TarokScoreBoard.Shared.DTO
{
  public class TeamPlayerDTO
  {
    public TeamPlayerDTO(string name)
    {
      this.Name = name;
    }

    public string Name { get; set; }
  }
}