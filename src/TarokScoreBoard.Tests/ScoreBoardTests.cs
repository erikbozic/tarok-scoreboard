using System;
using System.Collections.Generic;
using TarokScoreBoard.Core;
using TarokScoreBoard.Core.Entities;
using Xunit;

namespace TarokScoreBoard.Tests
{
  public class ScoreBoardTests
  {
    private IEnumerable<GamePlayer> fourPlayers;
    private GamePlayer erik;
    private GamePlayer jan;
    private GamePlayer nejc;
    private GamePlayer luka;

    public ScoreBoardTests()
    {
      erik = new GamePlayer("Erik");
      jan = new GamePlayer("Jan");
      nejc = new GamePlayer("Nejc");
      luka = new GamePlayer("Luka");

      fourPlayers = new List<GamePlayer> { erik, jan, luka, nejc };
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

      round.Game = Core.Game.Ena;
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

      round.Game = Core.Game.Ena;
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

      round.Game = Core.Game.Ena;
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

      round.Game = Core.Game.Ena;
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
         Team.NonPlaying,
         Announced.Announced,
         Contra.Re
        ));

      round.LeadPlayer = jan;
      round.SupportingPLayer = luka;

      round.Game = Core.Game.Ena;
      round.Won = true;

      round.ScoreDifference = 20;

      scoreBoard.ApplyTarokRound(round);

      Assert.True(scoreBoard.Scores[jan].Score == -150 );
      Assert.True(scoreBoard.Scores[luka].Score == -150);
    }

    [Fact(DisplayName = "Jan igra ena, rufa Lukata, zmagata 20 razlike, zgubita napovedanega pagant ultimo, ki je re-kontriran, vsi imajo radelc")]
    public void Test6()
    {
      var game = new GameInitializer(fourPlayers);

      var scoreBoard = game.StartGame();

      scoreBoard.Reset();
      scoreBoard.AddRadelc();
      var round = new TarokRound();

      round.Modifiers.Add(new Modifier
        (
         ModifierType.PagatUltimo,
         Team.NonPlaying, 
         Announced.Announced,
         Contra.Re
        ));

      round.LeadPlayer = jan;
      round.SupportingPLayer = luka;

      round.Game = Core.Game.Ena;
      round.Won = true;

      round.ScoreDifference = 20;

      scoreBoard.ApplyTarokRound(round);

      Assert.True(scoreBoard.Scores[jan].Score == -300);
      Assert.True(scoreBoard.Scores[luka].Score == -300);
      Assert.True(scoreBoard.Scores[jan].RadelcCount - scoreBoard.Scores[jan].UsedRadelcCount == 0);
    }

    [Fact(DisplayName = "Jan zmaga berača")]
    public void Test7()
    {
      var game = new GameInitializer(fourPlayers);

      var scoreBoard = game.StartGame();

      scoreBoard.Reset();

      var round = new TarokRound();

      round.Game = Core.Game.Berac;
      round.LeadPlayer = jan;
      round.Won = true;

      round.ScoreDifference = 0;

      scoreBoard.ApplyTarokRound(round);

      Assert.True(scoreBoard.Scores[jan].Score == 70);
      Assert.True(scoreBoard.Scores[jan].RadelcCount - scoreBoard.Scores[jan].UsedRadelcCount == 1);
    }

    [Fact(DisplayName = "Jan zmaga berača, z radelcem")]
    public void Test8()
    {
      var game = new GameInitializer(fourPlayers);

      var scoreBoard = game.StartGame();

      scoreBoard.Reset();
      scoreBoard.AddRadelc();
      var round = new TarokRound();
      
      round.Game = Core.Game.Berac;
      round.LeadPlayer = jan;
      round.Won = true;

      round.ScoreDifference = 0;

      scoreBoard.ApplyTarokRound(round);

      Assert.True(scoreBoard.Scores[jan].Score == 140);
      Assert.True(scoreBoard.Scores[jan].RadelcCount - scoreBoard.Scores[jan].UsedRadelcCount == 1);
    }

    [Fact(DisplayName = "Jan zamaga odprtega berača, z radelcem in kontro")]
    public void Test9()
    {
      var game = new GameInitializer(fourPlayers);

      var scoreBoard = game.StartGame();

      scoreBoard.Reset();
      scoreBoard.AddRadelc();
      var round = new TarokRound();

      round.Game = Core.Game.OdprtiBerac;
      round.LeadPlayer = jan;
      round.Won = true;
      round.ContraFactor = Contra.Contra;

      round.ScoreDifference = 0;

      scoreBoard.ApplyTarokRound(round);

      Assert.True(scoreBoard.Scores[jan].Score == 360);
      Assert.True(scoreBoard.Scores[jan].RadelcCount - scoreBoard.Scores[jan].UsedRadelcCount == 1);
    }

    [Fact(DisplayName = "Jan izgubi berača")]
    public void Test10()
    {
      var game = new GameInitializer(fourPlayers);

      var scoreBoard = game.StartGame();

      scoreBoard.Reset();

      var round = new TarokRound();

      round.Game = Core.Game.Berac;
      round.LeadPlayer = jan;
      round.Won = false;

      round.ScoreDifference = 0;

      scoreBoard.ApplyTarokRound(round);

      Assert.True(scoreBoard.Scores[jan].Score == -70);
      Assert.True(scoreBoard.Scores[jan].RadelcCount - scoreBoard.Scores[jan].UsedRadelcCount == 1);
    }

    [Fact(DisplayName = "Klop, Erik 10, Jan 5, Luka 25, Nejc, 30")]
    public void Test11()
    {
      var game = new GameInitializer(fourPlayers);

      var scoreBoard = game.StartGame();

      scoreBoard.Reset();

      var round = new KlopRound();
      round.Game = Core.Game.Klop;
      round.Won = false;

      round.KlopScores.Add(erik, new PlayerScore(-10));
      round.KlopScores.Add(jan, new PlayerScore(-5));
      round.KlopScores.Add(luka, new PlayerScore(-25));
      round.KlopScores.Add(nejc, new PlayerScore(-30));

      scoreBoard.ApplyTarokRound(round);

      Assert.True(scoreBoard.Scores[jan].Score == -5);
      Assert.True(scoreBoard.Scores[erik].Score == -10);
      Assert.True(scoreBoard.Scores[nejc].Score == -30);
      Assert.True(scoreBoard.Scores[luka].Score == -25);

      Assert.True(scoreBoard.Scores[jan].RadelcCount -  scoreBoard.Scores[jan].UsedRadelcCount == 1);
      Assert.True(scoreBoard.Scores[erik].RadelcCount - scoreBoard.Scores[erik].UsedRadelcCount == 1);
      Assert.True(scoreBoard.Scores[nejc].RadelcCount - scoreBoard.Scores[nejc].UsedRadelcCount == 1);
      Assert.True(scoreBoard.Scores[luka].RadelcCount - scoreBoard.Scores[luka].UsedRadelcCount == 1);
    }

    [Fact(DisplayName = "Igra Jan v ena, porufa lukata, zamgata 10 razlike,  Luka zgubi monda")]
    public void Test12()
    {
      var game = new GameInitializer(fourPlayers);

      var scoreBoard = game.StartGame();

      scoreBoard.Reset();

      var round = new TarokRound();

      round.Game = Core.Game.Ena;
      round.LeadPlayer = jan;
      round.SupportingPLayer = luka;
      round.Won = true;

      round.ScoreDifference = 10;

      round.MondFangPlayer = luka;

      scoreBoard.ApplyTarokRound(round);

      Assert.True(scoreBoard.Scores[jan].Score == 40);
      Assert.True(scoreBoard.Scores[luka].Score == 15);
    }
  }
}
