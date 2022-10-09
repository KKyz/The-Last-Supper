using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ShowStats : MonoBehaviour
{
    public TextMeshProUGUI scrollCounter, courseCounter, pieceCounter, timeCounter;

    public void LoadStats(PlayerManager player)
    {
        scrollCounter.text = player.scrollCount.ToString();
        courseCounter.text = player.courseCount.ToString();
        pieceCounter.text = player.pieceCount.ToString();

        player.accumulatedTime += 1;
        float minutes = Mathf.FloorToInt(player.accumulatedTime / 60);
        float seconds = Mathf.FloorToInt(player.accumulatedTime % 60);
        timeCounter.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (player.pieceCount > PlayerPrefs.GetInt("recordPieces", 0))
        {
            PlayerPrefs.SetInt("recordPieces", player.pieceCount);
        }
        
        if (player.scrollCount > PlayerPrefs.GetInt("recordScrolls", 0))
        {
            PlayerPrefs.SetInt("recordScrolls", player.scrollCount);
        }
        
        if (player.courseCount > PlayerPrefs.GetInt("recordCourse", 0))
        {
            PlayerPrefs.SetInt("recordCourse", player.courseCount);
        }
    }

    public void QuitToTitle()
    {
        ((GameManager)GameManager.singleton).ReturnToTitle();
        SceneManager.LoadScene("StartMenu");
    }
}

