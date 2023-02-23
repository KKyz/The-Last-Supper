using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using Unity.Mathematics;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerFunctions : NetworkBehaviour
{
    [HideInInspector]
    public ScrollArray playerScrolls;
    
    [HideInInspector]
    public PlayerManager player;

    [HideInInspector] 
    public StateManager stateManager;

    [HideInInspector]
    public MealManager mealManager;
    
    [HideInInspector]
    public MusicManager musicManager;
    
    [HideInInspector]
    public TextMeshProUGUI infoText;

    [Header("Popups")] 
    public GameObject drinkMenu;
    public GameObject talkMenu;
    public GameObject drinkPlate;
    public GameObject chalkBoard;
    public GameObject recommendFlag;
    public GameObject typeFlag;
    public GameObject fakeFlag;
    public GameObject swapFlag;
    public GameObject receipt;
    public GameObject vomitSplash;
    public GameObject healthSplash;
    public GameObject smokeSplash;
    public GameObject scrollInfo;
    public GameObject stealMenu;

    [Header("SFX")] 
    public AudioClip poisonSfx;
    public AudioClip healthSfx;
    public AudioClip scrollGetSfx;
    public AudioClip scrollLoseSfx;
    public AudioClip nextCourseSfx;
    public AudioClip popupSfx;
    public AudioClip eatingSfx;
    public AudioClip flagSfx;
    public AudioClip playerActiveSfx;
    public AudioClip quakeSfx;
    public AudioClip fakePoisonSfx;
    

    [Header("Miscellaneous")] 
    public Sprite dropdownUp;
    public Sprite dropdownDown;
    
    //[HideInInspector]
    public string currentState;
    
    [HideInInspector]
    public bool countTime;

    [HideInInspector]
    public float accumulatedTime;

    [HideInInspector] 
    public GameObject playerCam;

    private EnableDisableScrollButtons buttonToggle;
    private GameObject smellTarget, smellConfirm, swapConfirm, fakeConfirm, decoyTarget, chatPanel;
    public GameObject openPopup;
    private Vector3 zoomOutPos;
    private FadeInOut fade;
    private CameraActions camActions;
    private ShowHealth healthBar;
    private Animator playerAnim;
    private AudioSource uiAudio;
    [SerializeField]private SpawnPiece plate;
    private readonly List<GameObject> smellTargets = new();
    private readonly List<Transform> swapTargets = new();
    private bool startCourse;
    
    private TextMeshProUGUI normCounter;
    private Color normTop;
    private Color normBottom;

    private void DebugAnim(string name)
    {
        Debug.LogWarning(name);
        playerAnim.SetTrigger(name);
    }

    public void OnStartGame(PlayerManager localPlayer)
    {
        GameObject stateManagerObj = GameObject.Find("StateManager(Clone)");
        normCounter = transform.Find("NormCounter").GetComponentInChildren<TextMeshProUGUI>();
        normTop = normCounter.colorGradient.topLeft;
        normBottom = normCounter.colorGradient.bottomRight;
        stateManager = stateManagerObj.GetComponent<StateManager>();
        mealManager = stateManagerObj.GetComponent<MealManager>();
        musicManager = stateManagerObj.GetComponent<MusicManager>();
        buttonToggle = transform.GetComponent<EnableDisableScrollButtons>();
        uiAudio = transform.GetComponent<AudioSource>();
        healthBar = transform.Find("HealthBar").GetComponent<ShowHealth>();
        smellConfirm = transform.Find("SmellConfirm").gameObject;
        swapConfirm = transform.Find("SwapConfirm").gameObject;
        chatPanel = transform.Find("ChatLog").Find("Panel").gameObject;
        fakeConfirm = transform.Find("FakeConfirm").gameObject;
        fade = transform.Find("Fade").GetComponent<FadeInOut>();
        infoText = transform.Find("Info").GetComponent<TextMeshProUGUI>();

        currentState = "Idle";
        player = null;
        openPopup = null;
        decoyTarget = null;
        accumulatedTime = 0;
        chatPanel.SetActive(false);
        smellTargets.Clear();
        swapTargets.Clear();
        fade.FadeOut(3f);

        player = localPlayer;
        camActions = localPlayer.GetComponent<CameraActions>();
        playerCam = localPlayer.transform.Find("Camera").gameObject;
        playerScrolls = localPlayer.GetComponent<ScrollArray>();
        playerAnim = localPlayer.GetComponentInChildren<Animator>();

        zoomOutPos = new Vector3(0, 0, 0);
        countTime = true;
        //playerScrolls.CmdResetScrollAmount();
        playerScrolls.CmdGiveAllScrolls();
        camActions.OnStartGame();
        buttonToggle.OnStartGame();
        
        player.canSteal = true;

        if (player.isLocalPlayer)
        {
            player.CmdChangeHealth(player.health);
        }
    }

    [Client]
    private void ZoomIn()
    {
        Vector3 direction = zoomOutPos - stateManager.platePos;
        Vector3 zoomInPos = zoomOutPos - (direction * 0.4f);
        LeanTween.move(playerCam, zoomInPos, 1f).setEaseOutSine();
    }

    public void SetZoomOut()
    {
        if (zoomOutPos == new Vector3(0, 0, 0))
        {
            zoomOutPos = playerCam.transform.position;
        }
    }

    [Client]
    private void ShowInfoText(string info)
    { 
        infoText.text = info;
        infoText.GetComponent<CanvasGroup>().alpha = 0;
        infoText.GetComponent<InfoText>().ShowInfoText();
    }

    [Client]
    private void StartAction()
    {
        buttonToggle.ToggleButtons(3);
        ZoomIn();
    }

    [Client]
    private void FakeSplash()
    {
        GameObject sSplash = Instantiate(smokeSplash, Vector2.zero, quaternion.identity);
        uiAudio.PlayOneShot(fakePoisonSfx);
        sSplash.transform.SetParent(transform, false);
        camActions.ShakeCamera(1.5f);
    }

    [Client]
    public void Poison(bool splash)
    {
        if (player.health >= 1)
        {
            player.health -= 1;
            player.CmdChangeHealth(player.health);
            healthBar.SetHealth(player.health);
            if (splash)
            { 
                GameObject vSplash = Instantiate(vomitSplash, Vector2.zero, quaternion.identity);
                vSplash.transform.SetParent(transform, false);

                //DebugAnim("PoisonTr");
                uiAudio.PlayOneShot(poisonSfx);
                camActions.ShakeCamera(1.5f);
            }
        }
    }
    
    [Client]
    private void Health()
    {
        if (player.health < 3)
        {
            player.health += 1;
            player.CmdChangeHealth(player.health);
            uiAudio.PlayOneShot(healthSfx);
            healthBar.SetHealth(player.health);
            
            GameObject hSplash = Instantiate(healthSplash, Vector2.zero, quaternion.identity);
            hSplash.transform.SetParent(transform, false);
        }
    }
    
    [Command]
    public void CmdQuake()
    {
        RpcQuakeAnim();
    }

    [ClientRpc]
    private void RpcQuakeAnim()
    {
        StartCoroutine(QuakeFade());
    }
    
    private IEnumerator QuakeFade()
    {
        //DebugAnim("QuakeTr"); 
        uiAudio.PlayOneShot(quakeSfx);
        camActions.ShakeCamera(4.5f);
        fade.FadeIn(1.5f);
        buttonToggle.ToggleButtons(6);
        yield return new WaitForSeconds(2f);
        
        if (stateManager.currentPlayer == player.gameObject)
        {
            plate.Shuffle();
            playerScrolls.CmdRemoveScrollAmount("Quake");
            player.scrollCount += 1;
        }

        fade.FadeOut(1f);
    }
    
    [Client]
    public void Slap()
    {
        //DebugAnim("SlapTr");
        ResetActions();
        stateManager.CmdNextPlayer();
        playerScrolls.CmdRemoveScrollAmount("Slap");
        player.scrollCount += 1;
    }
    
    [Client]
    public void Skip()
    {
        //DebugAnim("SkipTr");
        player.orderVictim = false;
        ResetActions();
        stateManager.CmdNextPlayer();
        playerScrolls.CmdRemoveScrollAmount("Skip");
        player.scrollCount += 1;
    }
    
    [Client]
    public void Smell()
    {
        currentState = "Smelling";
        StartAction();
        ShowInfoText("Select up to three pieces to reveal");
    }
    
    [Client]
    public void ConfirmSmell(bool removeScroll)
    {
        //DebugAnim("SmellTr");
        foreach (GameObject piece in smellTargets)
        {
            foreach (Transform flag in piece.transform)
            {
                if (flag.CompareTag("TypeFlag"))
                {
                    StartCoroutine(flag.GetComponentInChildren<SetFlagType>().SetFlag());
                }
            }
        }

        if (removeScroll)
        {
            StartCoroutine(buttonToggle.ButtonDisable(smellConfirm.transform));
            playerScrolls.CmdRemoveScrollAmount("Smell");
            player.scrollCount += 1;   
        }
        
        ResetActions();
    }
    
    public void ConfirmFake()
    {
        //DebugAnim("DecoyTr"); 
        if (decoyTarget.transform.parent.GetComponent<FoodPiece>().type == "Normal")
        {
            mealManager.CmdCheckNPieces();
        }
        
        decoyTarget.transform.parent.GetComponent<FoodPiece>().FakePsn();
        foreach (Transform flag in decoyTarget.transform.parent)
        {
            if (flag.CompareTag("TypeFlag"))
            {
                StartCoroutine(flag.GetComponentInChildren<SetFlagType>().SetFlag());
            }
        }
        playerScrolls.CmdRemoveScrollAmount("Decoy");
        player.scrollCount += 1;
        ResetActions();
    }

    [Client]
    public void FakePoison()
    {
        currentState = "Poisoning";
        StartAction();
        ShowInfoText("Select a piece to dummy-out");
    }

    [Client]
    public void Swap()
    {
        currentState = "Swapping";
        StartAction();
        ShowInfoText("Select two pieces to swap around");
    }
    
    public void ConfirmSwap()
    {
        //DebugAnim("SwapTr");
        NetworkIdentity localPlayer = player.transform.GetComponent<NetworkIdentity>();
        Transform[] swapArray = {swapTargets[0], swapTargets[1]};
        CmdSyncSwap(swapTargets[0].GetComponent<FoodPiece>(), swapTargets[1].GetComponent<FoodPiece>(), localPlayer, swapArray);
        
        foreach (Transform piece in swapTargets)
        {
            foreach (Transform flag in piece)
            {
                if (flag.CompareTag("SwapFlag"))
                {
                    flag.name = "PlantedFlag";
                    StartCoroutine(DespawnBillboard(flag.gameObject));
                }
            }
        }

        playerScrolls.CmdRemoveScrollAmount("Swap");
        player.scrollCount += 1;
        ResetActions();
    }

    [Command]
    private void CmdSyncSwap(FoodPiece target1, FoodPiece target2, NetworkIdentity playerID, Transform[] targets)
    {
        (target1.type, target2.type) = (target2.type, target1.type);
        NetworkConnection conn = playerID.connectionToClient;
        TargetUpdateFlag(conn, targets);
    }
    
    [TargetRpc]
    private void TargetUpdateFlag(NetworkConnection target, Transform[] targets)
    {
        foreach (Transform piece in targets)
        {
            foreach (Transform flag in piece)
            {
                if (flag.CompareTag("TypeFlag"))
                {
                    StartCoroutine(flag.GetComponentInChildren<SetFlagType>().SetFlag());
                }
            }
        }
    }
    
    [TargetRpc]
    public void TargetSendMessage(NetworkConnection target, string message, string senderName)
    {
        chatPanel.GetComponentInChildren<TextMeshProUGUI>().text += "\n" + senderName + ": " + message;
        chatPanel.SetActive(true);
    }

    [Client]
    public void ToggleChatPanel()
    {
        Image dropdownArrow = transform.Find("ChatLog").Find("DropdownArrow").GetComponent<Image>();
        
        if (dropdownArrow.sprite == dropdownDown)
        {
            dropdownArrow.sprite = dropdownUp;
        }
        else
        {
            dropdownArrow.sprite = dropdownDown;
        }
        
        chatPanel.SetActive(!chatPanel.activeSelf);
    }
    
    [Client]
    public void OrderDrink()
    {
        //DebugAnim("OrderTr"); 
        openPopup = Instantiate(drinkMenu, Vector2.zero, quaternion.identity);
        uiAudio.PlayOneShot(popupSfx);
        openPopup.transform.SetParent(transform, false);
        openPopup.transform.SetSiblingIndex(transform.childCount - 2);
        openPopup.GetComponent<SpawnMenu>().SlideInMenu();
        buttonToggle.ToggleButtons(6);
    }

    [Client]
    public void RemoveDrinkScroll()
    {
        playerScrolls.CmdRemoveScrollAmount("Order");
        player.scrollCount += 1;
    }

    [Client]
    public void RemoveStealScroll(string scroll)
    {
        player.canSteal = false;
        player.scrollCount += 1;
        ShowScrollInfo(scroll);
    }
    
    [Client]
    private void ReceiveDrink()
    {
        openPopup = Instantiate(drinkPlate, Vector2.zero, quaternion.identity);
        openPopup.name = "DrinkMenu";
        uiAudio.PlayOneShot(popupSfx);
        openPopup.transform.SetParent(transform, false);
        openPopup.transform.SetSiblingIndex(transform.childCount - 2);
        openPopup.GetComponent<SpawnMenu>().SlideInMenu();
        buttonToggle.ToggleButtons(5);
        player.orderVictim = false;
        ShowInfoText("Select a glass to drink from");
    }
    
    [Client]
    private void StealResults(string pieceType)
    {
        openPopup = Instantiate(scrollInfo, Vector2.zero, quaternion.identity);
        openPopup.name = "StealResults";
        
        uiAudio.PlayOneShot(scrollLoseSfx);
        openPopup.transform.SetParent(transform, false);
        openPopup.transform.SetSiblingIndex(transform.childCount - 2);
        openPopup.GetComponent<SpawnMenu>().SlideInMenu();

        openPopup.transform.Find("Icon").GetComponent<Image>().sprite = playerScrolls.GetSprite(pieceType);
        openPopup.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = ("A" + pieceType + " scroll was stolen!");
        openPopup.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = "You have lost a " + pieceType + " scroll.";
        buttonToggle.ToggleButtons(6);
        player.CmdSetScrollVictim(null);
    }

    [Client]
    public void Eat()
    {
        currentState = "Eating"; 
        StartAction();
        ShowInfoText("Select a piece to eat (eating will end your turn)");
    }

    [Client]
    public void Recommend()
    {
        currentState = "Recommending";
        StartAction();
        ShowInfoText("Select a piece to recommend");
    }
    
    [Client]
    public void Steal()
    {
        currentState = "Stealing"; 
        openPopup = Instantiate(stealMenu, Vector2.zero, quaternion.identity);
        uiAudio.PlayOneShot(popupSfx);
        openPopup.transform.SetParent(transform, false);
        openPopup.transform.SetSiblingIndex(transform.childCount - 2);
        openPopup.GetComponent<SpawnMenu>().SlideInMenu();
        buttonToggle.ToggleButtons(6);
        //player.canSteal = false;
    }

    [Client]
    public void SpawnTalkMenu()
    {
        currentState = "Talking";
        uiAudio.PlayOneShot(popupSfx);
        openPopup = Instantiate(talkMenu, Vector2.zero, quaternion.identity);
        openPopup.transform.SetParent(transform, false);
        openPopup.transform.SetSiblingIndex(transform.childCount - 2);
        openPopup.GetComponent<SpawnMenu>().SlideInMenu();
        buttonToggle.ToggleButtons(6);
    }

    [Client]
    public void Encourage()
    {
        //DebugAnim("TauntTr");
        stateManager.CmdNextEncourage();
        ResetActions();
        playerScrolls.CmdRemoveScrollAmount("Taunt");
        player.scrollCount += 1;
    }

    [Client]
    private void ShowScrollInfo(string pieceType)
    {
        openPopup = Instantiate(scrollInfo, Vector2.zero, quaternion.identity);
        openPopup.name = "ScrollInfo";
        
        uiAudio.PlayOneShot(scrollGetSfx);
        openPopup.transform.SetParent(transform, false);
        openPopup.transform.SetSiblingIndex(transform.childCount - 2);
        openPopup.GetComponent<SpawnMenu>().SlideInMenu();

        openPopup.transform.Find("Icon").GetComponent<Image>().sprite = playerScrolls.GetSprite(pieceType);
        openPopup.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = ("You've got a " + pieceType + " scroll!");
        openPopup.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = playerScrolls.GetDescription(pieceType);
        buttonToggle.ToggleButtons(6);
    }

    [Client]
    private void Die()
    {
        ResetActions();
        stateManager.CmdRemovePlayer(player.netIdentity);
        if (openPopup != null){Destroy(openPopup);}
        openPopup = Instantiate(receipt, Vector2.zero, quaternion.identity);
        openPopup.name = "LoseScreen";

        if (player.pieceCount <= 2)
        {
            openPopup.transform.Find("Banner").GetComponent<TextMeshProUGUI>().text = "Very Unlucky"; 
        }
        else if (stateManager.activePlayers.Count == 0)
        {
            openPopup.transform.Find("Banner").GetComponent<TextMeshProUGUI>().text = "This Game Has Ended"; 
        }
        else
        {
            openPopup.transform.Find("Banner").GetComponent<TextMeshProUGUI>().text = "You Lose";  
        }
        
        openPopup.GetComponent<ShowStats>().LoadStats(player);
        openPopup.transform.SetParent(transform, false);
        openPopup.transform.SetSiblingIndex(transform.childCount - 2);
        buttonToggle.ToggleButtons(6);
        countTime = false;
        StartCoroutine(musicManager.PlayLoseResultBGM());

        if (stateManager.activePlayers.Count > 1)
        {stateManager.CmdNextPlayer();}
    }
    
    [Client]
    private void ServerGameEnd()
    {
        ResetActions();
        if (openPopup != null){Destroy(openPopup);}
        openPopup = Instantiate(receipt, Vector2.zero, quaternion.identity);
        openPopup.transform.Find("Banner").GetComponent<TextMeshProUGUI>().text = "This Game Has Ended";

        openPopup.GetComponent<ShowStats>().LoadStats(player);
        openPopup.transform.SetParent(transform, false);
        openPopup.transform.SetSiblingIndex(transform.childCount - 2);
        buttonToggle.ToggleButtons(6);
        countTime = false;
    }

    [Client]
    private void Win()
    {
        ResetActions();
        stateManager.CmdRemovePlayer(player.netIdentity);
        if (openPopup != null){Destroy(openPopup);}
        openPopup = Instantiate(receipt, Vector2.zero, quaternion.identity);
        openPopup.transform.Find("Banner").GetComponent<TextMeshProUGUI>().text = "You Win!";
        openPopup.GetComponent<ShowStats>().LoadStats(player);
        openPopup.transform.SetParent(transform, false);
        openPopup.transform.SetSiblingIndex(transform.childCount - 2);
        buttonToggle.ToggleButtons(6);
        countTime = false;
        StartCoroutine(musicManager.PlayWinResultBGM());
        player.hasWon = true;
    }
    

    public void ShowChalk()
    {
        SpawnPiece chalkData = GameObject.FindWithTag("Plate").GetComponent<SpawnPiece>();
        plate = chalkData;
        
        if (stateManager.activePlayers.Contains(player.netIdentity))
        {
            string chalkDescription = ""; 
        
            openPopup = Instantiate(chalkBoard, Vector2.zero, quaternion.identity);
            openPopup.transform.SetParent(transform, false);
            openPopup.transform.SetSiblingIndex(transform.childCount - 2);
            openPopup.GetComponent<SpawnMenu>().SlideInMenu();
            uiAudio.PlayOneShot(nextCourseSfx);
            buttonToggle.ToggleButtons(6);
            player.CmdSwitchContinueState(false);
            
            openPopup.transform.Find("Image").GetComponent<Image>().sprite = chalkData.chalkSprite;
            openPopup.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = chalkData.courseName;
            chalkDescription += "- " + chalkData.pieceTypes[0] + " Empty Pieces";
            chalkDescription += "\n - " + chalkData.pieceTypes[1] + " Poison Pieces";
            chalkDescription += "\n - " + chalkData.pieceTypes[2] + " Special Pieces";
            chalkDescription += "\n - " + (chalkData.pieceTypes[0] +  chalkData.pieceTypes[1] + chalkData.pieceTypes[2]) +" Total Pieces";
            openPopup.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = chalkDescription;

            if (isServer)
            {
                RpcCheckNPieces();
            }
        }
    }

    [Client]
    public void ResetActions()
    {
        currentState = "Idle";
        player.orderVictim = false; 
        LeanTween.move(playerCam, zoomOutPos, 1f).setEaseOutSine();

        if (player.actionable)
        {
            //DebugAnim("ActiveTr");
        }
        else
        {
            //DebugAnim("IdleTr"); 
        }

        if (infoText.gameObject.activeInHierarchy)
        {
            infoText.GetComponent<InfoText>().CloseInfoText();
        }

        if (decoyTarget != null)
        {
            StartCoroutine(DespawnBillboard(decoyTarget));
        }

        foreach (GameObject targetPiece in smellTargets)
        {
            foreach (Transform flag in targetPiece.transform)
            {
                if (flag.CompareTag("TypeFlag") && flag.name != "PlantedFlag")
                {
                    StartCoroutine(DespawnBillboard(flag.gameObject));
                }
            }
        }
        
        foreach (Transform swapPiece in swapTargets)
        {
            foreach (Transform flag in swapPiece)
            {
                if (flag.CompareTag("SwapFlag"))
                {
                    StartCoroutine(DespawnBillboard(flag.gameObject));
                }
            }
        }

        buttonToggle.ToggleButtons(6);
        smellTargets.Clear();
        swapTargets.Clear();
        StartCoroutine(buttonToggle.ButtonDisable(swapConfirm.transform));
        StartCoroutine(buttonToggle.ButtonDisable(smellConfirm.transform));
        StartCoroutine(buttonToggle.ButtonDisable(fakeConfirm.transform));
    }
    
    [Command]
    private void CmdDestroyPiece(GameObject piece)
    {
        NetworkServer.Destroy(piece);
        RpcCheckNPieces();
    }
    
    public IEnumerator DespawnBillboard(GameObject billboard)
    {
        uiAudio.PlayOneShot(flagSfx);
        Vector3 startPos = billboard.transform.position;
        Vector3 goalPos = new Vector3(startPos.x, (startPos.y + 1f), startPos.z);
        LeanTween.alpha(billboard, 0, 0.4f);
        LeanTween.move(billboard, goalPos, 0.6f);
        yield return new WaitForSeconds(0.6f);

        if (billboard != null)
        {
            NetworkServer.Destroy(billboard);
        }
    }

    public IEnumerator SpawnBillboard(Transform billboard)
    {
        if (billboard.parent != null)
        {
            billboard.position = billboard.parent.Find("FlagCenter").position;
        }

        billboard.localScale = new Vector3(0.07f, 0.07f, 0.07f);
        uiAudio.PlayOneShot(flagSfx);
        billboard.transform.LookAt(player.playerCam.transform.position);

        Vector3 goalPos = billboard.position;
        Vector3 startPos = new Vector3(goalPos.x, (goalPos.y + 1f), goalPos.z);
        billboard.position = startPos; 
        billboard.GetComponent<SpriteRenderer>().color = new  Color(1, 1, 1, 0);
        LeanTween.alpha(billboard.gameObject, 1, 0.9f);
        LeanTween.move(billboard.gameObject, goalPos, 0.6f);
        yield return 0;
    }


    [ClientRpc]
    private void RpcCheckNPieces()
    {
        StartCoroutine(CheckNPieces());
    }

    private IEnumerator CheckNPieces()
    {
        //Find Number Of Missing Pieces 
        for (int i = 0; i < 3; i++)
        {
            yield return 0;
        }
        
        mealManager.nPieces = 0;
        foreach (Transform piece in plate.transform)
        {
            if (piece.CompareTag("FoodPiece") && piece.GetComponent<FoodPiece>().type == "Normal")
            {
                mealManager.nPieces += 1;
            }
        }
        
        //Update NPiece Counters
        
        Color lastTop = Color.red;
        Color lastBottom = Color.red;
        if (mealManager.nPieces <= 1)
        {
            normCounter.colorGradient = new VertexGradient(lastTop, lastTop, lastBottom, lastBottom);
        }
        else
        {
            normCounter.colorGradient = new VertexGradient(normTop, normTop, normBottom, normBottom);
        }

        normCounter.text = "# Of Empty Pieces Left: " + mealManager.nPieces;
        
        if (mealManager.nPieces == 0 && stateManager.currentPlayer == player.gameObject)
        {
            CmdNextCourse();
        }
    }
    
    [Command]
    private void CmdNextCourse()
    {
        RpcForceButtonsOff();
        mealManager.NextCourse();
    }

    [ClientRpc]
    private void RpcForceButtonsOff()
    {
        player.CmdSwitchContinueState(false);
        buttonToggle.ToggleButtons(6);
    }

    [Command(requiresAuthority =  false)]
    private void CmdAddAuthority(NetworkConnectionToClient conn)
    {
        netIdentity.AssignClientAuthority(conn);
        stateManager.netIdentity.AssignClientAuthority(conn);
    }

    [Command(requiresAuthority =  false)]
    private void CmdRemoveAuthority()
    {
        netIdentity.RemoveClientAuthority();
        stateManager.netIdentity.RemoveClientAuthority();
    }

    private bool PlateIsPresent()
    {
        return plate != null;
    }
    
    private IEnumerator AddOneSmellFlag()
    {
        yield return new WaitUntil(PlateIsPresent);
        
        //Select a random piece from plate 
        plate.RefreshPieceList();
        List<Transform> unknownPieces = new();

        foreach (Transform piece in plate.transform)
        {
            if (piece.CompareTag("FoodPiece"))
            {
                unknownPieces.Add(piece);
            }
        
            foreach (Transform content in piece)
            {
                if (content.CompareTag("TypeFlag"))
                {
                    unknownPieces.Remove(piece);
                }
            }
        }

        if (unknownPieces.Count > 0)
        {
            Transform selectedPiece = unknownPieces[Random.Range(0, unknownPieces.Count - 1)];

            //Add a type flag to it
            smellTarget = Instantiate(typeFlag, selectedPiece, false);
            smellTarget.transform.position = smellTarget.transform.parent.Find("FlagCenter").position;
            smellTarget.transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);
            smellTarget.transform.LookAt(player.playerCam.transform.position);
            
            smellTarget.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            smellTargets.Add(selectedPiece.gameObject);

            ConfirmSmell(false);
        }
    }

    [Command]
    private void UpdateMaxPiecesEaten(int pieceCount)
    {
        stateManager.maxPiecesEaten = pieceCount;
    }

    void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            Poison(true);
        }

        if (Input.GetKeyDown("2"))
        {
            Health();
        }
        
        if (Input.GetKeyDown("3"))
        {
            Skip();
        }
        
        if (Input.GetKeyDown("4"))
        {
            Steal();
        }
        
        if (Input.GetKeyDown("c"))
        {
            CmdNextCourse();
        }

        if (countTime && player != null)
        {
            accumulatedTime += Time.deltaTime;
        }

        //Manages player options when playing 
        if (player != null)
        {
            //If it is the player's turn, switch to Action Buttons
            if (stateManager.currentPlayer == player.gameObject && openPopup == null && (buttonToggle.menuMode == 4 || buttonToggle.menuMode == 6 || buttonToggle.menuMode == 5))
            {
                if (!player.actionable)
                {
                    player.actionable = true;
                    player.hasRecommended = false;
                    player.hasTalked = false;
                    uiAudio.PlayOneShot(playerActiveSfx);
                    //DebugAnim("ActiveTr");
                    CmdRemoveAuthority();
                    CmdAddAuthority(player.connectionToClient);
                    StartCoroutine(AddOneSmellFlag());
                }

                if (!fade.gameObject.activeInHierarchy && stateManager.AllPlayersCanContinue())
                {
                    buttonToggle.ToggleButtons(2);
                }
            }

            //else, set player mode as inactive
            else if (stateManager.currentPlayer != player.gameObject)
            {
                player.actionable = false;

                if (openPopup != null && stateManager.AllPlayersCanContinue())
                {
                    if (openPopup.name != "ScrollInfo")
                    {
                        openPopup.GetComponent<SpawnMenu>().SlideOutMenu();
                    }
                }
                
                if (currentState != "Idle")
                {
                    ResetActions();
                }

                if (buttonToggle.menuMode != 4 && buttonToggle.menuMode != 3 && !fade.gameObject.activeInHierarchy && stateManager.AllPlayersCanContinue() && openPopup == null)
                {
                    buttonToggle.ToggleButtons(4);
                    ShowInfoText("Waiting for other players' turns..");
                }
            }
        }

        if (player != null)
        {
            if (player.health == 0 && openPopup == null && player.canContinue)
            {
                Die();
            }
            
            else if (stateManager.activePlayers.Count == 0 && openPopup != null && openPopup.name == "LoseScreen")
            {
                Destroy(openPopup);
                ServerGameEnd();
            }


            else if (stateManager.gameCanEnd && stateManager.gameMode == "Free-For-All" && stateManager.activePlayers.Count < 2 && player.health >= 1 && openPopup == null)
            {
                Win();  
            }
            
            else if (stateManager.activePlayers.Count == 0 && stateManager.gameMode == "Most Pieces" && player.pieceCount == stateManager.maxPiecesEaten && stateManager.AllPlayersCanContinue() && stateManager.maxPiecesEaten > 0)
            {
                Win();  
            }
            
            if (player.actionable)
            {
                if (player.orderVictim && openPopup == null && stateManager.AllPlayersCanContinue())
                {
                    ReceiveDrink();
                }
            }

            if (openPopup == null && player.stolenScroll != null)
            {
                StealResults(player.stolenScroll);
            }

            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit piece;
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out piece))
                {
                    if (piece.transform.CompareTag("FoodPiece"))
                    {
                        if (currentState == "Recommending")
                        {
                            player.CmdCreateRecommend(piece.transform.gameObject);
                            //DebugAnim("RecommendTr");
                            ResetActions();
                        }
                        
                        else if (player.actionable)
                        {
                            //Where the player is eating
                            if (currentState == "Eating")
                            {
                                string pieceType = piece.transform.gameObject.GetComponent<FoodPiece>().type;
                                //DebugAnim("EatTr");

                                if (pieceType == "FakePoison")
                                {
                                    FakeSplash();
                                }

                                else if (pieceType == "Poison")
                                {
                                    Poison(true);
                                }

                                else if (pieceType == "Health")
                                {
                                    Health();
                                }

                                else if (pieceType != "Normal")
                                {
                                    ShowScrollInfo(pieceType);
                                    playerScrolls.CmdAddScrollAmount(pieceType); 
                                }

                                else
                                {
                                    player.nPiecesEaten += 1;
                                }

                                if (player.nPiecesEaten >= 0)
                                { 
                                    player.canSteal = true;
                                    player.nPiecesEaten = 0;
                                }

                                Destroy(piece.transform.gameObject);
                                CmdDestroyPiece(piece.transform.gameObject);
                                player.pieceCount += 1;
                                uiAudio.PlayOneShot(eatingSfx);

                                if (player.pieceCount > stateManager.maxPiecesEaten)
                                {
                                    UpdateMaxPiecesEaten(player.pieceCount);
                                }
                                
                                stateManager.CmdNextPlayer();
                                ResetActions();
                            }

                            else if (currentState == "Smelling")
                            {
                                if (!smellTargets.Contains(piece.transform.gameObject))
                                {
                                    if (smellTargets.Count <= 2)
                                    {
                                        smellTarget = Instantiate(typeFlag, piece.transform, false);
                                        StartCoroutine(SpawnBillboard(smellTarget.transform));
                                        smellTargets.Add(piece.transform.gameObject);
                                    }

                                    if (smellTargets.Count >= 1)
                                    {
                                        StartCoroutine(buttonToggle.ButtonEnable(smellConfirm.transform));
                                    }
                                }

                                else
                                {
                                    foreach (Transform child in piece.transform)
                                    {
                                        if (child.CompareTag("TypeFlag") && child.gameObject.name != "PlantedFlag")
                                        {
                                            StartCoroutine(DespawnBillboard(child.gameObject));
                                        }
                                    }

                                    smellTargets.Remove(piece.transform.gameObject);

                                    if (smellTargets.Count <= 0)
                                    {
                                        StartCoroutine(buttonToggle.ButtonDisable(smellConfirm.transform));
                                    }
                                }
                            }

                            else if (currentState == "Poisoning")
                            {
                                if (decoyTarget == null)
                                {
                                    decoyTarget = Instantiate(fakeFlag, piece.transform);
                                    StartCoroutine(SpawnBillboard(decoyTarget.transform));
                                    StartCoroutine(buttonToggle.ButtonEnable(fakeConfirm.transform));
                                }

                                else
                                {
                                    StartCoroutine(DespawnBillboard(decoyTarget));
                                    StartCoroutine(buttonToggle.ButtonDisable(fakeConfirm.transform));
                                }
                            }

                            else if (currentState == "Swapping")
                            {
                                if (!swapTargets.Contains(piece.transform))
                                {
                                    if (swapTargets.Count < 2)
                                    {
                                        swapTargets.Add(piece.transform);
                                        GameObject newSwapFlag = Instantiate(swapFlag, piece.transform, false);
                                        StartCoroutine(SpawnBillboard(newSwapFlag.transform));
                                    }
                                }

                                else
                                {
                                    swapTargets.Remove(piece.transform);
                                    foreach (Transform flag in piece.transform)
                                    {
                                        if (flag.CompareTag("SwapFlag"))
                                        {
                                            StartCoroutine(DespawnBillboard(flag.gameObject));
                                        }
                                    }
                                }

                                if (swapTargets.Count == 2)
                                {
                                    StartCoroutine(buttonToggle.ButtonEnable(swapConfirm.transform));
                                }

                                else
                                {
                                    StartCoroutine(buttonToggle.ButtonDisable(swapConfirm.transform));
                                }
                            }

                            else if (currentState != "Idle")
                            {
                                Debug.LogWarning("STATE NOT FOUND");
                            }
                        }
                    }
                }
            }
        }
    }
}