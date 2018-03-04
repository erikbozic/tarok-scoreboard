using System;
using System.Collections.Generic;
using System.Text;

namespace TarokScoreBoard.Core
  {
  public class GameInitializer
  {
    private readonly Player[] players;

    public GameInitializer(Player[] players) // TODO Add ruleset
    {
      this.players = players;
    }

    public ScoreBoard StartGame()
    {
      var scoreBoard = new ScoreBoard(players);
      scoreBoard.Reset();
      return scoreBoard;
    }

  }
}
