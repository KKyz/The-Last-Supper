using System.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.Networking.Types;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : NetworkManager
{
    public GameObject stateManagerObj;
    
    public StateManager stateManager;
    public MealManager mealManager;
    private Transform playerList;
    private bool serverReadyToStart = false;
    private bool allPlayersConnected;

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
    
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        //Adds new player to players list when joined

        conn.identity.gameObject.name = PlayerPrefs.GetString("PlayerName", "player" + Random.Range(0, 99));

        stateManager.activePlayers.Add(conn.identity.netId);
        stateManager.playerNames.Add(conn.identity.gameObject.name);

        if (SceneManager.GetActiveScene().name == "StartMenu")
        {
            RefreshPlayerNames();
        }

        if (stateManager.activePlayers.Count >= 2)
        {
            stateManager.gameCanEnd = true;
        }
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        Debug.Log("I connected to a server!");
        
        autoCreatePlayer = true; // set the autoCreateFlag for the Network Manager (clients and host)
    }
    
    public new void StartHost()
    {
        base.StartHost();
        playerList = GameObject.Find("PlayerList").transform;
        
        RefreshPlayerNames();
        
    }

    public void JoinGame()
    {
        StartClient(); // manually start client
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (SceneManager.GetActiveScene().name != "StartMenu")
        {
            NetworkServer.Destroy(NetworkClient.spawned[conn.identity.netId].gameObject); 
            stateManager.activePlayers.Remove(conn.identity.netId);
            stateManager.playerNames.Remove(conn.identity.gameObject.name);
        }
        else
        {
            stateManager.activePlayers.Remove(conn.identity.netId);
            stateManager.playerNames.Remove(conn.identity.gameObject.name);
            RefreshPlayerNames();
        }
        
        
    }

    private void RefreshPlayerNames()
    {
        foreach (Transform playerName in playerList)
        {
            playerName.gameObject.SetActive(false);
        }
        
        foreach (var playerName in stateManager.playerNames)
        {
            Transform playerNameDisplay = playerList.GetChild(stateManager.playerNames.IndexOf(playerName));
            playerNameDisplay.gameObject.SetActive(true);
            playerNameDisplay.GetComponentInChildren<TextMeshProUGUI>().text = playerName;
        }
    }

    private void Update()
    {
        if (serverReadyToStart)
        {
            if (allPlayersConnected)
            {
                mealManager.StartOnLoad(); 
                stateManager.DefaultState();
                serverReadyToStart = false;
                allPlayersConnected = false;
            }
            else
            {
                foreach (uint id in stateManager.activePlayers)
                {
                    allPlayersConnected = stateManager.spawnedPlayers[id].GetComponent<NetworkIdentity>().connectionToClient.isReady;
                    if (!allPlayersConnected)
                    {
                        break;
                    }
                }
            }
        }
    }

    public override void OnServerSceneChanged(string newSceneName)
    {
        if (newSceneName != "StartMenu")
        {
            //Initial operations when server 
            serverReadyToStart = true;
        }
    }
}