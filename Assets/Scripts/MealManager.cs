using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class MealManager : NetworkBehaviour
{
    [SyncVar] 
    public int nPieces;
    public MealContainer mealContainer;
    
    private TextMeshProUGUI normCounter;
    private StateManager stateManager;
    private MusicManager musicManager;
    private Color normTop, normBottom;
    private readonly Stack<GameObject> currentCourses = new();
    private GameObject currentPlate;
    private bool firstPlate;

    public void StartOnLoad()
    {
        stateManager = GetComponent<StateManager>();
        musicManager = GetComponent<MusicManager>();
        normCounter = GameObject.Find("NormCounter").GetComponent<TextMeshProUGUI>();
        normTop = normCounter.colorGradient.topLeft;
        normBottom = normCounter.colorGradient.bottomRight;

        foreach (GameObject course in mealContainer.meals)
        {
            currentCourses.Push(course);
        }

        firstPlate = true;
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
        
        RpcUpdatePieceCounters();
    }

    [ClientRpc]
    public void RpcUpdatePieceCounters()
    {
        var lastTop = Color.red;
        var lastBottom = Color.red;
        if (nPieces <= 1)
        {
            normCounter.colorGradient = new VertexGradient(lastTop, lastTop, lastBottom, lastBottom);
        }
        else
        {
            normCounter.colorGradient = new VertexGradient(normTop, normTop, normBottom, normBottom);
        }
        
        normCounter.text = "# Of Empty Pieces Left: " + nPieces;
    }

    [ClientRpc]
    public void RpcShowChalk()
    {
        PlayerFunctions playerCanvas = GameObject.Find("PlayerCanvas").GetComponent<PlayerFunctions>();
        playerCanvas.ShowChalk();
    }

    [ClientRpc]
    public void RpcPlayCourseBGM(GameObject plate)
    {
        AudioClip courseBGM = plate.GetComponent<SpawnPiece>().courseBGM;
        musicManager.PlayBGM(courseBGM);
    }

    [ClientRpc]
    public void RpcLookAtPlate()
    {
        GameObject.FindWithTag("Player").GetComponent<CameraActions>().UpdateCameraLook();
    }

    [Command(requiresAuthority = false)]
    public void NextCourse()
    {
        //Add Authority

        foreach (uint playerID in stateManager.activePlayers)
        {
            GameObject playerObj = NetworkServer.spawned[playerID].gameObject;
            
            if (playerObj != null)
            {
                playerObj.GetComponent<PlayerManager>().courseCount += 1;
            }
        }

        if (currentPlate != null)
        {
            Destroy(currentPlate);
        }

        if (currentCourses.Count > 1 && !firstPlate)
        {
            currentCourses.Pop();
        }
        
        currentPlate = Instantiate(currentCourses.Peek(), transform.position, Quaternion.identity);
        firstPlate = false;
        NetworkServer.Spawn(currentPlate);
        currentPlate.transform.SetParent(transform, true);
        RpcShowChalk();
        RpcLookAtPlate();
        //RpcPlayCourseBGM(currentPlate);
        /* This function doesn't run on clients*/
        StartCoroutine(CheckNPieces());
    }
}
