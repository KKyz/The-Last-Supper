using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using Mirror;

public class GameManager : NetworkManager
{
    public PlayerFunctions playerUI;

    public StateManager stateManager;
    
    private bool isServer;

    public List<GameObject> courses = new List<GameObject>();
    private GameObject currentPlate;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        //Adds new player to players list when joined
        GameObject[] newPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject newPlayer in newPlayers)
        {
            if (!stateManager.players.Contains(newPlayer.GetComponent<NetworkIdentity>().netId))
            {
                newPlayer.name = "Player: " + UnityEngine.Random.Range(0, 999).ToString();
                stateManager.players.Add(newPlayer.GetComponent<NetworkIdentity>().netId);
            }
        }

        if (stateManager.players.Count >= 2)
        {
            stateManager.gameCanEnd = true;
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
    }

    public override void OnStartServer()
    {
        //Initial operations when server begins
        stateManager.gameCanEnd = false;
        isServer = true;

        stateManager.course = -1;
        NextCourse();

        StartCoroutine(PostStartCall());
    }

    private IEnumerator PostStartCall()
    {
        yield return 0;
        playerUI = GameObject.Find("PlayerCanvas").GetComponent<PlayerFunctions>();
        
        stateManager.turn = 0;
        stateManager.currentPlayer = NetworkClient.spawned[stateManager.players[0]].gameObject;
        stateManager.playerScript = stateManager.currentPlayer.GetComponent<PlayerManager>();
        Debug.Log(stateManager.currentPlayer.name);
    }
    
    void Update()
    {
        if (isServer)
        {
            if (Input.GetKeyDown("v"))
            {
                NextCourse();
            }
        }
    }
    
    public void NextEncourage()
    {
        //Function called by PlayerFunctions to trigger encourage of next player
        if (stateManager.turn < stateManager.players.Count - 1)
        {NetworkServer.spawned[stateManager.players[stateManager.turn + 1]].gameObject.GetComponent<PlayerManager>().isEncouraged = true;}
        else
        {NetworkServer.spawned[stateManager.players[0]].GetComponent<PlayerManager>().isEncouraged = true;}
    }

    IEnumerator CheckNPieces()
    {
        //Checks # of normal pieces (used to swap plates)
        yield return 0;
        foreach (Transform piece in currentPlate.transform)
        {
            if (piece.transform.CompareTag("FoodPiece") && piece.GetComponent<FoodPiece>().type == "Normal")
            {stateManager.nPieces += 1;}
        }
    }
    
    public void NextCourse()
    {
        stateManager.course += 1;

        foreach (uint playerID in stateManager.players)
        {
            GameObject playerObj = NetworkServer.spawned[playerID].gameObject;
            
            if ( playerObj != null)
            {
                playerObj.GetComponent<PlayerManager>().courseCount = stateManager.course + 1;
            }
        }

        if (currentPlate != null)
        {
            Destroy(currentPlate);
        }

        if (stateManager.course < 3)
        {
            currentPlate = Instantiate(courses[stateManager.course], transform.position, Quaternion.identity);
        }
        else
        {
            currentPlate = Instantiate(courses[2], transform.position, Quaternion.identity);
        }

        NetworkServer.Spawn(currentPlate);
        StartCoroutine(CheckNPieces());

        playerUI.plate = currentPlate.GetComponent<SpawnPiece>();
        currentPlate.transform.SetParent(transform);
    }
}