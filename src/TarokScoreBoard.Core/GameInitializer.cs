using System;
using System.Collections.Generic;

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

    public ScoreBoard StartGame(Guid gameId)
    {
      var scoreBoard = new ScoreBoard(players)
      {
        GameId = gameId
      };
      scoreBoard.ResetScores();
      return scoreBoard;
    }

  }
}
