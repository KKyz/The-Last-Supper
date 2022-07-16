using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using Mirror;

public class GameManager : NetworkManager
{
    public StateManager stateManager;

    public MealManager mealManager;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        // PlayerManager player = conn.identity.GetComponent<PlayerManager>();
        // player.name = "Player: " + Random.Range(0, 999);
        // stateManager.players.Add(player.GetComponent<NetworkIdentity>().netId);
        // stateManager.activePlayers += 1;
        //         
        // spawnedPlayers.Add(player.GetComponent<NetworkIdentity>().netId, player.GetComponent<PlayerManager>());
        
        
        //Adds new player to players list when joined
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject newPlayer in allPlayers)
        {
            if (!stateManager.players.Contains(newPlayer.GetComponent<NetworkIdentity>().netId))
            {
                newPlayer.name = "Player: " + Random.Range(0, 999);
                stateManager.players.Add(newPlayer.GetComponent<NetworkIdentity>().netId);
                stateManager.activePlayers += 1;
            }
        }

        Debug.Log(StateManager.instance.spawnedPlayers);
        
        StartCoroutine(PostJoinCall());

        if (stateManager.players.Count == 1)
        {
            mealManager.course = -1;
            mealManager.NextCourse();  
        }

        if (stateManager.players.Count >= 2)
        {
            stateManager.gameCanEnd = true;
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        stateManager.RemoveActivePlayer();
    }

    public override void OnStartServer()
    {
        //Initial operations when server begins
        stateManager.gameCanEnd = false;

        StartCoroutine(PostStartCall());
    }

    private IEnumerator PostStartCall()
    {
        yield return 0;

        stateManager.turn = 0;
        stateManager.currentPlayer = NetworkClient.spawned[stateManager.players[0]].gameObject;
        stateManager.playerScript = stateManager.currentPlayer.GetComponent<PlayerManager>();
        Debug.Log(stateManager.currentPlayer.name);
    }

    private IEnumerator PostJoinCall()
    {
        for (int i = 0; i <= 5; i++)
        {yield return 0;}

        mealManager.UpdatePieceCounters();
    }
}