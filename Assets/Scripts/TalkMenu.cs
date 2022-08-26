using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkMenu : MonoBehaviour
{
    private PlayerManager targetPlayer;
    private List<PlayerManager> players = new();
    private StateManager stateManager;
    private Transform playerButtons, talkButtons;
    
    void Start()
    {
        targetPlayer = null;
        int playerCount = 0;
        stateManager = GameObject.Find("StateManager").GetComponent<StateManager>();
        playerButtons = transform.Find("Players");
        talkButtons = transform.Find("TalkButtons");
        players.Clear();
        
        foreach (uint playerID in stateManager.activePlayers)
        {
            PlayerManager player = stateManager.spawnedPlayers[playerID];
            if (player.gameObject != stateManager.currentPlayer)
            {
                players.Add(player.GetComponent<PlayerManager>());
                playerButtons.GetChild(playerCount).gameObject.SetActive(true);
                playerButtons.GetChild(playerCount).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = player.name;
                playerCount += 1;
            }
        }
    }
    
    public void SelectPlayer(int playerNum)
    {
        targetPlayer = players[playerNum];
        UpdateTalkButtons();
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

    public void ConfirmTalk()
    {
        stateManager.currentPlayer.GetComponent<PlayerManager>().hasTalked = true; 
        gameObject.GetComponent<SpawnMenu>().SlideOutMenu();
    }
}
