[System.Serializable]
public class PlayerData
{
    public int maxPiece;
    public int maxCourse;
    public int maxScroll;
    public int gamesJoined;
    public int gamesWon;
    public int teamGamesWon;
    public float playTime;

    public PlayerData(GameManager player)
    {
        maxPiece = player.maxPiece;
        maxScroll = player.maxScroll;
        maxCourse = player.maxCourse;
        playTime = player.playTime;
        gamesJoined = player.gamesJoined;
        gamesWon = player.gamesWon;
        teamGamesWon = player.teamGamesWon;

    }
}
