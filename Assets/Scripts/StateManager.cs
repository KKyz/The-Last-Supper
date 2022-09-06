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
    
    public readonly SyncList<uint> activePlayers = new();
    public readonly SyncList<string> playerNames = new ();
    public readonly Dictionary<uint, PlayerManager> spawnedPlayers = new();

    [SyncVar]
    public int turn;
    
    public void OnStartGame()
    {
        if (isClient)
        {
            Debug.LogWarning("StateManager in DefaultState");
            turn = 0;
            currentPlayer = NetworkClient.spawned[activePlayers[turn]].gameObject;
            playerScript = currentPlayer.GetComponent<PlayerManager>();
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdRemovePlayer(uint playerID)
    {
        activePlayers.Remove(playerID);
        playerNames.Remove(NetworkClient.spawned[playerID].gameObject.name);
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
        currentPlayer = NetworkClient.spawned[activePlayers[turn]].gameObject;
        netIdentity.AssignClientAuthority(currentPlayer.GetComponent<NetworkConnection>());

        playerScript = currentPlayer.GetComponent<PlayerManager>();
    }
    
    [Command]
    public void CmdNextEncourage()
    {
        //Function called by PlayerFunctions to trigger encourage of next player
        //Add Authority
        if (turn < activePlayers.Count - 1)
        {NetworkServer.spawned[activePlayers[turn + 1]].gameObject.GetComponent<PlayerManager>().isEncouraged = true;}
        else
        {NetworkServer.spawned[activePlayers[0]].GetComponent<PlayerManager>().isEncouraged = true;}
    }

    [Command(requiresAuthority = false)]
    public void CmdNextEject()
    {
        //Add Authority
        if (turn < activePlayers.Count - 1)
        {NetworkServer.spawned[activePlayers[turn + 1]].gameObject.GetComponent<PlayerManager>().Eject();}
        else
        {NetworkServer.spawned[activePlayers[0]].gameObject.GetComponent<PlayerManager>().Eject();}
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
