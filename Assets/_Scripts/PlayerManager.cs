using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{

    //[HideInInspector]
    public int scrollCount, courseCount, pieceCount, nPiecesEaten;
    
    public int health;

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

    
    public GameObject currentRecommend, currentPlate;
    
    private GameObject playerCam;
    private Transform playerModel;
    private PlayerFunctions playerCanvas;

    public void OnStartGame()
    {
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

        
        playerModel = transform.Find("PlayerModel");
        playerCanvas = GameObject.Find("PlayerCanvas").GetComponent<PlayerFunctions>();
        playerCam = transform.Find("Camera").gameObject;
        playerCam.SetActive(false);
        
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
        }

        PlayerPrefs.SetInt("gamesJoined", PlayerPrefs.GetInt("gamesJoined", 0) + 1);
        playerCanvas.OnStartGame(this);
    }

    public void AddPlayerModel(int index)
    {
        //Creating player model
        RestaurantContents restaurant = GameObject.FindWithTag("Restaurant").GetComponent<RestaurantContents>();
        GameObject newPlayerModel = Instantiate(restaurant.playerModels[index], transform, false);
        newPlayerModel.name = "PlayerModel";
        NetworkServer.Spawn(newPlayerModel);
        
        //Uploading model pos to network
        NetworkTransformChild networkTransformChild = gameObject.AddComponent<NetworkTransformChild>();
        networkTransformChild.enabled = false;
        networkTransformChild.target = newPlayerModel.transform;
        networkTransformChild.enabled = true;
        
        //Uploading model animator to network
        Animator modelAnim = newPlayerModel.GetComponent<Animator>();
        NetworkAnimator networkAnimator = gameObject.AddComponent<NetworkAnimator>();
        networkAnimator.enabled = false;
        networkAnimator.animator = modelAnim;
        networkAnimator.enabled = true;
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
        gameObject.name = newName;
    }

    public void SyncRecommended(GameObject oldValue, GameObject newValue)
    {
        if (oldValue == null)
        {
            Debug.Log("PlayerManager: " + netId);
            
            //If the piece doesn't have any flags already, create one
            Vector3 pTrans = recommendedPiece.transform.position;
            currentRecommend = Instantiate(playerCanvas.recommendFlag, new Vector3(pTrans.x - 0.75f, pTrans.y + 1f, pTrans.z), Quaternion.identity);
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
            //currentRecommend.transform.Find("PlayerName").GetComponent<TextMeshProUGUI>().text = transform.name;
            StartCoroutine(playerCanvas.SpawnBillboard(currentRecommend, recommendedPiece.transform));
        }
        hasRecommended = true;
    }
}