[System.Serializable]
public class GameRecord
{
    public int playerID { get; set; }
    public int minigameID { get; set; }
    public System.DateTime playedAt { get; set; }
    public int score { get; set; }
}