using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Discovery;
using TMPro;
using Unity.Mathematics;
using UnityEngine.SceneManagement;

public class GameManager : NetworkManager
{
    [Header("Lobby")]
    [Scene] public string gameScene;
    public int minPlayers;
    public GameObject playerLobby;
    public GameObject discoveryButton;
    public Transform discoveryList;
    public List<PlayerLobby> roomPlayers = new();
    [HideInInspector]  public PlayerLobby localRoomPlayer;
    private Transform playerList;
    private int playerCount;
    private FadeInOut fade;
    private GameObject stateManagerInstance;
    Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();

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

        spawnPrefabs.Clear();
        GameObject[] spawnablePrefabs = Resources.LoadAll<GameObject>("NetworkPrefabs");
        
        foreach (GameObject prefab in spawnablePrefabs)
        {
            GetComponent<NetworkManager>().spawnPrefabs.Add(prefab);
        }
    }

    public void OnServerInitialized()
    {
        NetworkServer.Spawn(stateManagerInstance);
    }
 
    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        //Adds new player to players list when joined

        if ((numPlayers >= maxConnections) || SceneManager.GetActiveScene().name != "StartMenu")
        { 
            conn.Disconnect();
        }
    }  
    
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    { 
        //If in the menus, the lobby player is instantiated and manually added
        
        if (SceneManager.GetActiveScene().name == "StartMenu")
        {
            bool isLeader = roomPlayers.Count == 0;

            GameObject newPlayerLobby = Instantiate(playerLobby);

            newPlayerLobby.GetComponent<PlayerLobby>().isLeader = isLeader;

            NetworkServer.AddPlayerForConnection(conn, newPlayerLobby);
            newPlayerLobby.GetComponent<PlayerLobby>().TargetFindLocalPlayer(conn);
            DontDestroyOnLoad(conn.identity.gameObject);
        }
    }

    public override void OnClientDisconnect()
    {
        if (SceneManager.GetActiveScene().name == "StartMenu")
        {
            GameObject.Find("StartCanvas").GetComponent<MenuManager>().ForceReturnToTitle();
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

    public void OnDiscoverServer(ServerResponse info)
    {
        // Note that you can check the versioning to decide if you can connect to the server or not using this method
        discoveredServers[info.serverId] = info;
        GameObject newDiscoveryButton = Instantiate(discoveryButton, discoveryList, false);
        newDiscoveryButton.transform.Find("TableName").GetComponent<TextMeshProUGUI>().text = "AddInfo" + "'s Table";
        newDiscoveryButton.transform.Find("playerCount").GetComponent<TextMeshProUGUI>().text = "playerCount" + "/" + maxConnections;
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        // Remove connection from roomPlayers
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
        // If all players in lobby have isReady = true, then return true
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
    }

    private void ReplacePlayers()
    {
        //Void that changes connection's object from room player to game player
        int i = 0;
        foreach (PlayerLobby player in roomPlayers)
        {
            NetworkConnectionToClient conn = player.netIdentity.connectionToClient; 
            conn.isReady = true;
            string playerName = conn.identity.name;
            int playerIndex = roomPlayers.IndexOf(player);
            
            NetworkServer.ReplacePlayerForConnection(conn, Instantiate(playerPrefab), true);
            conn.identity.GetComponent<Transform>().position = startPositions[playerIndex].position;
            PlayerManager playerManager = conn.identity.GetComponent<PlayerManager>();
            conn.identity.name = playerName + i;
            //playerManager.TargetAddPlayerModel(conn, i);
            stateManager.activePlayers.Add(conn.identity);
            i++;
        }
    }

    private bool HasChangedRoomToGamePlayers()
    {
        foreach (var player in stateManager.activePlayers)
        {
            if (stateManager.activePlayers.Count != playerCount && player.GetComponent<PlayerLobby>() != null)
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

    public IEnumerator FadeToNewScene()
    {
        fade.FadeIn(1.5f);
        StartCoroutine(GameObject.Find("StartCanvas").GetComponent<MenuManager>().BGMFadeOut(1.4f));
        
        yield return new WaitForSeconds(1.7f);
        
        if (localRoomPlayer.isLeader)
        {
            ServerChangeScene(gameScene); 
        }

    }
    
    public void StartGame()
    {
        playerCount = roomPlayers.Count;
        localRoomPlayer.RpcFade();
    }
    

    private IEnumerator PostStartCall()
    {
        //Delayed call to ensure start order of Restaurant, Player, PlayerUI, StateManager, and finally MealManager

        currentRestaurant = mealManager.restaurant.gameObject;
        GameObject gameRestaurant = Instantiate(currentRestaurant, currentRestaurant.transform.position, quaternion.identity);
        NetworkServer.Spawn(gameRestaurant);
        
        yield return new WaitUntil(IsRestaurantInstantiated);
        
        stateManager.transform.position = GameObject.Find("StateManagerPos").transform.position;
        ReplacePlayers();

        foreach (NetworkIdentity activePlayer in stateManager.activePlayers)
        {
            PlayerManager playerManager = activePlayer.GetComponent<PlayerManager>();
            playerManager.RpcRenamePlayer(activePlayer.name); 
        }

        yield return new WaitUntil(HasChangedRoomToGamePlayers);
        
        stateManager.OnStartGame();
        
        if (stateManager.activePlayers.Count >= minPlayers)
        {
            stateManager.gameCanEnd = true;
        }

        yield return new WaitUntil(stateManager.CurrentPlayerAssigned);
        
        mealManager.RpcStartGame();

        yield return new WaitUntil(mealManager.PopulatedCourseStack);
        
        mealManager.NextCourse();
    }
    
    public override void OnClientSceneChanged()
    {
        base.OnClientSceneChanged();

        if (SceneManager.GetActiveScene().name != "StartMenu")
        {
            localRoomPlayer.CmdDestroy();
        }

        else
        {
            Debug.LogWarning("Stopped Client");
            StopClient();
            
            if (stateManagerInstance != null)
            {
                Destroy(stateManagerInstance);
            }
        }
    }

    public override void OnServerChangeScene(string newSceneName)
    {
        if (newSceneName != "StartMenu")
        {
            stateManagerInstance = Instantiate(stateManagerObj, new Vector3(-2.8f, 3.6f, 0), Quaternion.identity);
            NetworkServer.Spawn(stateManagerInstance);

            stateManager = stateManagerInstance.GetComponent<StateManager>();
            mealManager = stateManagerInstance.GetComponent<MealManager>();
        }
    }

    public override void OnServerSceneChanged(string newSceneName)
    {
        if (newSceneName != "StartMenu")
        {
            StartCoroutine(PostStartCall());
        }

        else
        {
            Debug.LogWarning("Stopped Host");
            StopHost();
        }
    }
}