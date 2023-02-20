using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ShowStats : MonoBehaviour
{
    public TextMeshProUGUI scrollCounter, courseCounter, pieceCounter, timeCounter;
    private GameManager gameManager;

    public void LoadStats(PlayerManager player)
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        PlayerData data = SaveSystem.LoadPlayer();

        gameManager.maxCourse = data.maxCourse;
        gameManager.maxPiece = data.maxPiece;
        gameManager.maxScroll = data.maxScroll;
        gameManager.playTime = data.playTime;
        gameManager.gamesWon = data.gamesWon;
        gameManager.gamesJoined = data.gamesJoined;
        
        scrollCounter.text = player.scrollCount.ToString();
        courseCounter.text = player.courseCount.ToString();
        pieceCounter.text = player.pieceCount.ToString();

        player.playerCanvas.accumulatedTime += 1;
        float minutes = Mathf.FloorToInt(player.playerCanvas.accumulatedTime / 60);
        float seconds = Mathf.FloorToInt(player.playerCanvas.accumulatedTime % 60);
        timeCounter.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        gameManager.playTime += player.playerCanvas.accumulatedTime;

        if (player.pieceCount >gameManager.maxPiece)
        {
            gameManager.maxPiece = player.pieceCount;
        }
        
        if (player.scrollCount > gameManager.maxScroll)
        {
            gameManager.maxScroll = player.scrollCount;
        }
        
        if (player.courseCount > gameManager.maxCourse)
        {
            gameManager.maxCourse = player.courseCount;
        }

        if (player.hasWon)
        {
            gameManager.gamesWon += 1;
        }

        gameManager.gamesJoined += 1;

        SaveSystem.SavePlayer(gameManager);
    }

    public void QuitToTitle()
    {
        gameManager.ReturnToTitle(); 
        SceneManager.LoadScene("StartMenu");
    }
}

