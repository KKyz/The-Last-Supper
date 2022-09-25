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

    public void Start()
    {
        gameObject.name = "StateManager";
        DontDestroyOnLoad(this);
        gameCanEnd = false;
    }
    

    [ServerCallback]
    public void OnStartGame()
    {
        gameCanEnd = false;
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
}
