using System;
using System.Collections.Generic;
using System.Text;
using TarokScoreBoard.Core.Entities;

namespace TarokScoreBoard.Core
  {
  public class GameInitializer
  {
    private readonly IEnumerable<Guid> players;

    public GameInitializer(IEnumerable<Guid> players) // TODO Add ruleset
    {
      this.players = players;
    }

    public GameInitializer()
    {

    }

    public ScoreBoard StartGame()
    {
      var scoreBoard = new ScoreBoard(players);
      scoreBoard.Reset();
      return scoreBoard;
    }

  }
}
