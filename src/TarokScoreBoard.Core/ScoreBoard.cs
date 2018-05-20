using System;
using System.Collections.Generic;
using System.Linq;
using TarokScoreBoard.Core.Entities;
/*
KLOP v 4:
ŠT. POBRANIH KART (da P1 ostane ena karta pri štetju - posledično P2 ostaneta 2)
P1: 4  10 13  16  19  22  25  28  31  34  37  40  46 49
P2: 50 44 41  38  35  32  29  26  23  20  17  14  8  5   

Če ima P1 35 in mu ostane karta, ali ima lahko P2 36 (35 + 2 karte) ?? Če je to res sta lahko dva polna.

ŠT. POBRANIH KART (da p1 ne ostane nobena pri štetju)
P1: 9  12  15  18  21  24  27  30  33  36  39  42  45
P2: 45 42  39  36  33  30  27  24  21  18  15  12  9  

Imata lahko oba točno 35 brez dodatne karte kar pomeni da sta oba prazna - makes sense

Možno št. pobranih kart:
0, 4,5  8,9,10  12,13,14,15  16,17,18,19,20  21,22,23,24,25  26,27,28,29  30,31,32,33,34,  35,36,37,38,  39,40,41,42,  43,44,45,46  47,48,49,50, 54

torej ne moreš pobrat manj kot 4 in pa tudi ne 6, 7 ali 11 kart.

 */
namespace TarokScoreBoard.Core
{
  public class ScoreBoard
  {
    private IEnumerable<Guid> players;

    public IDictionary<Guid, PlayerScore>  Scores { get; set; }

    public Guid GameId { get; set; }

    public ScoreBoard(IEnumerable<Guid> players)
    {
      this.players = players;      
    }

    private ScoreBoard()
    {
    }

    public void AddRadelc()
    {
      foreach (var score in Scores)
        score.Value.AddRadelc();
    }

    public void ResetScores()
    {
      Scores = new Dictionary<Guid, PlayerScore>();

      foreach (var player in players)
        Scores.Add(player, new PlayerScore());      
    }

    private void ChangeScore(Guid playerId, int baseScore) => 
      Scores[playerId].ChangeScore(baseScore * (Scores[playerId].HasRadelc() ? 2 : 1));

    public IDictionary<Guid, PlayerScore> ApplyTarokRound(TarokRound round)
    {
      if (round is KlopRound klop)
      { 
        round.Game = Shared.Enums.GameType.Klop;

        var fullPlayer = klop.KlopScores.FirstOrDefault(s => s.Value.Score < -35);
        var emptyPlayers = klop.KlopScores.Where(s => s.Value.Score == 0);
        
        if (emptyPlayers.Any() && fullPlayer.Value != null)
        {
          ChangeScore(fullPlayer.Key, -35);
          
          foreach(var empty in emptyPlayers)
          {
            ChangeScore(empty.Key, 35);

            if (Scores[empty.Key].HasRadelc())
              Scores[empty.Key].RemoveRadelc();
          }
        }
        else if (!emptyPlayers.Any() && fullPlayer.Value != null)
          ChangeScore(fullPlayer.Key, -70);
        else if (emptyPlayers.Any() && fullPlayer.Value == null)
        {
          foreach (var emptyPlayer in emptyPlayers)
          {
            ChangeScore(emptyPlayer.Key, (70/emptyPlayers.Count()));

            if (Scores[emptyPlayer.Key].HasRadelc())
              Scores[emptyPlayer.Key].RemoveRadelc(); 
          }
        }
        else
        {
          foreach (var klopScore in klop.KlopScores)
          {
            var score = klopScore.Value.Score == -1 ? 0 : klopScore.Value.Score; // TODO ugly workaround. Solve this better.
            Scores[klopScore.Key].ChangeScore(score * (Scores[klopScore.Key].HasRadelc() ? 2 : 1));
          }
        }
      }
      else
      {
        var hasRadelc = Scores[round.LeadPlayer.Value].HasRadelc();
        var roundScore = round.GetScore() * (hasRadelc ? 2 : 1);

        if (hasRadelc && round.Won)
          Scores[round.LeadPlayer.Value].RemoveRadelc();

        Scores[round.LeadPlayer.Value].ChangeScore(roundScore);
        if (round.SupportingPlayer != null)
          Scores[round.SupportingPlayer.Value].ChangeScore(roundScore);
      }

      if((int)round.Game >= 70)
        this.AddRadelc();
      

      if (round.MondFangPlayer != null)
        Scores[round.MondFangPlayer.Value].ChangeScore(-20);

      if (round.PagatFangPlayer != null)
        Scores[round.PagatFangPlayer.Value].ChangeScore(-25);

      return Scores;
    }

    public IDictionary<Guid, PlayerScore>  EndGame()
    {
      foreach (var score in Scores)
      {
        var leftRadelc = score.Value.RadelcCount - score.Value.UsedRadelcCount;
        
        score.Value.ChangeScore(leftRadelc * -100);
      }

      return this.Scores;
    }

    public static ScoreBoard FromRound(IEnumerable<RoundResult> roundResults)
    {
      var sb = new ScoreBoard
      {
        GameId = roundResults.First().GameId,
        Scores = new Dictionary<Guid, PlayerScore>()
      };

      foreach (var result in roundResults)
        sb.Scores.Add(result.PlayerId, PlayerScore.FromRoundResult(result));      
      
      return sb;
    }
  }
}
