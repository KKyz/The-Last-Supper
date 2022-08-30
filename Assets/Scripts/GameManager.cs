using System.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : NetworkManager
{
    public StateManager stateManager;

    public MealManager mealManager;

    private Transform playerList;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        //Adds new player to players list when joined
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject newPlayer in allPlayers)
        {
            if (!stateManager.activePlayers.Contains(newPlayer.GetComponent<NetworkIdentity>().netId))
            {
                if (PlayerPrefs.GetString("PlayerName") != null)
                {
                    newPlayer.name = PlayerPrefs.GetString("PlayerName", "player" + Random.Range(0, 99));
                }
                else
                {
                    newPlayer.name = "Player " + Random.Range(0, 99);
                }
                
                stateManager.activePlayers.Add(newPlayer.GetComponent<NetworkIdentity>().netId);
                stateManager.playerNames.Add(newPlayer.name);

                Transform playerNameDisplay = playerList.GetChild(stateManager.playerNames.IndexOf(newPlayer.name));
                playerNameDisplay.gameObject.SetActive(true);
                playerNameDisplay.GetComponentInChildren<TextMeshProUGUI>().text = newPlayer.name;
            }
        }

        StartCoroutine(PostJoinCall());
        StartCoroutine(PostStartCall());

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
        
        foreach (Transform playerName in playerList)
        {
            playerName.gameObject.SetActive(false);
        }
    }

    public void JoinGame()
    {
        StartClient(); // manually start client
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (SceneManager.GetActiveScene().name == "StartMenu")
        {
            Transform playerNameDisplay = playerList.GetChild(stateManager.playerNames.IndexOf(NetworkClient.spawned[conn.identity.netId].gameObject.name));
            playerNameDisplay.gameObject.SetActive(false);
        }
        else
        {
            stateManager.CmdRemovePlayer(conn.identity.netId);
            NetworkServer.Destroy(NetworkClient.spawned[conn.identity.netId].gameObject);  
        }
    }

    public override void OnServerChangeScene(string newSceneName)
    {
        DontDestroyOnLoad(stateManager);
    }

    public override void OnServerSceneChanged(string newSceneName)
    {
        if (newSceneName != "StartMenu")
        {
            //Initial operations when server begins
            mealManager.StartOnLoad();
            mealManager.NextCourse();
            stateManager.gameCanEnd = false;
        }
    }

    private IEnumerator PostStartCall()
    {
        yield return 0;

        stateManager.turn = 0;
        stateManager.currentPlayer = NetworkClient.spawned[stateManager.activePlayers[0]].gameObject;
        stateManager.playerScript = stateManager.currentPlayer.GetComponent<PlayerManager>();
    }

    private IEnumerator PostJoinCall()
    {
        for (int i = 0; i <= 5; i++)
        {yield return 0;}

        mealManager.RpcUpdatePieceCounters();
        stateManager.RpcRefreshPlayerNames();
        
        //GameObject.FindWithTag("Plate").GetComponent<SpawnPiece>().RpcUpdatePieceParent();
    }
}