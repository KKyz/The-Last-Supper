using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScrollToggles : MonoBehaviour
{
    [SerializeField]private List<string> scrollToggles;
    private GameManager gameManager;
    [SerializeField]private List<string> gameModes;
    private TMP_Dropdown gameModeDrop;

    public void Start()
    {
        gameModeDrop = transform.Find("Window").Find("GameOptions").Find("GameModeDrop").GetComponent<TMP_Dropdown>(); 
        gameModeDrop.ClearOptions();
        gameModeDrop.AddOptions(gameModes);
    }

    public void SelectGameMode(int index)
    {
        gameManager.gameMode = index;
    }

    // Do NOT change names of toggles in inspector
    public void InitScrollProb()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        foreach (Transform toggle in transform.Find("Window").Find("Toggles"))
        {
            if (toggle.name != "Steal")
            {
                scrollToggles.Add(toggle.name);
                gameManager.availableScrolls.Add(toggle.name);
            }
        }

        gameManager.scrollProb = (scrollToggles.Count + 0.1f - gameManager.availableScrolls.Count)/scrollToggles.Count; 
    }
    
    public void ToggleScroll(Toggle toggle)
    {
        int toggleIndex = toggle.transform.GetSiblingIndex();
        
        if (toggle.isOn)
        {
            if (toggle.name != "Steal")
            {
                if (gameManager != null && !gameManager.availableScrolls.Contains(scrollToggles[toggleIndex]))
                {
                    gameManager.availableScrolls.Add(scrollToggles[toggleIndex]);
                }
            }
            
            else
            {
                gameManager.stealActive = true;
            }
        }
        else
        {
            if (gameManager.availableScrolls.Count > 1)
            {
                if (toggle.name != "Steal")
                {
                    gameManager.availableScrolls.Remove(scrollToggles[toggleIndex]);
                }
                
                else
                {
                    gameManager.stealActive = false;
                }
            }
            
            else
            {
                toggle.isOn = true; 
            }
        }

        if (gameManager.availableScrolls.Count > 1)
        {
            gameManager.scrollProb = (scrollToggles.Count + 0.1f - gameManager.availableScrolls.Count)/scrollToggles.Count; 
        }

        else
        {
            gameManager.scrollProb = 1f;
        }
    }
}
