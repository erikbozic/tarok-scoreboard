using System;
using System.Collections.Generic;
using TarokScoreBoard.Core;
using TarokScoreBoard.Core.Entities;
using Xunit;

namespace TarokScoreBoard.Tests
{
  public class ScoreBoardTests
  {
    private IDictionary<Guid, GamePlayer> fourPlayers;
    private GamePlayer erik;
    private GamePlayer jan;
    private GamePlayer nejc;
    private GamePlayer luka;

    public ScoreBoardTests()
    {
      erik = new GamePlayer("Erik") { PlayerId = Guid.NewGuid() };
      jan = new GamePlayer("Jan") { PlayerId = Guid.NewGuid() };
      nejc = new GamePlayer("Nejc") { PlayerId = Guid.NewGuid() };
      luka = new GamePlayer("Luka") { PlayerId = Guid.NewGuid() };

      fourPlayers = new Dictionary<Guid, GamePlayer>()
      {
        {erik.PlayerId, erik },
        {jan.PlayerId, jan },
        {luka.PlayerId,  luka },
        {nejc.PlayerId, nejc }
      };
    }

    [Fact(DisplayName = "Jan igra ena, rufa Erika, zmaga 15, trula")]
    public void Test1()
      {
        var game = new GameInitializer(fourPlayers.Keys);
        var gameId = Guid.NewGuid();
        var scoreBoard = game.StartGame(gameId);

        scoreBoard.ResetScores();

        var round = new TarokRound();

        round.LeadPlayer = jan.PlayerId;
        round.SupportingPLayer = erik.PlayerId;

        round.Modifiers.Add(new Modifier(
          ModifierType.Trula,
          Team.Playing,
          Announced.Announced
        ));

        round.Game = Core.Game.Ena;
        round.Won = true;

        round.ScoreDifference = 15;

        scoreBoard.ApplyTarokRound(round);

        Assert.True(scoreBoard.Scores[jan.PlayerId].Score == 65);
        Assert.True(scoreBoard.Scores[erik.PlayerId].Score == 65);
      }

    [Fact(DisplayName = "Jan igra ena, rufa sebe, zmaga 5")]
    public void Test2()
    {
      var game = new GameInitializer(fourPlayers.Keys);
      var gameId = Guid.NewGuid();
      var scoreBoard = game.StartGame(gameId);

      scoreBoard.ResetScores();

      var round = new TarokRound();

      round.LeadPlayer = jan.PlayerId;

      round.Game = Core.Game.Ena;
      round.Won = true;

      round.ScoreDifference = 5;

      scoreBoard.ApplyTarokRound(round);

      Assert.True(scoreBoard.Scores[jan.PlayerId].Score == 35);
    }

    [Fact(DisplayName = "Jan igra ena, rufa sebe, zgubi brez razlike")]
    public void Test3()
    {
      var game = new GameInitializer(fourPlayers.Keys);
      var gameId = Guid.NewGuid();
      var scoreBoard = game.StartGame(gameId);

      scoreBoard.ResetScores();

      var round = new TarokRound();

      round.LeadPlayer = jan.PlayerId;

      round.Game = Core.Game.Ena;
      round.Won = false;

      round.ScoreDifference = 0;

      scoreBoard.ApplyTarokRound(round);

      Assert.True(scoreBoard.Scores[jan.PlayerId].Score == -30);
    }

    [Fact(DisplayName = "Jan igra ena, rufa Lukata, zgubita brez razlike, naredita napovedan pagant ultimo")]
    public void Test4()
    {
      var game = new GameInitializer(fourPlayers.Keys);
      var gameId = Guid.NewGuid();
      var scoreBoard = game.StartGame(gameId);

      scoreBoard.ResetScores();

      var round = new TarokRound();

      round.Modifiers.Add(new Modifier
        (
         ModifierType.PagatUltimo,
         Team.Playing,
         Announced.Announced
        ));

      round.LeadPlayer = jan.PlayerId;
      round.SupportingPLayer = luka.PlayerId;

      round.Game = Core.Game.Ena;
      round.Won = false;

      round.ScoreDifference = 0;

      scoreBoard.ApplyTarokRound(round);

      Assert.True(scoreBoard.Scores[jan.PlayerId].Score == 20);
      Assert.True(scoreBoard.Scores[luka.PlayerId].Score == 20);
    }

    [Fact(DisplayName = "Jan igra ena, rufa Lukata, zmagata 20 razlike, zgubita napovedanega pagant ultimo, ki je re-kontriran")]
    public void Test5()
    {
      var game = new GameInitializer(fourPlayers.Keys);
      var gameId = Guid.NewGuid();
      var scoreBoard = game.StartGame(gameId);

      scoreBoard.ResetScores();

      var round = new TarokRound();

      round.Modifiers.Add(new Modifier
        (
         ModifierType.PagatUltimo,
         Team.NonPlaying,
         Announced.Announced,
         Contra.Re
        ));

      round.LeadPlayer = jan.PlayerId;
      round.SupportingPLayer = luka.PlayerId;

      round.Game = Core.Game.Ena;
      round.Won = true;

      round.ScoreDifference = 20;

      scoreBoard.ApplyTarokRound(round);

      Assert.True(scoreBoard.Scores[jan.PlayerId].Score == -150);
      Assert.True(scoreBoard.Scores[luka.PlayerId].Score == -150);
    }

    [Fact(DisplayName = "Jan igra ena, rufa Lukata, zmagata 20 razlike, zgubita napovedanega pagant ultimo, ki je re-kontriran, vsi imajo radelc")]
    public void Test6()
    {
      var game = new GameInitializer(fourPlayers.Keys);
      var gameId = Guid.NewGuid();
      var scoreBoard = game.StartGame(gameId);

      scoreBoard.ResetScores();
      scoreBoard.AddRadelc();
      var round = new TarokRound();

      round.Modifiers.Add(new Modifier
        (
         ModifierType.PagatUltimo,
         Team.NonPlaying,
         Announced.Announced,
         Contra.Re
        ));

      round.LeadPlayer = jan.PlayerId;
      round.SupportingPLayer = luka.PlayerId;

      round.Game = Core.Game.Ena;
      round.Won = true;

      round.ScoreDifference = 20;

      scoreBoard.ApplyTarokRound(round);

      Assert.True(scoreBoard.Scores[jan.PlayerId].Score == -300);
      Assert.True(scoreBoard.Scores[luka.PlayerId].Score == -300);
      Assert.True(scoreBoard.Scores[jan.PlayerId].RadelcCount - scoreBoard.Scores[jan.PlayerId].UsedRadelcCount == 0);
    }

    [Fact(DisplayName = "Jan zmaga berača")]
    public void Test7()
    {
      var game = new GameInitializer(fourPlayers.Keys);
      var gameId = Guid.NewGuid();
      var scoreBoard = game.StartGame(gameId);

      scoreBoard.ResetScores();

      var round = new TarokRound();

      round.Game = Core.Game.Berac;
      round.LeadPlayer = jan.PlayerId;
      round.Won = true;

      round.ScoreDifference = 0;

      scoreBoard.ApplyTarokRound(round);

      Assert.True(scoreBoard.Scores[jan.PlayerId].Score == 70);
      Assert.True(scoreBoard.Scores[jan.PlayerId].RadelcCount - scoreBoard.Scores[jan.PlayerId].UsedRadelcCount == 1);
    }

    [Fact(DisplayName = "Jan zmaga berača, z radelcem")]
    public void Test8()
    {
      var game = new GameInitializer(fourPlayers.Keys);
      var gameId = Guid.NewGuid();
      var scoreBoard = game.StartGame(gameId);

      scoreBoard.ResetScores();
      scoreBoard.AddRadelc();
      var round = new TarokRound();

      round.Game = Core.Game.Berac;
      round.LeadPlayer = jan.PlayerId;
      round.Won = true;

      round.ScoreDifference = 0;

      scoreBoard.ApplyTarokRound(round);

      Assert.True(scoreBoard.Scores[jan.PlayerId].Score == 140);
      Assert.True(scoreBoard.Scores[jan.PlayerId].RadelcCount - scoreBoard.Scores[jan.PlayerId].UsedRadelcCount == 1);
    }

    [Fact(DisplayName = "Jan zamaga odprtega berača, z radelcem in kontro")]
    public void Test9()
    {
      var game = new GameInitializer(fourPlayers.Keys);
      var gameId = Guid.NewGuid();
      var scoreBoard = game.StartGame(gameId);

      scoreBoard.ResetScores();
      scoreBoard.AddRadelc();
      var round = new TarokRound();

      round.Game = Core.Game.OdprtiBerac;
      round.LeadPlayer = jan.PlayerId;
      round.Won = true;
      round.ContraFactor = Contra.Contra;

      round.ScoreDifference = 0;

      scoreBoard.ApplyTarokRound(round);

      Assert.True(scoreBoard.Scores[jan.PlayerId].Score == 360);
      Assert.True(scoreBoard.Scores[jan.PlayerId].RadelcCount - scoreBoard.Scores[jan.PlayerId].UsedRadelcCount == 1);
    }

    [Fact(DisplayName = "Jan izgubi berača")]
    public void Test10()
    {
      var game = new GameInitializer(fourPlayers.Keys);
      var gameId = Guid.NewGuid();
      var scoreBoard = game.StartGame(gameId);

      scoreBoard.ResetScores();

      var round = new TarokRound();

      round.Game = Core.Game.Berac;
      round.LeadPlayer = jan.PlayerId;
      round.Won = false;

      round.ScoreDifference = 0;

      scoreBoard.ApplyTarokRound(round);

      Assert.True(scoreBoard.Scores[jan.PlayerId].Score == -70);
      Assert.True(scoreBoard.Scores[jan.PlayerId].RadelcCount - scoreBoard.Scores[jan.PlayerId].UsedRadelcCount == 1);
    }

    [Fact(DisplayName = "Klop, Erik 10, Jan 5, Luka 25, Nejc, 30")]
    public void Test11()
    {
      var game = new GameInitializer(fourPlayers.Keys);
      var gameId = Guid.NewGuid();
      var scoreBoard = game.StartGame(gameId);

      scoreBoard.ResetScores();

      var round = new KlopRound();
      round.Game = Core.Game.Klop;
      round.Won = false;

      round.KlopScores.Add(erik.PlayerId, new PlayerScore(-10));
      round.KlopScores.Add(jan.PlayerId, new PlayerScore(-5));
      round.KlopScores.Add(luka.PlayerId, new PlayerScore(-25));
      round.KlopScores.Add(nejc.PlayerId, new PlayerScore(-30));

      scoreBoard.ApplyTarokRound(round);

      Assert.True(scoreBoard.Scores[jan.PlayerId].Score == -5);
      Assert.True(scoreBoard.Scores[erik.PlayerId].Score == -10);
      Assert.True(scoreBoard.Scores[nejc.PlayerId].Score == -30);
      Assert.True(scoreBoard.Scores[luka.PlayerId].Score == -25);

      Assert.True(scoreBoard.Scores[jan.PlayerId].RadelcCount - scoreBoard.Scores[jan.PlayerId].UsedRadelcCount == 1);
      Assert.True(scoreBoard.Scores[erik.PlayerId].RadelcCount - scoreBoard.Scores[erik.PlayerId].UsedRadelcCount == 1);
      Assert.True(scoreBoard.Scores[nejc.PlayerId].RadelcCount - scoreBoard.Scores[nejc.PlayerId].UsedRadelcCount == 1);
      Assert.True(scoreBoard.Scores[luka.PlayerId].RadelcCount - scoreBoard.Scores[luka.PlayerId].UsedRadelcCount == 1);
    }

    [Fact(DisplayName = "Igra Jan v ena, porufa lukata, zamgata 10 razlike,  Luka zgubi monda")]
    public void Test12()
    {
      var game = new GameInitializer(fourPlayers.Keys);
      var gameId = Guid.NewGuid();
      var scoreBoard = game.StartGame(gameId);

      scoreBoard.ResetScores();

      var round = new TarokRound();

      round.Game = Core.Game.Ena;
      round.LeadPlayer = jan.PlayerId;
      round.SupportingPLayer = luka.PlayerId;
      round.Won = true;

      round.ScoreDifference = 10;

      round.MondFangPlayer = luka.PlayerId;

      scoreBoard.ApplyTarokRound(round);

      Assert.True(scoreBoard.Scores[jan.PlayerId].Score == 40);
      Assert.True(scoreBoard.Scores[luka.PlayerId].Score == 15);
    }
  }
}
