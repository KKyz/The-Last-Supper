using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.Localization.Settings;
using Unity.Mathematics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;
//using PlayFab;
using System;
//using kcp2k;

public class GameManager : NetworkManager
{
    [Header("Lobby")] 
    [Scene] public string gameScene;
    [HideInInspector] public int minPlayers = 2;
    public GameObject playerLobby;
    public GameObject discoveryButton; 
    public List<PlayerLobby> roomPlayers = new();
    [HideInInspector] public PlayerLobby localRoomPlayer;
    private Transform playerList;
    private int playerCount;
    private FadeInOut fade;
    private GameObject stateManagerInstance;
    private MenuManager menuManager; 

    [Header("Game")] 
    public GameObject stateManagerObj;
    [HideInInspector]public StateManager stateManager;
    private MealManager mealManager;
    public RestaurantContents[] restaurants;
    [HideInInspector]public GameObject currentRestaurant;
    [HideInInspector]public int currentMenu;
    public float scrollProb;
    public bool stealActive;
    public bool tagTournament;
    public List<NetworkIdentity> team1, team2 = new();
    public List<string> availableScrolls;
    [HideInInspector]public int gameMode;
    [HideInInspector]public TMP_FontAsset languageFont;

    [Header("Network Discovery")] 
    public CustomNetworkDiscovery networkDiscovery;
    public Dictionary<long, DiscoveryResponse> discoveredServers = new();
    public Dictionary<long, GameObject> spawnedButtons = new();
    [HideInInspector]public float cleanupTimer;
    [HideInInspector]public Transform discoveryList;
    
    [HideInInspector]public float playTime;
    public int maxCourse, maxPiece, maxScroll, gamesJoined, gamesWon, teamGamesWon;

    [Header("Misc")] 
    public TMP_FontAsset jpFont;
    public TMP_FontAsset engFont;
    
    public event Action<string> OnPlayerAdded;
    public event Action<string> OnPlayerRemoved;
    public static event Action<NetworkConnection> OnClientConnected;
    public static event Action<NetworkConnection> OnClientDisconnected;
    public List<UnityNetworkConnection> Connections { get; set; }

    [SerializeField] Configuration configuration = default;


#if UNITY_EDITOR
    new void OnValidate()
    {
        if (networkDiscovery == null)
        {
            networkDiscovery = GetComponent<CustomNetworkDiscovery>();
            UnityEditor.Events.UnityEventTools.AddPersistentListener(networkDiscovery.OnServerFound, OnDiscoverServer); 
            UnityEditor.Undo.RecordObjects(new Object[] {this, networkDiscovery}, "Set NetworkDiscovery");
        }
    }
#endif
  /*  
    public Configuration Config {
        get {
            return configuration;
        }
    }
    
    public KcpTransport Transport {
        get {
            return transport as KcpTransport;
        }
        set {
            transport = value;
        }
    }


    public override void Awake () {
        base.Awake ();

        if (Config.buildType == BuildType.REMOTE_SERVER) {
            Connections = new List<UnityNetworkConnection> ();
            NetworkServer.RegisterHandler<ReceiveAuthenticateMessage> (OnRecieveAuthenticate);
        }
    }

    private void OnRecieveAuthenticate (NetworkConnection _conn, ReceiveAuthenticateMessage msgType) {
        var conn = Connections.Find (c => c.ConnectionId == _conn.connectionId);
        if (conn != null) {
            conn.PlayFabId = msgType.PlayFabId;
            conn.IsAuthenticated = true;
            OnPlayerAdded?.Invoke (msgType.PlayFabId);
        }
    }
    */

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "StartMenu")
        {
            cleanupTimer += Time.deltaTime;

            if (cleanupTimer > 60)
            {
                CleanUpDiscoveryList();
                cleanupTimer = 0;
            }
        }
    }

    public void ChangeFont()
    {
        if (LocalizationSettings.SelectedLocale.LocaleName == "Japanese (ja)")
        {
            languageFont = jpFont;
        }
        else
        {
            languageFont = engFont;
        }
        
        var textComponents = FindObjectsOfType<TMP_Text>(true);
        foreach (var component in textComponents)
        {
            if (component.font == jpFont || component.font == engFont)
            {
                component.font = languageFont;
            }
        }
    }

    public void ReturnToTitle()
    {
        if (GameObject.Find("PlayerCanvas").GetComponent<PlayerFunctions>().player.isServer)
        {
            StopHost();
            Debug.LogWarning("Stopped Host");
        }
        else
        {
            StopClient();
        }
    }

    public void Init()
    {
        cleanupTimer = 0;
        autoCreatePlayer = false;
        currentRestaurant = null;
        tagTournament = false;
        gameMode = 0;
        team1.Clear();
        team2.Clear();
        fade = GameObject.Find("Fade").GetComponent<FadeInOut>();
        menuManager = GameObject.Find("StartCanvas").GetComponent<MenuManager>();
        ChangeFont();

        spawnPrefabs.Clear();
        scrollProb = 0;
        availableScrolls.Clear();
        stealActive = true;
        GameObject[] spawnablePrefabs = Resources.LoadAll<GameObject>("NetworkPrefabs");

        foreach (GameObject prefab in spawnablePrefabs)
        {
            GetComponent<NetworkManager>().spawnPrefabs.Add(prefab);
        }

        SaveSystem.SaveCheck(this);
        
        PlayerData data = SaveSystem.LoadPlayer();

        maxCourse = data.maxCourse;
        maxPiece = data.maxPiece;
        maxScroll = data.maxScroll;
        playTime = data.playTime;
        gamesWon = data.gamesWon;
        gamesJoined = data.gamesJoined;
        teamGamesWon = data.teamGamesWon;
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
        
        /*var uconn = Connections.Find (c => c.ConnectionId == conn.connectionId);
        if (uconn == null) {
            Connections.Add (new UnityNetworkConnection () {
                Connection = conn,
                ConnectionId = conn.connectionId,
                LobbyId = PlayFabMultiplayerAgentAPI.SessionConfig.SessionId
            });
        }*/
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

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect();
        
        if (SceneManager.GetActiveScene().name == "StartMenu")
        {
            GameObject.Find("StartCanvas").GetComponent<MenuManager>().ForceReturnToTitle();
        }

        else if (conn.identity.CompareTag("Player"))
        {
            if (conn.identity.isLocalPlayer)
            {
                SceneManager.LoadScene("StartMenu");
            }
            else
            {
                //conn.identity.GetComponent<PlayerFunctions>().Die();
            }
        }
        
        //OnClientDisconnected?.Invoke (NetworkClient.connection);
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        

        if (SceneManager.GetActiveScene().name == "StartMenu")
        {
            NetworkClient.AddPlayer();
            //OnClientConnected?.Invoke (NetworkClient.connection);
        }
    }

    public void OnDiscoverServer(DiscoveryResponse info)
    {
        //Note that you can check the versioning to decide if you can connect to the server or not using this method
        discoveredServers[info.serverId] = info;

        if (!spawnedButtons.ContainsKey(info.serverId))
        {
            GameObject newDiscoveryButton = Instantiate(discoveryButton, discoveryList, false);
            spawnedButtons[info.serverId] = newDiscoveryButton;
        }

        if (spawnedButtons.ContainsKey(info.serverId))
        {
            GameObject button = spawnedButtons[info.serverId];
            if (button != null)
            {
                button.gameObject.SetActive(true);
                button.transform.Find("TableName").GetComponent<TextMeshProUGUI>().text = info.hostName + "'s Table";
                button.transform.Find("PlayerCount").GetComponent<TextMeshProUGUI>().text = info.playerCount + "/" + maxConnections;
                button.GetComponent<Button>().onClick.AddListener(delegate { menuManager.Connect(info); });
                button.GetComponent<DiscoveryButton>().ResetTimer();

                if (info.playerCount >= 4 || info.gameVersion != Application.version)
                {
                    button.GetComponent<Button>().enabled = false;
                }
            }
        }
    }

    private void CleanUpDiscoveryList()
    {
        long[] keys = spawnedButtons.Keys.ToArray();
        for (int i = keys.Length - 1; i >= 0; i--)
        {
            long id = keys[i];
            if (!spawnedButtons[id].activeInHierarchy)
            {
                spawnedButtons.Remove(id);
                discoveredServers.Remove(id);
            }
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        // Remove connection from roomPlayers
        if (conn.identity != null && SceneManager.GetActiveScene().name == "StartMenu")
        {
            var player = conn.identity.GetComponent<PlayerLobby>();

            roomPlayers.Remove(player);

            UpdateReadyState();
        }
        else
        {
            ReturnToTitle();
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

            NetworkServer.ReplacePlayerForConnection(conn, Instantiate(playerPrefab), true);

            for (int j = 0; j < i; j++)
            {
                playerName += (" ");
            }
            
            conn.identity.name = playerName;
            stateManager.connectedPlayers.Add(conn.identity);
            
            if (tagTournament)
            {
                if (team1.Contains(player.netIdentity))
                {
                    foreach (var ally in team1)
                    {
                        if (ally != player.netIdentity)
                            player.GetComponent<PlayerManager>().allies.Add(ally);
                    }
                }

                else
                {
                    foreach (var ally in team2)
                    {
                        if (ally != player.netIdentity)
                            player.GetComponent<PlayerManager>().allies.Add(ally);
                    }
                }
            }
            
            i++;
        }
    }

    private void SetPlayerPositions()
    {
        int i = 0;
        foreach (NetworkIdentity player in stateManager.connectedPlayers)
        {
           player.transform.position =  startPositions[i].position;
           i++;
        }
    }

    private bool HasChangedRoomToGamePlayers()
    {
        foreach (var player in stateManager.connectedPlayers)
        {
            if (stateManager.connectedPlayers.Count != playerCount && player.GetComponent<PlayerLobby>() != null)
            {
                return false;
            }
        }

        return true;
    }

    private bool IsRestaurantInstantiated()
    {
        return currentRestaurant != null && startPositions.Count == 4;
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
        networkDiscovery.StopAdvertising();
        localRoomPlayer.RpcFade();
    }


    private IEnumerator PostStartCall()
    {
        //Delayed call to ensure start order of Restaurant, Player, PlayerUI, StateManager, and finally MealManager 

        mealManager.restaurant = currentRestaurant.GetComponent<RestaurantContents>();
        mealManager.menuIndex = currentMenu;
        RenderSettings.skybox = mealManager.restaurant.skyBox;
        
        GameObject gameRestaurant = Instantiate(currentRestaurant, currentRestaurant.transform.position, quaternion.identity);
        NetworkServer.Spawn(gameRestaurant);

        yield return new WaitUntil(IsRestaurantInstantiated);

        stateManager.stealActive = stealActive;
        stateManager.gameMode = gameMode;
        stateManager.tagTournament = tagTournament;
        stateManager.transform.position = GameObject.Find("StateManagerPos").transform.position;
        ReplacePlayers();

        yield return new WaitUntil(HasChangedRoomToGamePlayers);
        
        SetPlayerPositions();
        stateManager.SyncToActivePlayers();

        int i = 0;
        foreach (NetworkIdentity activePlayer in stateManager.activePlayers)
        {
            PlayerManager playerManager = activePlayer.GetComponent<PlayerManager>();
            playerManager.RpcRenamePlayer(activePlayer.name);
            playerManager.RpcAddPlayerModel(i);
            playerManager.RpcStartOnLocal();
            i++;
        }

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
    }
}