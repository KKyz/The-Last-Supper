using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerManager playerScript;
    public List<GameObject> playerPrefabs = new List<GameObject>();
    private List<GameObject> players = new List<GameObject>();
    public List<GameObject> courses = new List<GameObject>();
    private List<Vector3> playerPos = new List<Vector3>();
    private GameObject currentPlayer, seats, currentPlate;
    private int turn, course;

    void Start()
    {
        course = 0;
        currentPlate = Instantiate(courses[course], transform.position, Quaternion.identity);
        currentPlate.transform.SetParent(transform);
        seats = GameObject.Find("Seats");

        foreach (Transform child in seats.transform)
        {
            playerPos.Add(child.transform.position);
            Destroy(child.gameObject);
        }

        for (int i = 0; i < playerPrefabs.Count; i++)
        {
            currentPlayer = Instantiate(playerPrefabs[i], playerPos[i], Quaternion.identity);
            currentPlayer.transform.SetParent(seats.transform);
            players.Add(currentPlayer);
        }
        
        turn = 0;
        currentPlayer = players[turn];
        playerScript = currentPlayer.GetComponent<PlayerManager>();
        playerScript.ActionToggle(true);
    }

    
    void Update()
    {
        if (Input.GetKeyDown("c"))
        {NextPlayer();}

        if (Input.GetKeyDown("v"))
        {NextCourse();}
    }

    public void NextPlayer()
    {
        if (turn < players.Count - 1)
        {turn += 1;}
        else
        {turn = 0;}

        playerScript.ResetActions(true);
        currentPlayer = players[turn];

        playerScript = currentPlayer.GetComponent<PlayerManager>();
        playerScript.ActionToggle(true);
        Debug.Log(currentPlayer.name);
    }

    public void NextCourse()
    {
        course += 1;

        if (course < 3)
        {
            Destroy(currentPlate);
            currentPlate = Instantiate(courses[course], transform.position, Quaternion.identity);
        }

        else
        {
            course = 2;
            Destroy(currentPlate);
            currentPlate = Instantiate(courses[2], transform.position, Quaternion.identity);
        }

        foreach (GameObject player in players)
        {player.GetComponent<PlayerManager>().plate = currentPlate.GetComponent<SpawnPiece>();}

        currentPlate.transform.SetParent(transform);
    }
}
