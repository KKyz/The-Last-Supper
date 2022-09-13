using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLobby : NetworkBehaviour
{
    [SyncVar(hook = nameof(ChangeDisplayName))]
    public string displayName = "Loading...";

    [SyncVar(hook = nameof(ChangeReadyStatus))]
    public bool isReady;

    public bool isLeader;

    private GameManager room;
    public LobbyManager lobbyManager;

    public override void OnStartAuthority()
    {
        //Display your saved name on display and on gameObject
        lobbyManager = GameObject.Find("Guests").GetComponent<LobbyManager>();
        room = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        string playerName = PlayerPrefs.GetString("PlayerName");
        connectionToClient.identity.name = playerName;
        CmdSetDisplayName(playerName);
    }

    public override void OnStartClient()
    {
        room.roomPlayers.Add(this);
        lobbyManager.UpdateDisplay();
    }

    public override void OnStopClient()
    {
        room.roomPlayers.Remove(this);
        lobbyManager.UpdateDisplay();
    }

    public void ChangeDisplayName(string oldValue, string newValue)
    {
        lobbyManager.UpdateDisplay();
    }
    
    public void ChangeReadyStatus(bool oldValue, bool newValue)
    {
        lobbyManager.UpdateDisplay();
    }

    public void ReadyToStart(bool readyToStart)
    {
        //Sets game start button as interactable (not SetActive) if you are the leader
        if (isLeader)
        {
            lobbyManager.startGameButton.interactable = readyToStart; 
        }
    }

    [Command]
    private void CmdSetDisplayName(string newDisplayName)
    {
        displayName = newDisplayName;
    }
    
    [TargetRpc]
    public void TargetFindLocalPlayer(NetworkConnection conn)
    {
        lobbyManager.startGameButton.gameObject.SetActive(false);
        lobbyManager.tableSetUp.SetActive(false);
        
        if (room.localRoomPlayer == null)
        {
            room.localRoomPlayer = NetworkClient.localPlayer.GetComponent<PlayerLobby>();
        }
        
        lobbyManager.startGameButton.gameObject.SetActive(isLeader);
        lobbyManager.tableSetUp.SetActive(isLeader);
    }

}
