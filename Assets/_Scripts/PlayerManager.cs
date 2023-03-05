using UnityEngine;
using Mirror;
using TMPro;

public class PlayerManager : NetworkBehaviour
{
    #region Attributes
    
    [HideInInspector]
    public int scrollCount, courseCount, pieceCount, nPiecesEaten;
    
    [SyncVar]
    public int health;

    [SyncVar]
    public string currentPlate;

    public readonly SyncList<PlayerManager> myTeam = new();

    public bool actionable, hasWon;

    [SyncVar] 
    public bool isEncouraged, hasRecommended, hasTalked, orderVictim, canContinue, canSteal;

    [SyncVar] 
    public string stolenScroll;

    [SyncVar(hook=nameof(SyncPsn))] 
    public bool psn0, psn1, psn2, psn3;

    [HideInInspector]
    public bool[] psnArray = new bool[4];
    
    [SyncVar(hook=nameof(SyncRecommended))]
    public GameObject recommendedPiece;

    private GameObject currentRecommendFlag;

    private ScrollArray scrollArray;
    
    [HideInInspector]
    public GameObject playerCam;
    
    [HideInInspector]
    public PlayerFunctions playerCanvas;

    [Header("Health Bar")]
    public Color[] barGradient = new Color[4]; 
    private SpriteRenderer fill;
    public float[] fillLengths = new float[4];

    #endregion

    #region Setup
    
    public void Start()
    {
        canContinue = false;
        hasWon = false;
        health = 2;
        scrollCount = 0;
        courseCount = 0;
        pieceCount = 0;
        nPiecesEaten = 0;
        actionable = false;
        hasTalked = false;
        isEncouraged = false;
        hasRecommended = false;
        orderVictim = false;
        stolenScroll = null;
        fill = transform.Find("PlayerTag").Find("PlayerHealth").Find("Fill").GetComponent<SpriteRenderer>();
        scrollArray = GetComponent<ScrollArray>();

        for (int i = 0; i < 4; i++)
        {
            psnArray[i] = false;
        }

        if (!isLocalPlayer)
        {
            playerCanvas = GameObject.Find("PlayerCanvas").GetComponent<PlayerFunctions>();
            playerCam.SetActive(false);
        }
        
        else
        {
            InitScrollArray();
        }
    }
    
    [Command(requiresAuthority = false)]
    public void CmdSwitchContinueState(bool state)
    {
        canContinue = state;
    }
    
    [ClientRpc]
    public void RpcRenamePlayer(string newName)
    {
        netIdentity.name = newName;
        gameObject.name = netIdentity.name;
        transform.Find("PlayerTag").Find("Name").GetComponent<TextMeshPro>().text = newName;
    }

    [ClientRpc]
    public void RpcStartOnLocal()
    {
        if (isLocalPlayer)
        {
            playerCanvas = GameObject.Find("PlayerCanvas").GetComponent<PlayerFunctions>();

            Transform playerModel = transform.Find("Model").GetChild(0);
            
            playerCam.SetActive(true);
            transform.Find("PlayerTag").gameObject.SetActive(false);
            
            foreach (Transform child in playerModel)
            {
                if (child.GetComponent<SkinnedMeshRenderer>() != null)
                {
                    child.GetComponent<SkinnedMeshRenderer>().enabled = false;
                }
            }
            
            PlayerPrefs.SetInt("gamesJoined", PlayerPrefs.GetInt("gamesJoined", 0) + 1);
            
            playerCanvas.OnStartGame(this);
            
            CmdChangeHealth(health);
        }
        
        GetComponent<CameraActions>().OnStartGame();
    }

    [ClientRpc]
    public void RpcAddPlayerModel(int index)
    {
        RestaurantContents restaurant = GameObject.FindWithTag("Restaurant").GetComponent<RestaurantContents>();
        Transform modelContainer = transform.Find("Model");
        GameObject newPlayerModel = Instantiate(restaurant.playerModels[index], modelContainer, false);
        //NetworkServer.Spawn(newPlayerModel);
    }

    #endregion

    #region Player Health

    [Command(requiresAuthority = false)]
    public void CmdChangeHealth(int value)
    {
        health = value;
        RpcSetHealth(value);
    }

    [ClientRpc]
    private void RpcSetHealth(int value) 
    {
        Vector3 fillLocalScale = fill.transform.localScale;
        fill.color = barGradient[value];
        
        fill.transform.localScale = new Vector3(fillLengths[value], fillLocalScale.y, fillLocalScale.z);
    }
    public void SyncPsn(bool oldValue, bool newValue) 
    {
        psnArray[0] = psn0;
        psnArray[1] = psn1;
        psnArray[2] = psn2;
        psnArray[3] = psn3;
    }

    #endregion

    #region Recommend
    
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
    private void RpcAddRecommend(string newName, Transform recommend, Transform piece)
    {
        currentRecommendFlag = recommend.gameObject;
        recommend.Find("FlagSprite").Find("PlayerName").GetComponent<TextMeshPro>().text = newName;
        recommend.SetParent(piece, false);
        StartCoroutine(playerCanvas.SpawnBillboard(recommend));
    }
    
    [ClientRpc]
    private void RpcDestroyRecommend(GameObject flag)
    {
        StartCoroutine(playerCanvas.DespawnBillboard(flag));
        currentRecommendFlag = null;
    }
    
    public void SyncRecommended(GameObject oldValue, GameObject newValue)
    {
        if (oldValue == null)
        {
            //If the piece doesn't have any flags already, create one 

            if (isServer)
            {
                currentRecommendFlag = Instantiate(playerCanvas.recommendFlag, recommendedPiece.transform, false);
                NetworkServer.Spawn(currentRecommendFlag);
                RpcAddRecommend(netIdentity.name, currentRecommendFlag.transform, recommendedPiece.transform);
            }
            
            
        }
        
        else if (newValue == null)
        {
            //If destroying pre-existing recommend

            if (isServer)
            {
               RpcDestroyRecommend(currentRecommendFlag); 
            }
            
        }
        
        else
        {
            //Replace flag with a new one at a different piece

            if (isServer)
            {
                RpcDestroyRecommend(currentRecommendFlag);
                
                currentRecommendFlag = Instantiate(playerCanvas.recommendFlag, recommendedPiece.transform, false);
                NetworkServer.Spawn(currentRecommendFlag);
                RpcAddRecommend(netIdentity.name, currentRecommendFlag.transform, recommendedPiece.transform);
            }
        }
        hasRecommended = true;
    }
    
    #endregion

    #region Scrolls

    [Command(requiresAuthority = false)]
    private void InitScrollArray()
    {
        if (scrollArray.playerScrolls != null)
        {
            foreach (var scroll in scrollArray.allScrolls)
            {
                scrollArray.playerScrolls.Add(scroll);
                scrollArray.scrollAmounts.Add(0);
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdSetScrollVictim(string scroll)
    {
        stolenScroll = scroll;
    }
    
    #endregion
}