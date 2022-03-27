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
    }

    public void QuitToTitle()
    {
        SceneManager.LoadScene(0);
    }
}

