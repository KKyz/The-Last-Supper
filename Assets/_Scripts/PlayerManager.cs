using UnityEngine;
using Mirror;
using TMPro;

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

    private GameObject currentRecommendFlag;
    
    public GameObject playerCam;
    
    public PlayerFunctions playerCanvas;

    public void Start()
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

        for (int i = 0; i < 4; i++)
        {
            psnArray[i] = false;
        }

        if (!isLocalPlayer)
        {
            playerCanvas = GameObject.Find("PlayerCanvas").GetComponent<PlayerFunctions>();
            playerCam.SetActive(false);
        }
    }
    
    [Command(requiresAuthority = false)]
    public void CmdSwitchContinueState(bool state)
    {
        canContinue = state;
    }

    [ClientRpc]
    public void RpcStartOnLocal()
    {
        if (isLocalPlayer)
        {
            playerCanvas = GameObject.Find("PlayerCanvas").GetComponent<PlayerFunctions>();

            Transform playerModel = transform.Find("Model").GetChild(0);
            
            playerCam.SetActive(true);
            
            foreach (Transform child in playerModel)
            {
                if (child.GetComponent<SkinnedMeshRenderer>() != null)
                {
                    child.GetComponent<SkinnedMeshRenderer>().enabled = false;
                }
            }
            
            PlayerPrefs.SetInt("gamesJoined", PlayerPrefs.GetInt("gamesJoined", 0) + 1);
            
            playerCanvas.OnStartGame(this);
        }
    }

    [ClientRpc]
    public void RpcAddPlayerModel(int index)
    {
        RestaurantContents restaurant = GameObject.FindWithTag("Restaurant").GetComponent<RestaurantContents>();
        Transform modelContainer = transform.Find("Model");
        GameObject newPlayerModel = Instantiate(restaurant.playerModels[index], modelContainer, false);
        //NetworkServer.Spawn(newPlayerModel); 
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
    
    [ClientRpc]
    private void RpcNameRecommend(string newName, GameObject recommend)
    {
        recommend.transform.Find("PlayerName").GetComponent<TextMeshPro>().text = newName;
    }
    
    public void SyncRecommended(GameObject oldValue, GameObject newValue)
    {
        if (oldValue == null)
        {
            //If the piece doesn't have any flags already, create one 
            Vector3 pTrans = recommendedPiece.transform.position;
            currentRecommendFlag = Instantiate(playerCanvas.recommendFlag, new Vector3(), Quaternion.identity);
            if (isServer)
            {
                NetworkServer.Spawn(currentRecommendFlag);
                RpcNameRecommend(netIdentity.name, currentRecommendFlag);
            }
            StartCoroutine(playerCanvas.SpawnBillboard(currentRecommendFlag, recommendedPiece.transform));

        }
        
        else if (newValue == null)
        {
            //If destroying pre-existing recommend
            StartCoroutine(playerCanvas.DespawnBillboard(currentRecommendFlag));
            currentRecommendFlag = null;
        }
        
        else
        {
            //Replace flag with a new one at a different piece
            StartCoroutine(playerCanvas.DespawnBillboard(currentRecommendFlag));
            Vector3 pTrans = recommendedPiece.transform.position;
            currentRecommendFlag = Instantiate(playerCanvas.recommendFlag, new Vector3(pTrans.x - 0.75f, pTrans.y + 1f, pTrans.z), Quaternion.identity);
            if (isServer)
            {
                NetworkServer.Spawn(currentRecommendFlag);
                RpcNameRecommend(netIdentity.name, currentRecommendFlag);
            }
            StartCoroutine(playerCanvas.SpawnBillboard(currentRecommendFlag, recommendedPiece.transform));
        }
        hasRecommended = true;
    }
}