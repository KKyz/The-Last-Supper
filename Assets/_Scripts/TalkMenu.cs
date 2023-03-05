using System;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TalkMenu : NetworkBehaviour
{
    public int messageID;
    private NetworkIdentity targetPlayer;
    private List<NetworkIdentity> players = new(); 
    private StateManager stateManager;
    private PlayerFunctions playerFunctions;
    public ToggleGroup playerToggles, messageToggles;
    private Button talkButton;

    void Start()
    {
        messageID = -1;
        targetPlayer = null;
        int playerCount = 0; 
        stateManager = GameObject.Find("StateManager(Clone)").GetComponent<StateManager>();
        playerFunctions = GameObject.Find("PlayerCanvas").GetComponent<PlayerFunctions>();
        playerToggles = transform.Find("Players").GetComponent<ToggleGroup>();
        messageToggles = transform.Find("MessageToggles").GetComponent<ToggleGroup>();
        talkButton = transform.Find("TalkButton").GetComponent<Button>();
        players.Clear();

        for (int i = 0; i < 3; i++)
        {
            playerToggles.transform.GetChild(i).gameObject.SetActive(false); 
        }

        foreach (NetworkIdentity player in stateManager.activePlayers)
        {
            players.Add(player);
            
            if (player.gameObject != playerFunctions.player.gameObject)
            {
                playerToggles.transform.GetChild(playerCount).gameObject.SetActive(true);
                playerToggles.transform.GetChild(playerCount).GetComponentInChildren<TextMeshProUGUI>().text = player.name; 
                playerCount += 1;
            }
            
        }
        
        foreach (Transform toggle in messageToggles.transform)
        {
            toggle.gameObject.SetActive(false); 
        }
        
        SelectPlayer();
    }

    public void SelectPlayer()
    {
        if (playerToggles != null)
        {
            Toggle playerToggle = playerToggles.GetFirstActiveToggle();
            targetPlayer = players[playerToggle.transform.GetSiblingIndex()];
        }
        
        UpdateMessageToggles();
    }

    public void SelectMessage()
    {
        if (messageToggles != null)
        {
            messageID = messageToggles.GetFirstActiveToggle().transform.GetSiblingIndex();
        }
    }

    public void Update()
    {
        if (targetPlayer != null && messageID != -1)
        {
            talkButton.interactable = true;
        }

        else
        {
            talkButton.interactable = false;
        }
    }
    
    public void CloseMenu()
    {
        players.Clear();
        gameObject.GetComponent<SpawnMenu>().SlideOutMenu(); 
    }

    private void UpdateMessageToggles()
    {
        //Attack Row
        
        for (int i = 0; i < players.Count; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                GameObject messageToggle = messageToggles.transform.GetChild(j).gameObject;
                messageToggle.SetActive(true);
                string toggleText = messageToggle.transform.GetChild(1).GetComponent<TMP_Text>().text;

                if (players[i].isLocalPlayer)
                {
                    toggleText = stateManager.messages[0];
                }
                else
                {
                    toggleText = players[i] + " " + stateManager.messages[1];
                }
            }
        }
        
        //Support Row

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] != targetPlayer)
            {
                for (int j = 2; j < 4; j++)
                {
                    GameObject messageToggle = messageToggles.transform.GetChild(j).gameObject;
                    messageToggle.SetActive(true);
                    string toggleText = messageToggle.transform.GetChild(1).GetComponent<TMP_Text>().text;

                    if (players[i].isLocalPlayer)
                    {
                        toggleText = stateManager.messages[2];
                    }
                    else
                    {
                        toggleText = players[i] + " " + stateManager.messages[3];
                    }
                }
            }
        }
        
        //Team Row

        for (int i = 0; i < players.Count; i++)
        {
            for (int j = 2; j < 4; j++)
            {
                GameObject messageToggle = messageToggles.transform.GetChild(j).gameObject;
                messageToggle.SetActive(true);
                string toggleText = messageToggle.transform.GetChild(1).GetComponent<TMP_Text>().text;

                if (players[i] != targetPlayer && players[i].isLocalPlayer == false)
                {
                    toggleText = stateManager.messages[4];
                }
                else
                {
                    toggleText = stateManager.messages[5];
                }
            }
        }

    }

    [Command(requiresAuthority = false)]
    private void CmdConfirmTalk(int messageToSend)
    {
        NetworkConnection conn = targetPlayer.connectionToClient;  
        playerFunctions.TargetSendMessage(conn, messageToSend, playerFunctions.player.name);
    }

    public void ConfirmTalk()
    {
        playerFunctions.player.hasTalked = true;
        CmdConfirmTalk(messageID);
        gameObject.GetComponent<SpawnMenu>().SlideOutMenu();
    }
}
