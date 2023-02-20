using System.Collections.Generic;
using System.Linq;
using TMPro;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class StealScroll : NetworkBehaviour
{
    public PlayerFunctions playerFunctions;
    public ScrollArray victim;
    private Button stealButton;
    private readonly List<ScrollArray> victims = new();
    public ToggleGroup playerToggles, scrollToggles;
    public string selectedScroll;
    private StateManager stateManager;


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
        scrollToggles = transform.Find("Scrolls").GetComponent<ToggleGroup>();

        for (int i = 0; i < 3; i++)
        {
            playerToggles.transform.GetChild(i).gameObject.SetActive(false); 
        }

        foreach (NetworkIdentity player in stateManager.activePlayers)
        {
            ScrollArray scrollArray = player.GetComponent<ScrollArray>();

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
            toggle.gameObject.SetActive(false);
        }
    }
    
    public void SelectVictim()
    {
        Debug.LogWarning("Selected new victim");
        if (playerToggles != null)
        {
            Toggle playerToggle = playerToggles.GetFirstActiveToggle();
            victim = victims[playerToggle.transform.GetSiblingIndex()];
            
            ShowAvailableScrolls();
        }
    }
    
    public void SelectScroll()
    {
        if (scrollToggles != null)
        {
            selectedScroll = scrollToggles.GetFirstActiveToggle().transform.GetComponentInChildren<TMP_Text>().text;
        }
    }

    private void ShowAvailableScrolls()
    {
        selectedScroll = null;
        
        foreach (Transform scrollLabel in scrollToggles.transform)
        {
            scrollLabel.gameObject.SetActive(false); 
        }

        if (victim != null)
        {
            for (int i = 0; i < victim.NumberOfScrolls(); i++)
            {
                scrollToggles.transform.GetChild(i).GetComponentInChildren<TMP_Text>().text = victim.GetName(i);
                scrollToggles.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
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

        victim.CmdRemoveScrollAmount(selectedScroll);
        victim.GetComponent<PlayerManager>().CmdSetScrollVictim(selectedScroll);
        playerFunctions.player.GetComponent<ScrollArray>().CmdAddScrollAmount(selectedScroll);
        
        CloseMenu();
    }
}
