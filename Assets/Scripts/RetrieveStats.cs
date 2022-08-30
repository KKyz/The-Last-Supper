using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RetrieveStats : MonoBehaviour
{
    private TextMeshProUGUI joinCounter, winCounter, maxPiece, maxCourse, timeCounter, maxScrollCounter; 
    //Get stats to show in menu
    void Start()
    {
        joinCounter = transform.Find("JoinCounter").GetComponent<TextMeshProUGUI>();
        winCounter = transform.Find("WinCounter").GetComponent<TextMeshProUGUI>();
        maxPiece = transform.Find("PieceCounter").GetComponent<TextMeshProUGUI>();
        maxCourse = transform.Find("CourseCounter").GetComponent<TextMeshProUGUI>();
        timeCounter = transform.Find("TimeCounter").GetComponent<TextMeshProUGUI>();
        maxScrollCounter = transform.Find("ScrollCounter").GetComponent<TextMeshProUGUI>();
        
        
        
        float playTime = PlayerPrefs.GetInt("playTime", 0);
        float minutes = Mathf.FloorToInt(playTime / 60);
        float seconds = Mathf.FloorToInt(playTime % 60);
        
        timeCounter.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        winCounter.text = PlayerPrefs.GetInt("gamesWon", 0).ToString();
        joinCounter.text = PlayerPrefs.GetInt("gamesJoined", 0).ToString();
        maxPiece.text = PlayerPrefs.GetInt("recordPieces", 0).ToString();
        maxScrollCounter.text = PlayerPrefs.GetInt("recordScrolls", 0).ToString();
        maxCourse.text = PlayerPrefs.GetInt("recordCourse", 0).ToString();


    }
}
