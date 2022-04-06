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
    public bool gameCanEnd;
    
    public readonly SyncList<uint> players = new SyncList<uint>();

    [SyncVar]
    public int course, nPieces;

    [SyncVar]
    public int turn;
    
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
        
        Debug.Log("Turn: " + turn);
        currentPlayer = NetworkClient.spawned[players[turn]].gameObject;
        playerScript = currentPlayer.GetComponent<PlayerManager>();
    }
}
