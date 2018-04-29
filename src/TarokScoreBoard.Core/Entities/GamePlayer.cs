using System.ComponentModel.DataAnnotations.Schema;

namespace TarokScoreBoard.Core.Entities
{
  public partial class GamePlayer
  {
    public GamePlayer()
    {

    }

    public GamePlayer(string name)
    {
      Name = name;
    }

    [NotMapped] // TODO does dapper honor this?
    public bool IsMaestro { get; set; }
  }
}
