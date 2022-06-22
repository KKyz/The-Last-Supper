using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class MealManager : NetworkBehaviour
{
    [SyncVar] public int course, nPieces, pPieces;

    private StateManager stateManager;

    public TextMeshProUGUI psnCounter, normCounter;

    void Start()
    {
        stateManager = gameObject.GetComponent<StateManager>();
        psnCounter = GameObject.Find("PsnCounter").GetComponent<TextMeshProUGUI>();
        normCounter = GameObject.Find("NormCounter").GetComponent<TextMeshProUGUI>();
    }

    public List<GameObject> courses = new List<GameObject>();
    private GameObject currentPlate;
    
    public IEnumerator CheckNPieces()
    {
        //Checks # of normal pieces (used to swap plates)
        yield return 0;
        nPieces = 0;
        pPieces = 0;

        foreach (Transform piece in currentPlate.transform)
        {
            if (piece.transform.CompareTag("FoodPiece") && piece.GetComponent<FoodPiece>().type == "Normal")
            {nPieces += 1;}

            if (piece.transform.CompareTag("FoodPiece") && piece.GetComponent<FoodPiece>().type == "Poison")
            {pPieces += 1;}
        }

        psnCounter.text = pPieces.ToString();
        normCounter.text = nPieces.ToString();
    }
    
    [Command(requiresAuthority = false)]
    public void NextCourse()
    {
        Debug.Log("Next Course!");
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
        StartCoroutine(CheckNPieces());
        currentPlate.transform.SetParent(transform);
    }
}
