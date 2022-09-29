using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TalkMenu : NetworkBehaviour
{
    public string message;
    private NetworkIdentity targetPlayer;
    private List<NetworkIdentity> players = new();
    private StateManager stateManager;
    private Transform talkButtons, toggleTransform;
    private ToggleGroup playerToggles;

    void Start()
    {
        message = "";
        int playerCount = 0;
        stateManager = GameObject.Find("StateManager").GetComponent<StateManager>();
        playerToggles = transform.Find("Players").GetComponent<ToggleGroup>();
        talkButtons = transform.Find("TalkButtons");
        players.Clear();

        foreach (NetworkIdentity playerIdentity in stateManager.activePlayers)
        {
            PlayerManager player = playerIdentity.GetComponent<PlayerManager>();
            if (player.gameObject != NetworkClient.localPlayer.gameObject)
            {
                players.Add(playerIdentity);
                playerToggles.transform.GetChild(playerCount).gameObject.SetActive(true);
                playerToggles.transform.GetChild(playerCount).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = player.name;
                playerCount += 1;
            }
        }
    }
    
    public void SelectPlayer()
    {
        Toggle playerToggle = playerToggles.ActiveToggles().FirstOrDefault();
        targetPlayer = players[playerToggle.transform.GetSiblingIndex()];
        UpdateTalkButtons();
    }

    public void SelectMessage(Transform button)
    {
        message = button.GetComponentInChildren<TextMeshProUGUI>().text;
    }

    public void UpdateTalkButtons()
    {
        //Attack Row
        Transform attackRow = talkButtons.Find("AttackRow");

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] != targetPlayer)
            {
                GameObject talkButton = attackRow.GetChild(i).gameObject;
                talkButton.SetActive(true);
                talkButton.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = players[i] + " is targeting you";
            }
        }
        
        //Support Row
        Transform supportRow = talkButtons.Find("SupportRow");

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] != targetPlayer)
            {
                GameObject talkButton = supportRow.GetChild(i).gameObject;
                talkButton.SetActive(true);
                talkButton.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = players[i] + " is helping you";
            }
        }
        
        //Team Row
        Transform teamRow = talkButtons.Find("TeamRow");

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] != targetPlayer)
            {
                GameObject talkButton = teamRow.GetChild(i).gameObject;
                talkButton.SetActive(true);
                talkButton.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "let's team up on " + players[i];
            }
        }

    }

    [Command(requiresAuthority = false)]
    public void CmdConfirmTalk(string messageToSend, string playerName)
    {
        NetworkConnection conn = targetPlayer.connectionToClient;  
        GetComponentInParent<PlayerFunctions>().TargetSendMessage(conn, messageToSend, playerName);
    }

    public void ConfirmTalk()
    {
        stateManager.currentPlayer.GetComponent<PlayerManager>().hasTalked = true;
        CmdConfirmTalk(message, NetworkClient.localPlayer.gameObject.name);
        gameObject.GetComponent<SpawnMenu>().SlideOutMenu();
    }
}