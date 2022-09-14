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
    
    [SyncVar]
    public int turn;
    
    public readonly SyncList<NetworkIdentity> activePlayers = new();
    public readonly Dictionary<uint, PlayerManager> spawnedPlayers = new();

    [ServerCallback]
    public void OnStartGame()
    {
        gameCanEnd = false;
        currentPlayer = null;
        turn = 0;
        currentPlayer = activePlayers[turn].gameObject;
        playerScript = currentPlayer.GetComponent<PlayerManager>();
    }

    public bool CurrentPlayerAssigned()
    {
        return currentPlayer != null;
    }

    [Command(requiresAuthority = false)]
    public void CmdRemovePlayer(NetworkIdentity player)
    {
        activePlayers.Remove(player);
    }
    
    [Command(requiresAuthority = false)]
    public void CmdNextPlayer()
    {
        if (playerScript.isEncouraged)
        {
            playerScript.isEncouraged = false;
        }

        else
        {
            if (turn < activePlayers.Count - 1)
            {
                turn += 1;
            }

            else
            {
                turn = 0;
            }
        }
        
        netIdentity.RemoveClientAuthority();
        currentPlayer = activePlayers[turn].gameObject;
        netIdentity.AssignClientAuthority(currentPlayer.GetComponent<NetworkConnectionToClient>());

        playerScript = currentPlayer.GetComponent<PlayerManager>();
    }
    
    [Command]
    public void CmdNextEncourage()
    {
        //Function called by PlayerFunctions to trigger encourage of next player
        //Add Authority
        if (turn < activePlayers.Count - 1)
        {activePlayers[turn + 1].gameObject.GetComponent<PlayerManager>().isEncouraged = true;}
        else
        {activePlayers[0].GetComponent<PlayerManager>().isEncouraged = true;}
    }

    [Command(requiresAuthority = false)]
    public void CmdNextEject()
    {
        //Add Authority
        if (turn < activePlayers.Count - 1)
        {activePlayers[turn + 1].gameObject.GetComponent<PlayerManager>().Eject();}
        else
        {activePlayers[0].gameObject.GetComponent<PlayerManager>().Eject();}
    }
    
    [Command]
    public void CmdSyncOrder(bool[] psnArray, PlayerManager victim)
    {
        victim.psn0 = psnArray[0];
        victim.psn1 = psnArray[1]; 
        victim.psn2 = psnArray[2];
        victim.psn3 = psnArray[3];
        victim.orderVictim = true;
    }
}
