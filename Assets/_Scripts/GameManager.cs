using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;
using Telepathy;
using TMPro;
using Unity.Mathematics;
using UnityEngine.SceneManagement;

public class GameManager : NetworkManager
{
    [Header("Lobby")]
    [Scene] public string gameScene;
    public int minPlayers;
    public PlayerLobby playerLobby;
    public List<PlayerLobby> roomPlayers = new();
    public PlayerLobby localRoomPlayer;
    private Transform playerList;
    private FadeInOut fade;

    [Header("Game")] 
    public GameObject stateManagerObj;
    private StateManager stateManager;
    private MealManager mealManager;
    private GameObject currentRestaurant;

    private new void Start()
    {
        autoCreatePlayer = false;
        currentRestaurant = null;
        fade = GameObject.Find("Fade").GetComponent<FadeInOut>();
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

        if ((numPlayers >= maxConnections) || SceneManager.GetActiveScene().name != "StartMenu")
        {
            conn.Disconnect();
        }
    }
    
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        //If in the menus, the lobby player is instantiated and manually added
        if (SceneManager.GetActiveScene().name == "StartMenu")
        {
            bool isLeader = roomPlayers.Count == 0;

            PlayerLobby newPlayerLobby = Instantiate(playerLobby);

            newPlayerLobby.isLeader = isLeader;

            NetworkServer.AddPlayerForConnection(conn, newPlayerLobby.gameObject);
            newPlayerLobby.TargetFindLocalPlayer(conn);
            DontDestroyOnLoad(conn.identity.gameObject);
        }
    }

    public override void OnStartHost()
    {
        //Add all server spawnable objects
        spawnPrefabs = Resources.LoadAll<GameObject>("NetworkPrefabs").ToList();
        
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        //Add all server spawnable objects
        GameObject[] spawnablePrefabs = Resources.LoadAll<GameObject>("NetworkPrefabs");
        
        foreach (GameObject prefab in spawnablePrefabs)
        {
            GetComponent<NetworkManager>().spawnPrefabs.Add(prefab);
        }
        
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        
        if (SceneManager.GetActiveScene().name == "StartMenu")
        {
            NetworkClient.AddPlayer();
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        // Remove connection from roomPlayers, activePlayers
        if (conn.identity != null)
        {
            var player = conn.identity.GetComponent<PlayerLobby>();

            roomPlayers.Remove(player);

            UpdateReadyState();
        }

        base.OnServerDisconnect(conn);
    }

    public void UpdateReadyState()
    {
        foreach (PlayerLobby player in roomPlayers)
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

        foreach (PlayerLobby player in roomPlayers)
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
        roomPlayers.Clear();
        stateManager.activePlayers.Clear();
    }

    private void ReplacePlayers()
    {
        Debug.LogWarning("Replace Players: " + roomPlayers.Count);
        //Void that changes connection's object from room player to game player
        foreach (PlayerLobby player in roomPlayers)
        {
            NetworkConnectionToClient conn = player.netIdentity.connectionToClient;
            string playerName = conn.identity.name;
            int playerIndex = roomPlayers.IndexOf(player);
            GameObject roomPlayer = conn.identity.gameObject;
            
            NetworkServer.ReplacePlayerForConnection(conn, Instantiate(playerPrefab), true);
            conn.identity.GetComponent<Transform>().position = startPositions[playerIndex].position;
            conn.identity.gameObject.name = playerName;
            conn.identity.GetComponent<PlayerManager>().OnStartGame();
            stateManager.activePlayers.Add(conn.identity);
            
            Destroy(roomPlayer, 0.1f);
        }
    }

    private bool HasChangedRoomToGamePlayers()
    {
        foreach (PlayerLobby player in roomPlayers)
        {
            if (player.gameObject != playerPrefab)
            {
                return false;
            }
        }
        
        return true;
    }

    private bool IsRestaurantInstantiated()
    {
        return currentRestaurant != null;
    }
    
    private IEnumerator FadeToNewScene()
    {
        fade.FadeIn(1.5f);
        yield return new WaitForSeconds(1.7f);
        
        if (localRoomPlayer.isLeader)
        {
            ServerChangeScene(gameScene); 
        }

    }
    
    public void StartGame()
    {
        StartCoroutine(FadeToNewScene());
    }
    
    private IEnumerator PostStartCall()
    {
        //Delayed call to ensure start order of Restaurant, Player, PlayerUI, StateManager, and finally MealManager
        currentRestaurant = mealManager.restaurant.gameObject;
        Instantiate(currentRestaurant, currentRestaurant.transform.position, quaternion.identity);
        
        yield return new WaitUntil(IsRestaurantInstantiated);

        stateManager.transform.position = GameObject.Find("StateManagerPos").transform.position;
        ReplacePlayers();

        yield return new WaitUntil(HasChangedRoomToGamePlayers);
        
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