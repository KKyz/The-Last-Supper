using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLobby : NetworkBehaviour
{
    public GameObject lobbyUi;
    public TMP_Text[] playerNames = new TMP_Text[4];
    public Toggle[] playerReadyToggles = new Toggle[4];
    public Button startGameButton;

    [SyncVar(hook = nameof(ChangeDisplayName))]
    public string displayName = "Loading...";

    [SyncVar(hook = nameof(ChangeReadyStatus))]
    public bool isReady = false;

    public bool isLeader;

    private GameManager room;

    private GameManager Room
    {
        //Find the room object in the scene (like GameObject.Find)
        get
        {
            if (room != null)
            {
                return room;
            }

            return room = NetworkManager.singleton as GameManager;
        }
    }
     
    public override void OnStartAuthority()
    {
        //Display your saved name on display and on gameObject
        string playerName = PlayerPrefs.GetString("PlayerName");
        CmdSetDisplayName(playerName);
        connectionToClient.identity.name = playerName;

        lobbyUi.SetActive(true);
    }

    public override void OnStartClient()
    {
        Room.RoomPlayers.Add(this);

        UpdateDisplay();
    }

    public override void OnStopClient()
    {
        Room.RoomPlayers.Remove(this);

        UpdateDisplay();
    }

    public void ChangeDisplayName(string oldValue, string newValue)
    {
        UpdateDisplay();
    }
    
    public void ChangeReadyStatus(bool oldValue, bool newValue)
    {
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        //Hacky way to find player that you are of authority
        if (!hasAuthority)
        {
            foreach (var player in Room.RoomPlayers)
            {
                if (player.hasAuthority)
                {
                    player.UpdateDisplay();
                    break;
                }
            }
            return;
        }
        
        for (int i = 0; i < playerNames.Length; i++)
        {
            playerNames[i].text = "Waiting For Player...";
            playerReadyToggles[i].isOn = false;
        }

        for (int i = 0; i < Room.RoomPlayers.Count; i++)
        {
            playerNames[i].text = Room.RoomPlayers[i].displayName;
            playerReadyToggles[i].isOn = Room.RoomPlayers[i].isReady;
        }
    }

    public void ReadyToStart(bool readyToStart)
    {
        //Sets game start button as active if you are the leader
        if (isLeader)
        {
            startGameButton.interactable = readyToStart; 
        }
    }

    [Command]
    private void CmdSetDisplayName(string newDisplayName)
    {
        displayName = newDisplayName;
    }

    [Command]
    public void CmdReadyUp()
    { 
        isReady = !isReady;
        
        Room.UpdateReadyState();
    }

    [Command]
    public void CmdStartGame()
    {
        if (Room.RoomPlayers[0].connectionToClient == connectionToClient)
        {
            Room.StartGame();
        }
    }

}
