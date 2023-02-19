using System.Collections.Generic;
using TMPro;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class StealScroll : NetworkBehaviour
{
    public PlayerFunctions playerFunctions;
    public ScrollArray victim;
    private Button stealButton;
    public readonly List<ScrollArray> victims = new();
    public Transform playerToggles;
    public Toggle playerToggle, selectedScroll;
    public List<Toggle> scrollToggles = new();
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
        playerToggles = transform.Find("Players");
        playerToggle = null;

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
            scrollToggles.Add(toggle.GetComponent<Toggle>());
            toggle.gameObject.SetActive(false);
        }
        
        SelectVictim(0);
    }
    
    public void SelectVictim(int index)
    {
        if (playerToggles != null)
        {
            if (playerToggle == null)
            {
                playerToggles.GetChild(0).GetComponent<Toggle>().isOn = true;  
            }
            else if (index != playerToggle.transform.GetSiblingIndex())
            {
                playerToggle.isOn = false;
            }

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
        
        if (scrollToggles.Count > 0)
        {
            foreach (var scrollLabel in scrollToggles)
            {
                scrollLabel.gameObject.SetActive(false); 
            }
        }

        if (victim != null)
        {
            for (int i = 0; i < victim.NumberOfScrolls(); i++)
            {
                scrollToggles[i].GetComponentInChildren<TMP_Text>().text = victim.GetName(i);
                scrollToggles[i].gameObject.SetActive(true);
            }
        }
        
        SelectScroll(0);
    }

    public void SelectScroll(int index)
    {
        if (selectedScroll == null)
        {
            foreach (Toggle toggle in scrollToggles)
            {
                if (toggle.gameObject.activeInHierarchy)
                {
                    selectedScroll = toggle;
                }
            }
        }
        else if (index != selectedScroll.transform.GetSiblingIndex())
        {
            selectedScroll.isOn = false;
        }
        
        foreach (Toggle toggle in scrollToggles)
        {
            if (toggle.isOn)
            {
                selectedScroll = toggle;
            }
        }
        
        foreach (Toggle toggle in scrollToggles)
        {
            if (toggle.transform.GetSiblingIndex() != selectedScroll.transform.GetSiblingIndex())
            {
                toggle.isOn = false;
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
        string stolenScroll = selectedScroll.GetComponentInChildren<TMP_Text>().text;
        playerFunctions.RemoveStealScroll(stolenScroll);

        victim.CmdRemoveScrollAmount(stolenScroll);
        playerFunctions.player.GetComponent<ScrollArray>().CmdAddScrollAmount(stolenScroll);
        
        CloseMenu();
    }
}
