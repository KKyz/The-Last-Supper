using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    [HideInInspector]
    public PlayerManager playerScript;

    [HideInInspector]    
    public PlayerFunctions playerUI;

<<<<<<< Updated upstream
    [HideInInspector]
=======
    
>>>>>>> Stashed changes
    public List<GameObject> players = new List<GameObject>();

    [HideInInspector]
    public GameObject currentPlayer;

    public int nPieces;

    public List<GameObject> courses = new List<GameObject>();
<<<<<<< Updated upstream
    public GameObject playerPrefab;
    private GameObject seats, currentPlate;
    private List<Vector3> playerPos = new List<Vector3>();
=======
    private GameObject seats, currentPlate;
    private List<Vector3> playerPos = new List<Vector3>();
    
>>>>>>> Stashed changes
    private int turn, course;

    public void OnServerAddPlayer()
    {
<<<<<<< Updated upstream
        playerUI = GameObject.Find("PlayerCanvas").GetComponent<PlayerFunctions>();

        course = -1;
        NextCourse();
        seats = GameObject.Find("Players");

        foreach (Transform pos in seats.transform)
        {
            playerPos.Add(pos.transform.position);
            Destroy(pos.gameObject);
=======
        //Adds new player to players list when joined
        Debug.Log("PlayerConnected");
        GameObject newPlayer = GameObject.FindGameObjectWithTag("Player");
        if (!players.Contains(newPlayer))
        {
            players.Add(newPlayer);
>>>>>>> Stashed changes
        }
    }

<<<<<<< Updated upstream
        for (int i = 0; i < 4; i++)
        {
            currentPlayer = Instantiate(playerPrefab, playerPos[i], Quaternion.identity);
            currentPlayer.transform.SetParent(seats.transform);
            currentPlayer.name = Random.Range(0, 99).ToString();
            players.Add(currentPlayer);
        }
=======
    public override void OnStartServer()
    {
        //Initial operations when server begins
        Debug.Log("ServerLoaded");
        playerUI = GameObject.Find("PlayerCanvas").GetComponent<PlayerFunctions>();
        seats = GameObject.Find("Players");

        course = -1;
        NextCourse();

>>>>>>> Stashed changes
        
        turn = 0;
        currentPlayer = players[turn];
        playerUI.RpcSync(currentPlayer);
        StartCoroutine(PostStartCall());
    }

    IEnumerator PostStartCall()
    {
        yield return new WaitForEndOfFrame();
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

        //if (nPieces == 0)
        //{NextCourse();}
    }

    public void NextEncourage()
    {
        //Function called by PlayerFunctions to trigger encourege of next player
        if (turn < players.Count - 1)
        {players[turn + 1].GetComponent<PlayerManager>().isEncouraged = true;}
        else
        {players[0].GetComponent<PlayerManager>().isEncouraged = true;}
    }

    public void NextPlayer()
    {
        //If encouraged, then don't switch
<<<<<<< Updated upstream
        // Doesn't work(?s)
=======
        // Doesn't work(?)
>>>>>>> Stashed changes
        if (playerScript.isEncouraged)
        {playerScript.isEncouraged = false; Debug.Log("Encourage cleared");}

        else
        {
            if (turn < players.Count - 1)
            {turn += 1;}
            else
            {turn = 0;}

            //Reset all CurrentPlayer operations
            playerUI.RpcResetActions(true);
            playerUI.RpcActionToggle(false);

            //Sets next player as CurrentPlayer
            currentPlayer = players[turn];
            playerUI.RpcSync(currentPlayer);
            playerUI.RpcActionToggle(true);
            Debug.Log(currentPlayer.name);
        }
    }

    public void NextCourse()
    {
        course += 1;

        if (course < 3)
        {
            Destroy(currentPlate);
            currentPlate = Instantiate(courses[course], transform.position, Quaternion.identity);
<<<<<<< Updated upstream
=======
            NetworkServer.Spawn(currentPlate);
>>>>>>> Stashed changes
        }

        //If more than 1 player is alive at this stage, then desserts plate keeps refreshing
        else
        {
            course = 2;
            Destroy(currentPlate);
            currentPlate = Instantiate(courses[2], transform.position, Quaternion.identity);
<<<<<<< Updated upstream
=======
            NetworkServer.Spawn(currentPlate);
>>>>>>> Stashed changes
        }

        playerUI.plate = currentPlate.GetComponent<SpawnPiece>();
        currentPlate.transform.SetParent(transform);
    }
}