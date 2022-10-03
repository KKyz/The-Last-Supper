using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        // Code might not like being in here...

        if (SceneManager.GetActiveScene().name == "StartMenu")
        {
            room.localRoomPlayer = this;
            CmdSetDisplayName(PlayerPrefs.GetString("PlayerName"));
        }
    } 

    public override void OnStartClient()
    {
        // lobbyManager.UpdateDisplay();
    }

    public void Awake()
    {
        room = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (SceneManager.GetActiveScene().name == "StartMenu")
        {
            lobbyManager = GameObject.Find("Guests").GetComponent<LobbyManager>();
            room.roomPlayers.Add(this);
        }
    }

    public override void OnStopClient()
    {
        if (SceneManager.GetActiveScene().name == "StartMenu")
        {
            room.roomPlayers.Remove(this);
            lobbyManager.UpdateDisplay();
        }
    }

    public void ChangeDisplayName(string oldValue, string newValue)
    {
    // the problem is lobbyManager is ever only referenced for the client. I.e., host may not have a reference to this local player's lobby manager. lobby manager is null for it.

        if (lobbyManager != null)
        {
            lobbyManager.UpdateDisplay();
        }
    }
    
    public void ChangeReadyStatus(bool oldValue, bool newValue)
    {
        if (lobbyManager != null)
        {
            lobbyManager.UpdateDisplay();
        }
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
    public void CmdDestroy()
    {
        StartCoroutine(DestroySelf());
    }

    private IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(0.1f);
        
        NetworkServer.Destroy(gameObject);
    }

    [ClientRpc]
    public void RpcFade()
    {
        StartCoroutine(room.FadeToNewScene());
    }

    [Command(requiresAuthority = false)]
    public void CmdReadyUp()
    { 
        isReady = !isReady;
        
        room.UpdateReadyState();
    }

    [Command]
    private void CmdSetDisplayName(string newDisplayName)
    {
        displayName = newDisplayName;
        
        connectionToClient.identity.name = newDisplayName;
    }
    
    [TargetRpc]
    public void TargetFindLocalPlayer(NetworkConnection conn)
    {
        if (!isLeader)
        {
            lobbyManager.startGameButton.gameObject.SetActive(false);
        }
            //lobbyManager.tableSetUp.SetActive(false);
        
        if (room.localRoomPlayer == null)
        {
            room.localRoomPlayer = NetworkClient.localPlayer.GetComponent<PlayerLobby>();
        }
        
        lobbyManager.startGameButton.gameObject.SetActive(isLeader);
        //lobbyManager.tableSetUp.SetActive(isLeader);
    } 

}
