using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : NetworkManager
{
    [Header("Lobby")] [Scene] public string menuScene = string.Empty;
    [Scene] public string gameScene = string.Empty;
    public int minPlayers;
    public PlayerLobby playerLobby;
    public List<PlayerLobby> RoomPlayers { get; } = new();
    private Transform playerList;

    [Header("Game")] public GameObject stateManagerObj;
    private StateManager stateManager;
    private MealManager mealManager;

    private new void Start()
    {
        autoCreatePlayer = false;
        GameObject stateManagerInstance = Instantiate(stateManagerObj, new Vector3(-2.8f, 3.6f, 0), Quaternion.identity);
        stateManagerInstance.name = "StateManager";
        stateManager = stateManagerInstance.GetComponent<StateManager>();
        mealManager = stateManagerInstance.GetComponent<MealManager>();
        stateManager.gameCanEnd = false;
        DontDestroyOnLoad(stateManager);
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        //Adds new player to players list when joined

        if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }

        if (SceneManager.GetActiveScene().name != menuScene)
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        //If in the menus, the lobby player is instantiated and manually added
        if (SceneManager.GetActiveScene().name == menuScene)
        {
            bool isLeader = RoomPlayers.Count == 0;

            PlayerLobby activePlayerLobby = Instantiate(playerLobby);

            activePlayerLobby.isLeader = isLeader;

            NetworkServer.AddPlayerForConnection(conn, activePlayerLobby.gameObject);

            stateManager.activePlayers.Add(conn.identity.netId);
            stateManager.playerNames.Add(conn.identity.name);
            Debug.LogWarning("ActivePlayers Count: " + stateManager.activePlayers.Count);
        }
    }

    public new void StartHost()
    {
        //Add all server spawnable objects
        spawnPrefabs = Resources.LoadAll<GameObject>("NetworkPrefabs").ToList();
    }

    public override void OnStartClient()
    {
        //Add all server spawnable objects
        GameObject[] spawnablePrefabs = Resources.LoadAll<GameObject>("NetworkPrefabs");

        foreach (GameObject prefab in spawnablePrefabs)
        {
            GetComponent<NetworkManager>().spawnPrefabs.Add(prefab);
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        // Remove connection from roomPlayers, activePlayers, and activePlayerNames
        if (conn.identity != null)
        {
            var player = conn.identity.GetComponent<PlayerLobby>();

            RoomPlayers.Remove(player);

            UpdateReadyState();
        }

        base.OnServerDisconnect(conn);
    }

    public void UpdateReadyState()
    {
        foreach (PlayerLobby player in RoomPlayers)
        {
            player.ReadyToStart(IsReadyToStart());
        }
    }

    private bool IsReadyToStart()
    {
        //If all players in lobby have isReady = true, then return true
        if (numPlayers < minPlayers)
        {
            return false;
        }

        foreach (PlayerLobby player in RoomPlayers)
        {
            if (!player.isReady)
            {
                return false;
            }
        }

        return true;
    }

    public override void OnStopServer()
    {
        //If all players in lobby have isReady = true, then return true
        RoomPlayers.Clear();
        stateManager.activePlayers.Clear();
        stateManager.playerNames.Clear();
    }

    private void ReplacePlayer(NetworkConnectionToClient conn)
    {
        //Void that changes connection's object from room player to game player
        GameObject roomPlayer = conn.identity.gameObject;

        NetworkServer.ReplacePlayerForConnection(conn, Instantiate(playerPrefab), true);

        Destroy(roomPlayer, 0.1f);
    }

    //// private IEnumerator FadeToNewScene()
    // {
    
    // }

    public void StartGame()
    {
        ServerChangeScene(gameScene); 
    }
    
    private IEnumerator PostStartCall()
    {
        //Delayed call to ensure players spawn first before turns or course begins

        foreach (PlayerLobby player in RoomPlayers)
        {
            NetworkConnectionToClient playerConn = player.GetComponent<NetworkConnectionToClient>();
            ReplacePlayer(playerConn);
        }

        yield return 0;
        
        if (stateManager.activePlayers.Count >= minPlayers)
        {
            stateManager.gameCanEnd = true;
        }
        
        stateManager.OnStartGame();

        yield return new WaitUntil(stateManager.CurrentPlayerAssigned);
        
        mealManager.OnStartGame();
    }

    public override void OnServerSceneChanged(string newSceneName)
    {
        StartCoroutine(PostStartCall());
    }
}