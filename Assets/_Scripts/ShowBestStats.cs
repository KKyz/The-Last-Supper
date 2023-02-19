using TMPro;
using UnityEngine;

public class ShowBestStats : MonoBehaviour
{
    private TextMeshProUGUI joinCounter, winCounter, maxPiece, maxCourse, timeCounter, maxScroll;
    //Get stats to show in menu
    void Start()
    {
        PlayerData data = SaveSystem.LoadPlayer();
        
        joinCounter = transform.Find("JoinCounter").GetComponent<TextMeshProUGUI>();
        winCounter = transform.Find("WinCounter").GetComponent<TextMeshProUGUI>();
        maxPiece = transform.Find("PieceCounter").GetComponent<TextMeshProUGUI>();
        maxCourse = transform.Find("CourseCounter").GetComponent<TextMeshProUGUI>();
        timeCounter = transform.Find("TimeCounter").GetComponent<TextMeshProUGUI>();
        maxScroll = transform.Find("ScrollCounter").GetComponent<TextMeshProUGUI>();
        
        
        
        float playTime = PlayerPrefs.GetFloat("playTime", 0);
        float minutes = Mathf.FloorToInt(playTime / 60);
        float seconds = Mathf.FloorToInt(playTime % 60);
        
        timeCounter.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        winCounter.text = data.gamesWon.ToString();
        joinCounter.text = data.gamesJoined.ToString();
        maxPiece.text = data.maxCourse.ToString();
        maxScroll.text = data.maxScroll.ToString();
        maxCourse.text = data.maxCourse.ToString();


    }
}
