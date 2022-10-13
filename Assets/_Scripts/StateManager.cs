using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

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
        DontDestroyOnLoad(this);
        gameCanEnd = false;
    }

    public void Reset()
    {
        turn = 0;
        activePlayers.Clear();
        gameCanEnd = false;
    }

    public void Update()
    {
        //Code to add back players if they randomly get disconnected
        if (SceneManager.GetActiveScene().name != "StartMenu" && !isServer)
        {
            foreach (NetworkIdentity player in activePlayers)
            {
                if (player == null)
                {
                    CmdResyncActivePlayers(activePlayers.IndexOf(player));
                }
            }
        }
    }

    [Command]
    private void CmdResyncActivePlayers(int index)
    {
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject allPlayer in allPlayers)
        {
            if (!activePlayers.Contains(allPlayer.GetComponent<NetworkIdentity>()))
            {
                activePlayers[index] = allPlayer.GetComponent<NetworkIdentity>();
            }
        }
    }

    [ServerCallback]
    public void OnStartGame()
    {
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
                //Debug.LogWarning("Player Null");
                return false;
            }

            if (player.GetComponent<PlayerManager>().canContinue == false)
            {
                //Debug.LogWarning("player can't continue ");
                return false;
            }
        }
        
        //Debug.LogWarning("All Pass");
        return true;
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
