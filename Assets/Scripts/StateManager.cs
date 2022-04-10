using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class StateManager : NetworkBehaviour
{

    [SyncVar] 
    public GameObject currentPlayer;
    
    [SyncVar]
    public PlayerManager playerScript;

    [SyncVar]
    public int activePlayers;

    [SyncVar] 
    public bool gameCanEnd;
    
    public readonly SyncList<uint> players = new SyncList<uint>();

    [SyncVar]
    public int turn;

    [Command(requiresAuthority = false)]
    public void removeActivePlayer()
    {
        activePlayers -= 1;
    }
    
    [Command(requiresAuthority = false)]
    public void SetCurrentPlayer()
    {
        if (playerScript.isEncouraged)
        {
            playerScript.isEncouraged = false;
        }

        else
        {
            if (turn < players.Count - 1)
            {
                turn += 1;
            }

            else
            {
                turn = 0;
            }
        }
        
        currentPlayer = NetworkClient.spawned[players[turn]].gameObject;
        playerScript = currentPlayer.GetComponent<PlayerManager>();
    }
    
    [Command(requiresAuthority = false)]
    public void NextEncourage()
    {
        //Function called by PlayerFunctions to trigger encourage of next player
        if (turn < players.Count - 1)
        {NetworkServer.spawned[players[turn + 1]].gameObject.GetComponent<PlayerManager>().isEncouraged = true;}
        else
        {NetworkServer.spawned[players[0]].GetComponent<PlayerManager>().isEncouraged = true;}
    }
}
