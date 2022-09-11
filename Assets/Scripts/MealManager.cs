using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class MealManager : NetworkBehaviour
{
    [SyncVar] 
    public int nPieces;
    public MealContainer menu;
    public GameObject[] restaurants;
    
    private TextMeshProUGUI normCounter;
    private StateManager stateManager;
    private MusicManager musicManager;
    private Color normTop, normBottom;
    private readonly Stack<GameObject> currentCourses = new();
    private GameObject currentPlate;
    private bool firstPlate;

    public void OnStartGame()
    {
        stateManager = GetComponent<StateManager>();
        musicManager = GetComponent<MusicManager>();
        normCounter = GameObject.Find("NormCounter").GetComponent<TextMeshProUGUI>();
        normTop = normCounter.colorGradient.topLeft;
        normBottom = normCounter.colorGradient.bottomRight;

        foreach (GameObject course in menu.meals)
        {
            currentCourses.Push(course);
        }

        firstPlate = true;
        
        NextCourse();
    }

    private bool PlateChanged()
    {
        return currentPlate != null;
    }

    public IEnumerator CheckNPieces()
    {
        //Checks # of normal pieces (used to swap plates)

        yield return new WaitUntil(PlateChanged);
        nPieces = 0;

        foreach (Transform piece in currentPlate.transform)
        {
            if (piece.CompareTag("FoodPiece") && piece.GetComponent<FoodPiece>().type == "Normal")
            {nPieces += 1;}
        }
        
        RpcUpdatePieceCounters();
    }

    [ClientRpc]
    private void RpcUpdatePieceCounters()
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
    public void RpcUpdatePlayerEnd()
    {
        PlayerFunctions playerCanvas = GameObject.Find("PlayerCanvas").GetComponent<PlayerFunctions>();
        playerCanvas.ShowChalk();
        
        AudioClip courseBGM = currentPlate.GetComponent<SpawnPiece>().courseBGM;
        musicManager.PlayBGM(courseBGM);
        
        GameObject.FindWithTag("Player").GetComponent<CameraActions>().UpdateCameraLook();
    }

    [Command(requiresAuthority = false)]
    public void NextCourse()
    {
        Debug.LogWarning("Plate Added");
        //Add Authority

        foreach (NetworkIdentity player in stateManager.activePlayers)
        {
            PlayerManager playerScript = player.GetComponent<PlayerManager>();
            
            if (playerScript != null)
            {
                playerScript.courseCount += 1;
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
        //RpcUpdatePlayerEnd();
        /* This function doesn't run on clients*/
        StartCoroutine(CheckNPieces());
        currentPlate.GetComponent<SpawnPiece>().RpcUpdatePieceParent();
    }
}
