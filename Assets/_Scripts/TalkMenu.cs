using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TalkMenu : NetworkBehaviour
{
    public int messageID;
    public NetworkIdentity targetPlayer;
    private List<NetworkIdentity> players = new(); 
    private List<NetworkIdentity> allPlayers = new(); 
    private StateManager stateManager;
    private PlayerFunctions playerFunctions;
    public ToggleGroup playerToggles, messageToggles;
    private Dictionary<Toggle, int> messageDict= new ();
    private Button talkButton;
    private int currentPlayerIndex;

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
            allPlayers.Add(player);
            
            if (player.gameObject != playerFunctions.player.gameObject)
            {
                players.Add(player);
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
        
            UpdateMessageToggles();
        }
    }

    public void SelectMessage()
    {
        if (messageToggles != null)
        {
            messageID = messageDict[messageToggles.GetFirstActiveToggle()];
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

        int index = 0;
        foreach (var player in allPlayers)
        {
            GameObject messageToggle = messageToggles.transform.GetChild(index).gameObject;
            TMP_Text toggleText = messageToggle.transform.GetChild(1).GetComponent<TMP_Text>();

            if (player.isLocalPlayer)
            {
                toggleText.text = stateManager.messages[0];
                messageToggle.SetActive(true);
                messageDict.Add(messageToggle.GetComponent<Toggle>(), 0);
                index++;
            }
            else if (player != targetPlayer)
            {
                toggleText.text = player.gameObject.name + " " + stateManager.messages[1];
                messageToggle.SetActive(true);
                messageDict.Add(messageToggle.GetComponent<Toggle>(), 1);
                index++;
            }
        }
        
        //Support Row

        foreach (var player in allPlayers)
        {
            if (player != targetPlayer)
            {
                GameObject messageToggle = messageToggles.transform.GetChild(index).gameObject;
                TMP_Text toggleText = messageToggle.transform.GetChild(1).GetComponent<TMP_Text>();

                if (player.isLocalPlayer)
                {
                    toggleText.text = stateManager.messages[2];
                    messageDict.Add(messageToggle.GetComponent<Toggle>(), 2);
                    messageToggle.SetActive(true);
                    index++;
                }
                else
                {
                    toggleText.text = player.gameObject.name + " " + stateManager.messages[3];
                    messageDict.Add(messageToggle.GetComponent<Toggle>(), 3);
                    messageToggle.SetActive(true);
                    index++;
                }
            }
        }
        
        //Team Row

        foreach (var player in allPlayers)
        {
            if (player != targetPlayer)
            {
                GameObject messageToggle = messageToggles.transform.GetChild(index).gameObject;
                TMP_Text toggleText = messageToggle.transform.GetChild(1).GetComponent<TMP_Text>();

                if (!player.isLocalPlayer)
                {
                    toggleText.text = stateManager.messages[5] + " " + player.gameObject.name;
                    messageToggle.SetActive(true);
                    messageDict.Add(messageToggle.GetComponent<Toggle>(), 5);
                    index++;
                }
                else if (player.isLocalPlayer)
                {
                    toggleText.text = stateManager.messages[4];
                    messageToggle.SetActive(true);
                    messageDict.Add(messageToggle.GetComponent<Toggle>(), 4);
                    index++;
                }
            }
        }

    }

    public class MsgContents
    {
        public int MessageID;
        public NetworkIdentity TargetPlayer;
        public string SenderName;
    }

    public void ConfirmTalk()
    {
        MsgContents msgContents = new MsgContents{MessageID = messageID, TargetPlayer = targetPlayer, SenderName = playerFunctions.player.name};
        //playerFunctions.player.hasTalked = true;
        playerFunctions.CmdConfirmTalk(msgContents);
        gameObject.GetComponent<SpawnMenu>().SlideOutMenu(); 
    }
}
