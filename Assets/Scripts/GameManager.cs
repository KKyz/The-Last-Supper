using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkManager
{
    [HideInInspector]
    public PlayerManager playerScript;
    
    public PlayerFunctions playerUI;
    
    public List<GameObject> players = new List<GameObject>();

    [HideInInspector]
    public GameObject currentPlayer;

    public int nPieces, activePlayers;
    public bool gameCanEnd;

    public List<GameObject> courses = new List<GameObject>();
    private GameObject seats, currentPlate;
    private List<Vector3> playerPos = new List<Vector3>();

    [SerializeField]
    private int turn, course;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        
        //Adds new player to players list when joined
        GameObject[] newPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject newPlayer in newPlayers)
        {
            if (!players.Contains(newPlayer))
            {
                players.Add(newPlayer);
                activePlayers += 1;
                Debug.Log("new player joined");

            }
        }

        if (players.Count >= 2)
        {gameCanEnd = true;}
    }

    public override void OnClientDisconnect()
    {
        foreach (GameObject player in players)
        {
            if (player == null)
            {
                NetworkServer.Destroy(player.gameObject);
                players.Remove(player);
            }
        }
    }

    public override void OnStartServer()
    {
        //Initial operations when server begins
        gameCanEnd = false;
        activePlayers = 0;
        seats = GameObject.Find("Players");

        course = -1;
        NextCourse();
        
        StartCoroutine(PostStartCall());
    }

    IEnumerator PostStartCall()
    {
        yield return new WaitForEndOfFrame();
        playerUI = GameObject.Find("PlayerCanvas").GetComponent<PlayerFunctions>();
        
        turn = 0;
        currentPlayer = players[turn];
        Debug.Log(currentPlayer.name);
        
        playerUI.RpcSync(currentPlayer);
        playerScript = currentPlayer.GetComponent<PlayerManager>();
        playerUI.RpcActionToggle(true);
        playerUI.CmdCancelAction();
    }
    
    void Update()
    {
        //For debugging
        if (Input.GetKeyDown("c"))
        {NextPlayer();}

        if (Input.GetKeyDown("v"))
        {NextCourse();}
    }

    public void NextEncourage()
    {
        //Function called by PlayerFunctions to trigger encourage of next player
        if (turn < players.Count - 1)
        {players[turn + 1].GetComponent<PlayerManager>().isEncouraged = true;}
        else
        {players[0].GetComponent<PlayerManager>().isEncouraged = true;}
    }

    public void NextPlayer()
    {
        //If encouraged, then don't switch
        // Doesn't work(?)
        if (playerScript.isEncouraged)
        {playerScript.isEncouraged = false; Debug.Log("Encourage cleared");}

        else
        {
            if (turn < players.Count - 1)
            {turn += 1;}
            else
            {turn = 0;}

            //Reset all CurrentPlayer operations
            playerUI.RpcResetActions();
            playerUI.RpcActionToggle(false);

            //Sets next player as CurrentPlayer
            currentPlayer = players[turn];
            playerUI.RpcSync(currentPlayer);
            playerUI.RpcActionToggle(true);
            Debug.Log(currentPlayer.name);
        }
    }

    IEnumerator CheckNPieces()
    {
        yield return 0;
        foreach (Transform piece in currentPlate.transform)
        {
            if (piece.transform.CompareTag("FoodPiece") && piece.GetComponent<FoodPiece>().type == "Normal")
            {nPieces += 1;}
        }
    }
    
    public void NextCourse()
    {
        course += 1;
        
        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerManager>().courseCount = course;
        }

        if (currentPlate != null)
        {Destroy(currentPlate);}
        
        if (course < 3)
        {currentPlate = Instantiate(courses[course], transform.position, Quaternion.identity);}
        else
        {currentPlate = Instantiate(courses[2], transform.position, Quaternion.identity);}
        
        NetworkServer.Spawn(currentPlate);
        StartCoroutine(CheckNPieces());

        playerUI.plate = currentPlate.GetComponent<SpawnPiece>();
        currentPlate.transform.SetParent(transform);
    }
}