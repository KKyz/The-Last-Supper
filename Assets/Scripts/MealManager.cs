using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class MealManager : NetworkBehaviour
{
    [SyncVar] public int course, nPieces;

    private StateManager stateManager;

    public TextMeshProUGUI normCounter;

    public List<GameObject> courses = new List<GameObject>();
    
    private GameObject currentPlate;
    
    void Start()
    {
        stateManager = gameObject.GetComponent<StateManager>();
        normCounter = GameObject.Find("NormCounter").GetComponent<TextMeshProUGUI>();
    }

    public IEnumerator CheckNPieces()
    {
        //Checks # of normal pieces (used to swap plates)
        
        yield return 0; //DO NOT REMOVE THIS, IT NEEDS YIELD RETURN 0 IN ORDER TO CHECK W PLATE PROPER
        nPieces = 0;

        foreach (Transform piece in currentPlate.transform)
        {
            if (piece.CompareTag("FoodPiece") && piece.GetComponent<FoodPiece>().type == "Normal")
            {nPieces += 1;}
        }
        
        UpdatePieceCounters();
    }

    [ClientRpc]
    public void UpdatePieceCounters()
    {
        normCounter.text = nPieces.ToString();
    }
    
    [Command(requiresAuthority = false)]
    public void NextCourse()
    {
        //Add Authority
        course += 1;

        foreach (uint playerID in stateManager.players)
        {
            GameObject playerObj = NetworkServer.spawned[playerID].gameObject;
            
            if ( playerObj != null)
            {
                playerObj.GetComponent<PlayerManager>().courseCount = course + 1;
            }
        }

        if (currentPlate != null)
        {
            Destroy(currentPlate);
        }

        if (course < 3)
        {
            currentPlate = Instantiate(courses[course], transform.position, Quaternion.identity);
        }
        else
        {
            currentPlate = Instantiate(courses[2], transform.position, Quaternion.identity);
        }

        NetworkServer.Spawn(currentPlate);
        currentPlate.transform.SetParent(transform);
        /* This function doesn't run on clients*/
        StartCoroutine(CheckNPieces());
    }
}
