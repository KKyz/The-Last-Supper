using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor.Localization.Plugins.XLIFF.V20;

public class StateManager : NetworkBehaviour
{
    
    [SyncVar] 
    public GameObject currentPlayer;
    
    [SyncVar]
    public PlayerManager playerScript;

    [SyncVar] 
    public bool gameCanEnd, stealActive;
    
    [SyncVar]
    public int turn;
    
    [SyncVar]
    public Vector3 centerPos;
    
    [SyncVar]
    public Vector3 platePos;

    [SyncVar] 
    public string gameMode;

    [SyncVar] 
    public int maxPiecesEaten;
    
    [HideInInspector] public List<NetworkIdentity> connectedPlayers = new();
    
    public readonly SyncList<NetworkIdentity> activePlayers = new();

    public void Start()
    {
        DontDestroyOnLoad(this);
        gameCanEnd = false;
        maxPiecesEaten = 0;
        gameMode = "Free-For-All";
    }

    public void Reset()
    {
        turn = 0;
        activePlayers.Clear();
        gameCanEnd = false;
    }

    public void SyncToActivePlayers()
    {
        foreach (var player in connectedPlayers)
        {
            activePlayers.Add(player);
        }
    }

    [ServerCallback]
    public void OnStartGame()
    {
        centerPos = transform.position;
        currentPlayer = null;
        playerScript = null;
        turn = 0;
        currentPlayer = activePlayers[turn].gameObject;
        playerScript = currentPlayer.GetComponent<PlayerManager>();
    }

    public bool CurrentPlayerAssigned()
    {
        return currentPlayer != null;
    }

    public bool AllPlayersCanContinue()
    {
        foreach (var player in activePlayers)
        {
            if (player == null)
            {
                return false;
            }

            if (player.GetComponent<PlayerManager>().canContinue == false)
            {
                return false;
            }
        }
        
        return true;
    }

    [Command]
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
        
        currentPlayer = activePlayers[turn].gameObject;

        playerScript = currentPlayer.GetComponent<PlayerManager>();
    }
    
    [Command]
    public void CmdNextEncourage()
    {
        //Function called by PlayerFunctions to trigger encourage of next player
        if (turn < activePlayers.Count - 1)
        {activePlayers[turn + 1].gameObject.GetComponent<PlayerManager>().isEncouraged = true;}
        else
        {activePlayers[0].GetComponent<PlayerManager>().isEncouraged = true;}
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

    [Command]
    public void CmdSyncSteal(string scroll, NetworkIdentity victim, NetworkIdentity thief)
    {
        TargetChangeScroll(victim.connectionToClient, scroll, true);
        TargetChangeScroll(thief.connectionToClient, scroll, false);
    }

    [TargetRpc]
    private void TargetChangeScroll(NetworkConnection target, string scrollName, bool removing)
    {
        ScrollArray scrollArray = target.identity.GetComponent<ScrollArray>();
        
        if (removing)
        {
            scrollArray.RemoveScrollAmount(scrollName);
        }
        else
        {
            scrollArray.AddScrollAmount(scrollName);
        }
    }
}
