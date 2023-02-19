using System.Collections.Generic;
using Mirror;
using System.Linq;
using Mono.Cecil.Cil;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StealScroll : MonoBehaviour
{
    private PlayerFunctions playerFunctions;
    private ScrollArray victim;
    private Button stealButton;
    private readonly List<ScrollArray> victims = new();
    private ToggleGroup playerToggles;
    public List<TMP_Text> scrollLabels;
    private StateManager stateManager;
    private string selectedScroll;
    
    
    void Start()
    {
        victim = null;
        selectedScroll = null;
        int playerCount = 0;
        victims.Clear();
        stealButton = transform.Find("StealButton").GetComponent<Button>();
        stateManager = GameObject.Find("StateManager(Clone)").GetComponent<StateManager>();
        playerFunctions = GameObject.Find("PlayerCanvas").GetComponent<PlayerFunctions>();
        playerToggles = transform.Find("Players").GetComponent<ToggleGroup>();
        
        //Add other player's names

        foreach (NetworkIdentity player in stateManager.activePlayers)
        {
            ScrollArray scrollArray = player.GetComponent<ScrollArray>(); //How to get scroll array from another client?
            playerToggles.transform.GetChild(playerCount).gameObject.SetActive(false);
            
            if (player.gameObject != stateManager.currentPlayer)
            {
                victims.Add(scrollArray);
                playerToggles.transform.GetChild(playerCount).gameObject.SetActive(true);
                playerToggles.transform.GetChild(playerCount).GetComponentInChildren<TextMeshProUGUI>().text = player.name; 
                playerCount += 1;
            }
        }

        foreach (Transform toggle in transform.Find("Scrolls"))
        {
            foreach (Transform child in toggle)
            {
                
                if (child.GetComponent<TMP_Text>() != null)
                {
                    scrollLabels.Add(child.GetComponent<TMP_Text>());
                }
            }
            
            toggle.gameObject.SetActive(false);
        }
    }
    
    public void SelectVictim()
    {
        if (playerToggles != null)
        {
            Toggle playerToggle = playerToggles.ActiveToggles().FirstOrDefault();

            foreach (Transform toggle in playerToggles.transform)
            {
                if (toggle.GetComponent<Toggle>().isOn)
                {
                    playerToggle = toggle.GetComponent<Toggle>();
                }
            }
            victim = victims[playerToggle.transform.GetSiblingIndex()];
        }
        
        ShowAvailableScrolls();
    }

    private void ShowAvailableScrolls()
    {
        selectedScroll = null;

        foreach (var scrollLabel in scrollLabels)
        {
            scrollLabel.transform.parent.gameObject.SetActive(false);
        }

        if (victim != null)
        {
            Debug.LogWarning(victim.NumberOfScrolls());
            for (int i = 0; i < victim.NumberOfScrolls(); i++)
            {
                scrollLabels[i].text = victim.GetName(i);
                scrollLabels[i].transform.parent.gameObject.SetActive(true);
            }
        }
    }

    public void SelectScroll(int index)
    {
        Debug.LogWarning("Toggle index: " + index);
        //selectedScroll = scrollLabels[index].text;
    }

    
    public void Update()
    {
        if (victim != null && selectedScroll != null)
        {
            stealButton.interactable = true;
        }
        
        else
        {
            stealButton.interactable = false;
        }
    }
    
    public void CloseMenu()
    {
        victims.Clear();
        gameObject.GetComponent<SpawnMenu>().SlideOutMenu(); 
    }
    
    public void Steal()
    {
        playerFunctions.RemoveStealScroll(selectedScroll);

        var victimConnection = victim.GetComponent<PlayerManager>().netIdentity;
        var playerConnection = playerFunctions.player.netIdentity;
        
        //How to get scroll array from another client?

        stateManager.CmdSyncSteal(selectedScroll, victimConnection, playerConnection);
        
        CloseMenu();
    }
}
