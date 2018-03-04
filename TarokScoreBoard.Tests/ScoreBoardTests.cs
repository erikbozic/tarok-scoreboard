using System;
using TarokScoreBoard.Core;
using Xunit;

namespace TarokScoreBoard.Tests
{
  public class ScoreBoardTests
  {
    private Player[] fourPlayers;
    private Player erik;
    private Player jan;
    private Player nejc;
    private Player luka;

    public ScoreBoardTests()
    {
      erik = new Player("Erik");
      jan = new Player("Jan");
      nejc = new Player("Nejc");
      luka = new Player("Luka");

      fourPlayers = new Player[] { erik, jan, luka, nejc };
    }

    [Fact(DisplayName = "Jan igra ena, rufa Erika, zmaga 15, trula")]
    public void Test1()
    {
      var game = new GameInitializer(fourPlayers);

      var scoreBoard = game.StartGame();

      scoreBoard.Reset();
      
      var round = new TarokRound();

      round.LeadPlayer = jan;
      round.SupportingPLayer = erik;

      round.Modifiers.Add(new Modifier(
        ModifierType.Trula,
        Team.Playing,
        Announced.Announced
      ));

      round.Game = Game.Ena;
      round.Won = true;

      round.ScoreDifference = 15;
      
      scoreBoard.ApplyTarokRound(round);

      Assert.True(scoreBoard.Scores[jan].Score == 65);
      Assert.True(scoreBoard.Scores[erik].Score == 65);
    }

    [Fact(DisplayName = "Jan igra ena, rufa sebe, zmaga 5")]
    public void Test2()
    {
      var game = new GameInitializer(fourPlayers);

      var scoreBoard = game.StartGame();

      scoreBoard.Reset();

      var round = new TarokRound();

      round.LeadPlayer = jan;

      round.Game = Game.Ena;
      round.Won = true;

      round.ScoreDifference = 5;

      scoreBoard.ApplyTarokRound(round);

      Assert.True(scoreBoard.Scores[jan].Score == 35);
    }

    [Fact(DisplayName = "Jan igra ena, rufa sebe, zgubi brez razlike")]
    public void Test3()
    {
      var game = new GameInitializer(fourPlayers);

      var scoreBoard = game.StartGame();

      scoreBoard.Reset();

      var round = new TarokRound();

      round.LeadPlayer = jan;

      round.Game = Game.Ena;
      round.Won = false;

      round.ScoreDifference = 0;

      scoreBoard.ApplyTarokRound(round);

      Assert.True(scoreBoard.Scores[jan].Score == -30);
    }

    [Fact(DisplayName = "Jan igra ena, rufa Lukata, zgubita brez razlike, naredita napovedan pagant ultimo")]
    public void Test4()
    {
      var game = new GameInitializer(fourPlayers);

      var scoreBoard = game.StartGame();

      scoreBoard.Reset();

      var round = new TarokRound();

      round.Modifiers.Add(new Modifier
        (
         ModifierType.PagatUltimo,
         Team.Playing,
         Announced.Announced
        ));

      round.LeadPlayer = jan;
      round.SupportingPLayer = luka;

      round.Game = Game.Ena;
      round.Won = false;

      round.ScoreDifference = 0;

      scoreBoard.ApplyTarokRound(round);

      Assert.True(scoreBoard.Scores[jan].Score == 20);
      Assert.True(scoreBoard.Scores[luka].Score == 20);
    }

    [Fact(DisplayName = "Jan igra ena, rufa Lukata, zmagata 20 razlike, zgubita napovedanega pagant ultimo, ki je re-kontriran")]
    public void Test5()
    {
      var game = new GameInitializer(fourPlayers);

      var scoreBoard = game.StartGame();

      scoreBoard.Reset();

      var round = new TarokRound();

      round.Modifiers.Add(new Modifier
        (
         ModifierType.PagatUltimo,
         Team.NonPlaying, // Is this correct? 
         Announced.Announced,
         Contra.Re
        ));

      round.LeadPlayer = jan;
      round.SupportingPLayer = luka;

      round.Game = Game.Ena;
      round.Won = true;

      round.ScoreDifference = 20;

      scoreBoard.ApplyTarokRound(round);

      Assert.True(scoreBoard.Scores[jan].Score == -150 );
      Assert.True(scoreBoard.Scores[luka].Score == -150);
    }
  }


}
