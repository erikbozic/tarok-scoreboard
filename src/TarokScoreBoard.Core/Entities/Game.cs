using System.Collections.Generic;
using System.Linq;
using TarokScoreBoard.Shared.DTO;

namespace TarokScoreBoard.Core.Entities
{
  partial class Game
  {
    public List<GamePlayer> Players { get; set; }

    public GameDTO ToDto()
    {
      return new GameDTO()
      {
        GameId = GameId,
        Date = Date,
        Name = Name,
        TeamId = TeamId,
        Players = Players?.Select(p => new GamePlayerDTO()
        {
          Name = p.Name,
          PlayerId = p.PlayerId,
          Position = p.Position,
          IsMaestro = p.IsMaestro
        }).ToList()
      };
    }
  }
}
