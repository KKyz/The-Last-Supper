using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class MealManager : NetworkBehaviour
{
    [SyncVar]
    private bool firstPlate;
    
    public int nPieces;
    public RestaurantContents restaurant;
    public GameObject platterTop;
    public int menuIndex = 0;

    private TextMeshProUGUI normCounter;
    private StateManager stateManager;
    private MusicManager musicManager;
    private Color normTop, normBottom;
    private readonly Stack<GameObject> courseStack = new();
    public readonly Stack<AudioClip> bgmStack = new();
    private GameObject currentPlate;
    public PlayerManager localPlayer;

    [ClientRpc]
    public void RpcStartGame()
    {
        stateManager = GetComponent<StateManager>();
        musicManager = GetComponent<MusicManager>();
        normCounter = GameObject.Find("NormCounter").GetComponentInChildren<TextMeshProUGUI>();
        normTop = normCounter.colorGradient.topLeft;
        normBottom = normCounter.colorGradient.bottomRight;
        
        if (localPlayer == null)
        {
            localPlayer = GameObject.Find("PlayerCanvas").GetComponent<PlayerFunctions>().player;
        }
        
        PopulateCourses(menuIndex);

        if (isServer)
        {
            firstPlate = true;
        }
    }

    public void Reset()
    {
        nPieces = 0;
        currentPlate = null;
        musicManager.musicPlayer.Stop();
        localPlayer = null;
    }

    private void PopulateCourses(int i)
    {
        courseStack.Clear();
        bgmStack.Clear();

        GameObject[] menu = restaurant.GetCourses(i);
        //Add courses from the selected restaurant into course stack
        foreach (GameObject course in menu)
        {
            courseStack.Push(course);
        }

        AudioClip[] Bgm = restaurant.bgmClips;
        //Add music from selected restaurant into music 
        foreach (AudioClip track in Bgm)
        {
            bgmStack.Push(track);
        }

    }

    public bool PopulatedCourseStack()
    {
        return courseStack.Count == 4 && localPlayer != null;
    }

    [Command(requiresAuthority = false)]
    public void CmdCheckNPieces()
    {
        StartCoroutine(CheckNPieces());
    }

    private IEnumerator CheckNPieces()
    {
        //Checks # of normal pieces (used to swap plates)

        for (int i = 0; i < 3; i++)
        {
            yield return 0;
        }

        nPieces = 0;

        foreach (Transform piece in currentPlate.transform)
        {
            if (piece.CompareTag("FoodPiece") && piece.GetComponent<FoodPiece>().type == "Normal")
            {
                nPieces += 1;
            }
        }

        RpcUpdatePieceCounters();
    }

    [ClientRpc]
    private void RpcUpdatePieceCounters()
    {
        Color lastTop = Color.red;
        Color lastBottom = Color.red;
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
    private void RpcUpdatePlayerEnd()
    {
        PlayerFunctions playerCanvas = GameObject.Find("PlayerCanvas").GetComponent<PlayerFunctions>();
        playerCanvas.ShowChalk();
        playerCanvas.SetZoomOut();

        currentPlate = GameObject.FindWithTag("Plate");
        playerCanvas.playerCam.transform.LookAt(currentPlate.transform);
        
        if (!firstPlate && bgmStack.Count > 1)
        {
            bgmStack.Pop();
        }

        if (musicManager.musicPlayer != null)
        {
            musicManager.PlayBGM(bgmStack.Peek());
        }

        if (isServer)
        {
            CmdCheckNPieces();
        }
    }
    
    [ClientRpc]
    private void RpcAddCourseCounter()
    {
        if (localPlayer != null && stateManager.activePlayers.Contains(localPlayer.netIdentity))
        {
            localPlayer.courseCount += 1;
        }
    }

    [ClientRpc]
    private void RpcFaceCenter()
    {
        PlayerFunctions playerCanvas = GameObject.Find("PlayerCanvas").GetComponent<PlayerFunctions>();
        playerCanvas.FaceCenter();
    }

    [Command(requiresAuthority = false)]
    private void CmdAssignPlate(GameObject plate)
    {
        plate.name += localPlayer.courseCount;
        
        foreach (NetworkIdentity player in stateManager.activePlayers)
        {
            player.GetComponent<PlayerManager>().currentPlate = plate.name;
        }
    }

    private IEnumerator CourseTransitionAnim()
    {
        Vector3 managerPos = transform.position;
        Vector3 platterStartPos = new Vector3(managerPos.x, managerPos.y + 50f, managerPos.z);
        GameObject platterInstance = Instantiate(platterTop, platterStartPos, Quaternion.identity);
        NetworkServer.Spawn(platterInstance);
        LeanTween.moveY(platterInstance, managerPos.y, 1.8f).setEaseInSine(); 

        RpcAddCourseCounter();

        yield return new WaitForSeconds(0.5f);
        
        RpcFaceCenter();

        yield return new WaitForSeconds(2f);

        if (currentPlate != null)
        {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("FoodPiece"))
            {
                NetworkServer.Destroy(obj);
            }

            NetworkServer.Destroy(currentPlate);
        }

        if (!firstPlate && courseStack.Count > 1)
        {
            courseStack.Pop();
        }
        
        currentPlate = Instantiate(courseStack.Peek(), transform.position, Quaternion.identity);
        Vector3 platePos = currentPlate.transform.position;
        stateManager.platePos = new Vector3(platePos.x, platePos.y + 2.5f, platePos.z);
        NetworkServer.Spawn(currentPlate);

        yield return new WaitForSeconds(0.5f);
        
        CmdAssignPlate(currentPlate);

        yield return new WaitUntil(PlateSpawnedOnAllPlayers);

        RpcUpdatePlayerEnd();

        LeanTween.moveY(platterInstance, platterStartPos.y, 1.8f).setEaseInSine().setLoopPingPong();
        currentPlate.transform.position = managerPos;
        yield return new WaitForSeconds(1f);
        NetworkServer.Destroy(platterInstance);
        firstPlate = false;
    }
    
    private bool PlateSpawnedOnAllPlayers()
    {
        foreach (NetworkIdentity player in stateManager.activePlayers)
        {
            if (player == null || player.GetComponent<PlayerManager>().currentPlate != currentPlate.name)
            {
                return false;
            }
        }

        return true;
    }

    [ServerCallback]
    public void NextCourse()
    {
        StartCoroutine(CourseTransitionAnim());
    }
}
