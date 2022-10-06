using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{

    [HideInInspector]
    public int scrollCount, courseCount, pieceCount, nPiecesEaten;
    
    public int health;

    [SyncVar]
    public string currentPlate;

    public bool actionable;

    [SyncVar] 
    public bool isEncouraged, hasRecommended, hasTalked, orderVictim, canContinue;
    
    public float accumulatedTime;

    [SyncVar(hook=nameof(SyncPsn))] 
    public bool psn0, psn1, psn2, psn3;

    [HideInInspector]
    public bool[] psnArray = new bool[4];
    
    [SyncVar(hook=nameof(SyncRecommended))]
    public GameObject recommendedPiece;

    
    public GameObject currentRecommend;
    
    private GameObject playerCam;
    private Transform playerModel;
    private PlayerFunctions playerCanvas;

    public void Start()
    {
        playerCam = transform.Find("Camera").gameObject;
        playerCam.SetActive(false);
        
        canContinue = false;
        health = 2;
        scrollCount = 0;
        courseCount = 0;
        pieceCount = 0;
        nPiecesEaten = 0;
        accumulatedTime = 0;
        actionable = false;
        hasTalked = false;
        isEncouraged = false;
        hasRecommended = false;
        orderVictim = false;
        recommendedPiece = null;
        
        for (int i = 0; i < 4; i++)
        {
            psnArray[i] = false;
        }

        playerModel = transform.Find("Model");

        if (isLocalPlayer)
        {
            playerCam.SetActive(true);
            
            foreach (Transform child in playerModel)
            {
                if (child.GetComponent<SkinnedMeshRenderer>() != null)
                {
                    child.GetComponent<SkinnedMeshRenderer>().enabled = false;
                }
            }
            
            PlayerPrefs.SetInt("gamesJoined", PlayerPrefs.GetInt("gamesJoined", 0) + 1);
            
            playerCanvas = GameObject.Find("PlayerCanvas").GetComponent<PlayerFunctions>();
            playerCanvas.OnStartGame(this);
        }
    }
    
    [Command(requiresAuthority = false)]
    public void CmdSwitchContinueState(bool state)
    {
        canContinue = state;
        Debug.LogWarning(name + ": " + state); //This debug cannot be removed...why?
    }

    [TargetRpc]
    public void TargetAddPlayerModel(NetworkConnection conn,int index)
    {
        RestaurantContents restaurant = GameObject.FindWithTag("Restaurant").GetComponent<RestaurantContents>();
        Transform modelContainer = transform.Find("Model");
        GameObject newPlayerModel = Instantiate(restaurant.playerModels[index], modelContainer, false);
        NetworkServer.Spawn(newPlayerModel);

        foreach (Transform item in newPlayerModel.transform)
        {
            item.SetParent(modelContainer, false);
        }
        
        NetworkServer.Destroy(newPlayerModel);
    }

    public void SyncPsn(bool oldValue, bool newValue)
    {
        psnArray[0] = psn0;
        psnArray[1] = psn1;
        psnArray[2] = psn2;
        psnArray[3] = psn3;
    }

    [Command(requiresAuthority = false)]
    public void CmdCreateRecommend(GameObject piece)
    {
        if (piece == recommendedPiece)
        {
            recommendedPiece = null;
        }
        else
        {
            recommendedPiece = piece;
        }
    }
    
    [ClientRpc]
    public void RpcRenamePlayer(string newName)
    {
        netIdentity.name = newName;
        gameObject.name = netIdentity.name;
    }

    public void SyncRecommended(GameObject oldValue, GameObject newValue)
    {
        if (oldValue == null)
        {
            Debug.Log("PlayerManager: " + netId);
            
            //If the piece doesn't have any flags already, create one
            Vector3 pTrans = recommendedPiece.transform.position;
            currentRecommend = Instantiate(playerCanvas.recommendFlag, new Vector3(pTrans.x - 0.75f, pTrans.y + 1f, pTrans.z), Quaternion.identity);
            NetworkServer.Spawn(currentRecommend);
            //currentRecommend.transform.Find("PlayerName").GetComponent<TextMeshProUGUI>().text = transform.name;
            StartCoroutine(playerCanvas.SpawnBillboard(currentRecommend, recommendedPiece.transform));
        }
        
        else if (newValue == null)
        {
            //If destroying pre-existing recommend
            StartCoroutine(playerCanvas.DespawnBillboard(currentRecommend));
            currentRecommend = null;
        }
        
        else
        {
            //Replace flag with a new one at a different piece
            StartCoroutine(playerCanvas.DespawnBillboard(currentRecommend));
            Vector3 pTrans = recommendedPiece.transform.position;
            currentRecommend = Instantiate(playerCanvas.recommendFlag, new Vector3(pTrans.x - 0.75f, pTrans.y + 1f, pTrans.z), Quaternion.identity);
            currentRecommend.transform.SetParent(recommendedPiece.transform);
            NetworkServer.Spawn(currentRecommend);
            //currentRecommend.transform.Find("PlayerName").GetComponent<TextMeshProUGUI>().text = transform.name;
            StartCoroutine(playerCanvas.SpawnBillboard(currentRecommend, recommendedPiece.transform));
        }
        hasRecommended = true;
    }
}