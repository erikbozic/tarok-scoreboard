namespace TarokScoreBoard.Core
{
  public class PlayerScore
  {
    public PlayerScore(int score = 0)
    {
      _score = score;
    }

    private int _score;

    public int Score { get => _score;}

    public int RadelcCount { get; private set; }

    public int UsedRadelcCount { get; private set; }

    public void AddRadelc()
    {
      RadelcCount++;
    }

    public bool HasRadelc()
    {
      return RadelcCount > UsedRadelcCount;
    }

    public void RemoveRadelc()
    {
      UsedRadelcCount++;
    }

    public int ChangeScore(int change)
    {
      // TODO log change 
      _score += change;
      return Score;
    }
  }
}