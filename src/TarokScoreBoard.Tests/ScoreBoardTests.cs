﻿using System;
using System.Collections.Generic;
using TarokScoreBoard.Core;
using TarokScoreBoard.Core.Entities;
using TarokScoreBoard.Shared.Enums;
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

      var round = new TarokRound
      {
        LeadPlayer = jan.PlayerId,
        SupportingPlayer = erik.PlayerId,
        Game = GameType.Ena,
        Won = true,
        ScoreDifference = 15
      };

      round.Modifiers.Add(new Modifier(
          ModifierType.Trula,
          TeamModifier.Playing,
          announced: true
        ));

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

      var round = new TarokRound
      {
        LeadPlayer = jan.PlayerId,
        Game = GameType.Ena,
        Won = true,
        ScoreDifference = 5
      };

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

      var round = new TarokRound
      {
        LeadPlayer = jan.PlayerId,
        Game = GameType.Ena,
        Won = false,
        ScoreDifference = 0
      };

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

      var round = new TarokRound
      {
        LeadPlayer = jan.PlayerId,
        SupportingPlayer = luka.PlayerId,
        Game = GameType.Ena,
        Won = false,
        ScoreDifference = 0
      };

      round.Modifiers.Add(new Modifier
       (
        ModifierType.PagatUltimo,
        TeamModifier.Playing,
        announced: true
       ));

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

      var round = new TarokRound
      {
        LeadPlayer = jan.PlayerId,
        SupportingPlayer = luka.PlayerId,
        Game = GameType.Ena,
        Won = true,
        ScoreDifference = 20
      };

      round.Modifiers.Add(new Modifier
      (
       ModifierType.PagatUltimo,
       TeamModifier.NonPlaying,
       announced: true,
       contra: Contra.Re
      ));

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
      var round = new TarokRound
      {
        LeadPlayer = jan.PlayerId,
        SupportingPlayer = luka.PlayerId,
        Game = GameType.Ena,
        Won = true,
        ScoreDifference = 20
      };

      round.Modifiers.Add(new Modifier
        (
         ModifierType.PagatUltimo,
         TeamModifier.NonPlaying,
         announced: true,
         contra: Contra.Re
        ));

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

      var round = new TarokRound
      {
        Game = GameType.Berac,
        LeadPlayer = jan.PlayerId,
        Won = true,
        ScoreDifference = 0
      };

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
      var round = new TarokRound
      {
        Game = GameType.Berac,
        LeadPlayer = jan.PlayerId,
        Won = true,
        ScoreDifference = 0
      };

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
      var round = new TarokRound
      {
        Game = GameType.OdprtiBerac,
        LeadPlayer = jan.PlayerId,
        Won = true,
        ContraFactor = Contra.Contra,
        ScoreDifference = 0
      };

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

      var round = new TarokRound
      {
        Game = GameType.Berac,
        LeadPlayer = jan.PlayerId,
        Won = false,
        ScoreDifference = 0
      };

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

      var round = new KlopRound
      {
        Game = GameType.Klop,
        Won = false
      };

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

    [Fact(DisplayName = "Igra Jan v ena, porufa Lukata, zmagata 10 razlike,  Luka zgubi monda")]
    public void Test12()
    {
      var game = new GameInitializer(fourPlayers.Keys);
      var gameId = Guid.NewGuid();
      var scoreBoard = game.StartGame(gameId);

      scoreBoard.ResetScores();

      var round = new TarokRound
      {
        Game = GameType.Ena,
        LeadPlayer = jan.PlayerId,
        SupportingPlayer = luka.PlayerId,
        Won = true,
        ScoreDifference = 10,
        MondFangPlayer = luka.PlayerId
      };

      scoreBoard.ApplyTarokRound(round);

      Assert.True(scoreBoard.Scores[jan.PlayerId].Score == 40);
      Assert.True(scoreBoard.Scores[luka.PlayerId].Score == 20);
    }

    [Fact(DisplayName = "Klop, Erik 0, Jan 0, Luka 35, Nejc, 35")]
    public void Test13()
    {
      var game = new GameInitializer(fourPlayers.Keys);
      var gameId = Guid.NewGuid();
      var scoreBoard = game.StartGame(gameId);

      scoreBoard.ResetScores();

      var round = new KlopRound
      {
        Game = GameType.Klop,
        Won = false
      };

      round.KlopScores.Add(erik.PlayerId, new PlayerScore(0));
      round.KlopScores.Add(jan.PlayerId, new PlayerScore(0));
      round.KlopScores.Add(luka.PlayerId, new PlayerScore(-35));
      round.KlopScores.Add(nejc.PlayerId, new PlayerScore(-35));

      scoreBoard.ApplyTarokRound(round);

      Assert.True(scoreBoard.Scores[jan.PlayerId].Score == 35);
      Assert.True(scoreBoard.Scores[erik.PlayerId].Score == 35);
      Assert.True(scoreBoard.Scores[nejc.PlayerId].Score == 0);
      Assert.True(scoreBoard.Scores[luka.PlayerId].Score == 0);

      Assert.True(scoreBoard.Scores[jan.PlayerId].RadelcCount - scoreBoard.Scores[jan.PlayerId].UsedRadelcCount == 1);
      Assert.True(scoreBoard.Scores[erik.PlayerId].RadelcCount - scoreBoard.Scores[erik.PlayerId].UsedRadelcCount == 1);
      Assert.True(scoreBoard.Scores[nejc.PlayerId].RadelcCount - scoreBoard.Scores[nejc.PlayerId].UsedRadelcCount == 1);
      Assert.True(scoreBoard.Scores[luka.PlayerId].RadelcCount - scoreBoard.Scores[luka.PlayerId].UsedRadelcCount == 1);
    }

    [Fact(DisplayName = "Jan igra v ena, Luka zgubi monda, Erik zgubi zadnjega pagata nenapovedano")]
    public void Test14()
    {
      var game = new GameInitializer(fourPlayers.Keys);
      var gameId = Guid.NewGuid();
      var scoreBoard = game.StartGame(gameId);

      scoreBoard.ResetScores();

      var round = new TarokRound
      {
        Game = GameType.Ena,
        LeadPlayer = jan.PlayerId,
        SupportingPlayer = luka.PlayerId,
        Won = true,
        ScoreDifference = 10,
        MondFangPlayer = luka.PlayerId,
        PagatFangPlayer = erik.PlayerId
      };

      scoreBoard.ApplyTarokRound(round);

      Assert.True(scoreBoard.Scores[jan.PlayerId].Score == 40);
      Assert.True(scoreBoard.Scores[luka.PlayerId].Score == 20);
      Assert.True(scoreBoard.Scores[erik.PlayerId].Score == -25);
    }

    [Fact(DisplayName = "Jan igra v ena, rufa Luka naredita valata")]
    public void Test15()
    {
      var game = new GameInitializer(fourPlayers.Keys);
      var gameId = Guid.NewGuid();
      var scoreBoard = game.StartGame(gameId);

      scoreBoard.ResetScores();

      var round = new TarokRound
      {
        Game = GameType.Ena,
        LeadPlayer = jan.PlayerId,
        SupportingPlayer = luka.PlayerId,
        Won = true,
        ScoreDifference = 30,
      };

      round.Modifiers.Add(new Modifier(ModifierType.Valat, TeamModifier.Playing, false));

      scoreBoard.ApplyTarokRound(round);

      Assert.True(scoreBoard.Scores[jan.PlayerId].Score == 250);
      Assert.True(scoreBoard.Scores[luka.PlayerId].Score == 250);
    }

    [Fact(DisplayName = "Erik gre barvnega valata in zmaga")]
    public void Test16()
    {
      var game = new GameInitializer(fourPlayers.Keys);
      var gameId = Guid.NewGuid();
      var scoreBoard = game.StartGame(gameId);

      scoreBoard.ResetScores();

      var round = new TarokRound
      {
        Game = GameType.SoloTri,
        LeadPlayer = erik.PlayerId,
        Won = true,
        ScoreDifference = 30,
      };

      round.Modifiers.Add(new Modifier(ModifierType.BarvniValat, TeamModifier.Playing, true));

      scoreBoard.ApplyTarokRound(round);

      Assert.True(scoreBoard.Scores[erik.PlayerId].Score == 125);
    }

    [Fact(DisplayName = "Klop, Erik 10, Jan 36, Luka 0, Nejc, 24")]
    public void Test17()
    {
      var game = new GameInitializer(fourPlayers.Keys);
      var gameId = Guid.NewGuid();
      var scoreBoard = game.StartGame(gameId);

      scoreBoard.ResetScores();

      var round = new KlopRound
      {
        Game = GameType.Klop,
        Won = false
      };

      round.KlopScores.Add(jan.PlayerId, new PlayerScore(-36));
      round.KlopScores.Add(erik.PlayerId, new PlayerScore(-10));
      round.KlopScores.Add(nejc.PlayerId, new PlayerScore(-24));
      round.KlopScores.Add(luka.PlayerId, new PlayerScore(-0));

      scoreBoard.ApplyTarokRound(round);

      Assert.True(scoreBoard.Scores[jan.PlayerId].Score == -35);
      Assert.True(scoreBoard.Scores[erik.PlayerId].Score == 0);
      Assert.True(scoreBoard.Scores[nejc.PlayerId].Score == 0);
      Assert.True(scoreBoard.Scores[luka.PlayerId].Score == 35);

      Assert.True(scoreBoard.Scores[jan.PlayerId].RadelcCount - scoreBoard.Scores[jan.PlayerId].UsedRadelcCount == 1);
      Assert.True(scoreBoard.Scores[erik.PlayerId].RadelcCount - scoreBoard.Scores[erik.PlayerId].UsedRadelcCount == 1);
      Assert.True(scoreBoard.Scores[nejc.PlayerId].RadelcCount - scoreBoard.Scores[nejc.PlayerId].UsedRadelcCount == 1);
      Assert.True(scoreBoard.Scores[luka.PlayerId].RadelcCount - scoreBoard.Scores[luka.PlayerId].UsedRadelcCount == 1);
    }

    [Fact(DisplayName = "Klop, Erik 10, Jan 1, Luka 30, Nejc, 30")]
    public void Test18()
    {
      var game = new GameInitializer(fourPlayers.Keys);
      var gameId = Guid.NewGuid();
      var scoreBoard = game.StartGame(gameId);

      scoreBoard.ResetScores();

      var round = new KlopRound
      {
        Game = GameType.Klop,
        Won = false
      };

      round.KlopScores.Add(jan.PlayerId, new PlayerScore(-1));
      round.KlopScores.Add(erik.PlayerId, new PlayerScore(-10));
      round.KlopScores.Add(nejc.PlayerId, new PlayerScore(-30));
      round.KlopScores.Add(luka.PlayerId, new PlayerScore(-30));

      scoreBoard.ApplyTarokRound(round);

      Assert.True(scoreBoard.Scores[jan.PlayerId].Score == 0);
      Assert.True(scoreBoard.Scores[erik.PlayerId].Score == -10);
      Assert.True(scoreBoard.Scores[nejc.PlayerId].Score == -30);
      Assert.True(scoreBoard.Scores[luka.PlayerId].Score == -30);

      Assert.True(scoreBoard.Scores[jan.PlayerId].RadelcCount - scoreBoard.Scores[jan.PlayerId].UsedRadelcCount == 1);
      Assert.True(scoreBoard.Scores[erik.PlayerId].RadelcCount - scoreBoard.Scores[erik.PlayerId].UsedRadelcCount == 1);
      Assert.True(scoreBoard.Scores[nejc.PlayerId].RadelcCount - scoreBoard.Scores[nejc.PlayerId].UsedRadelcCount == 1);
      Assert.True(scoreBoard.Scores[luka.PlayerId].RadelcCount - scoreBoard.Scores[luka.PlayerId].UsedRadelcCount == 1);

    }

    [Fact(DisplayName = "Valat nasprotna ekipa, kontra")]
    public void Test19()
    {
      var game = new GameInitializer(fourPlayers.Keys);
      var gameId = Guid.NewGuid();
      var scoreBoard = game.StartGame(gameId);

      scoreBoard.ResetScores();

      var round = new TarokRound
      {
        Game = GameType.Ena,
        Won = false,
        Modifiers = new List<Modifier>()
        {
          new Modifier(ModifierType.Valat, TeamModifier.NonPlaying, false)
        },
        ContraFactor = Contra.Contra,
        ScoreDifference = 35,
        LeadPlayer = fourPlayers[nejc.PlayerId].PlayerId,
        SupportingPlayer = fourPlayers[luka.PlayerId].PlayerId,
      };

      scoreBoard.ApplyTarokRound(round);

      Assert.True(scoreBoard.Scores[jan.PlayerId].Score == 0);
      Assert.True(scoreBoard.Scores[erik.PlayerId].Score == 0);
      Assert.True(scoreBoard.Scores[nejc.PlayerId].Score == -500, "Nejc ima -500");
      Assert.True(scoreBoard.Scores[luka.PlayerId].Score == -500, "Luka ima -500");

      Assert.True(scoreBoard.Scores[jan.PlayerId].RadelcCount - scoreBoard.Scores[jan.PlayerId].UsedRadelcCount == 0);
      Assert.True(scoreBoard.Scores[erik.PlayerId].RadelcCount - scoreBoard.Scores[erik.PlayerId].UsedRadelcCount == 0);
      Assert.True(scoreBoard.Scores[nejc.PlayerId].RadelcCount - scoreBoard.Scores[nejc.PlayerId].UsedRadelcCount == 0);
      Assert.True(scoreBoard.Scores[luka.PlayerId].RadelcCount - scoreBoard.Scores[luka.PlayerId].UsedRadelcCount == 0);

    }

  }
}
