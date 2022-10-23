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

    [Header("SFX")] 
    public AudioClip poisonSfx;
    public AudioClip healthSfx;
    public AudioClip scrollGetSfx;
    public AudioClip nextCourseSfx;
    public AudioClip popupSfx;
    public AudioClip eatingSfx;
    public AudioClip flagSfx;
    public AudioClip playerActiveSfx;
    public AudioClip quakeSfx;
    

    [Header("Miscellaneous")] 
    public Sprite dropdownUp;
    public Sprite dropdownDown;
    
    [HideInInspector]
    public string currentState;
    
    [HideInInspector]
    public bool countTime;

    private EnableDisableScrollButtons buttonToggle;
    private GameObject smellTarget, smellConfirm, swapConfirm, fakeConfirm, openPopup, fakeTarget, chatPanel;
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
        fakeTarget = null;
        chatPanel.SetActive(false);
        smellTargets.Clear();
        swapTargets.Clear();
        fade.FadeOut(1.5f);

        player = localPlayer;
        camActions = localPlayer.GetComponent<CameraActions>();
        playerScrolls = localPlayer.GetComponent<ScrollArray>();
        playerAnim = localPlayer.GetComponentInChildren<Animator>();

        countTime = true;
        //playerScrolls.ResetScrollAmount();
        buttonToggle.OnStartGame();
    }

    [Client]
    private void ShowInfoText(string info)
    {
        infoText.text = info;
        infoText.GetComponent<InfoText>().ShowInfoText();
    }

    [Client]
    private void StartAction()
    {
        buttonToggle.ToggleButtons(3);
        camActions.ZoomIn();
    }

    [Client]
    private void FakeSplash()
    {
        GameObject sSplash = Instantiate(smokeSplash, Vector2.zero, quaternion.identity);
        sSplash.transform.SetParent(transform, false);
        camActions.ShakeCamera(1.5f);
    }

    [Client]
    public void Poison(bool splash)
    {
        if (player.health >= 1)
        {
            player.health -= 1;
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
            playerScrolls.RemoveScrollAmount("Quake");
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
        playerScrolls.RemoveScrollAmount("Slap");
        player.scrollCount += 1;
    }
    
    [Client]
    public void Skip()
    {
        //DebugAnim("SkipTr");
        player.orderVictim = false;
        ResetActions();
        stateManager.CmdNextPlayer();
        playerScrolls.RemoveScrollAmount("Skip");
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
            playerScrolls.RemoveScrollAmount("Smell");
            player.scrollCount += 1;   
        }
        
        ResetActions();
    }
    
    public void ConfirmFake()
    {
        //DebugAnim("DecoyTr"); 
        if (fakeTarget.transform.parent.GetComponent<FoodPiece>().type == "Normal")
        {
            mealManager.CmdCheckNPieces();
        }
        
        fakeTarget.transform.parent.GetComponent<FoodPiece>().FakePsn();
        foreach (Transform flag in fakeTarget.transform.parent)
        {
            if (flag.CompareTag("TypeFlag"))
            {
                StartCoroutine(flag.GetComponentInChildren<SetFlagType>().SetFlag());
            }
        }
        playerScrolls.RemoveScrollAmount("Decoy");
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

        playerScrolls.RemoveScrollAmount("Swap");
        player.scrollCount += 1;
        ResetActions();
    }

    [Command(requiresAuthority = false)]
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
        playerScrolls.RemoveScrollAmount("Order");
        player.scrollCount += 1;
    }
    
    [Client]
    private void ReceiveDrink()
    {
        Debug.LogWarning("SpawnMenu");
        openPopup = Instantiate(drinkPlate, Vector2.zero, quaternion.identity);
        openPopup.transform.name = "DrinkMenu";
        uiAudio.PlayOneShot(popupSfx);
        openPopup.transform.SetParent(transform, false);
        openPopup.transform.SetSiblingIndex(transform.childCount - 2);
        openPopup.GetComponent<SpawnMenu>().SlideInMenu();
        buttonToggle.ToggleButtons(5);
        player.orderVictim = false;
        ShowInfoText("Select a glass to drink from");
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
    public void SpawnTalkMenu()
    {
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
        playerScrolls.RemoveScrollAmount("Taunt");
        player.scrollCount += 1;
    }

    [Client]
    private void ShowScrollInfo(string pieceType)
    {
        openPopup = Instantiate(scrollInfo, Vector2.zero, quaternion.identity);
        
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
        openPopup = Instantiate(receipt, Vector2.zero, quaternion.identity);
        openPopup.transform.Find("Banner").GetComponent<TextMeshProUGUI>().text = "You Win!";
        openPopup.GetComponent<ShowStats>().LoadStats(player);
        openPopup.transform.SetParent(transform, false);
        openPopup.transform.SetSiblingIndex(transform.childCount - 2);
        buttonToggle.ToggleButtons(6);
        countTime = false;
        StartCoroutine(musicManager.PlayWinResultBGM());
        PlayerPrefs.SetInt("gamesWon", PlayerPrefs.GetInt("gamesWon", 0) + 1);
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
        camActions.ZoomOut();

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

        if (fakeTarget != null)
        {
            StartCoroutine(DespawnBillboard(fakeTarget));
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
    
    [Command(requiresAuthority = false)]
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
    
    [Command(requiresAuthority = false)]
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

    [Command(requiresAuthority = false)]
    private void CmdAddAuthority(NetworkConnectionToClient conn)
    {
        netIdentity.AssignClientAuthority(conn);
        stateManager.netIdentity.AssignClientAuthority(conn);
    }
    
    [Command(requiresAuthority = false)]
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

    void Update()
    {
        if (countTime && player != null)
        {
            player.accumulatedTime += Time.deltaTime;
            PlayerPrefs.SetFloat("playTime", PlayerPrefs.GetInt("playTime", 0) + Time.deltaTime);
        }

        //Manages player options when playing 
        if (openPopup == null && player != null)
        {
            //If it is the player's turn, switch to Action Buttons
            if (stateManager.currentPlayer == player.gameObject && (buttonToggle.menuMode == 4 || buttonToggle.menuMode == 6 || buttonToggle.menuMode == 5))
            {
                if (!player.actionable && netIdentity.hasAuthority == false)
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
                
                if (netIdentity.hasAuthority)
                {
                    CmdRemoveAuthority();
                }

                if (buttonToggle.menuMode != 4 && buttonToggle.menuMode != 3 && !fade.gameObject.activeInHierarchy && stateManager.AllPlayersCanContinue())
                {
                    buttonToggle.ToggleButtons(4);
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


            else if (stateManager.activePlayers.Count == 1 && stateManager.gameCanEnd && player.health >= 1 && openPopup == null)
            {
                Win();
            }
            
            if (player.actionable)
            {
                if (player.orderVictim && openPopup == null)
                {
                    ReceiveDrink();
                }
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
                                    playerScrolls.AddScrollAmount(pieceType); 
                                }

                                if (player.nPiecesEaten >= 10)
                                {
                                    Health();
                                    player.nPiecesEaten = 0;
                                }
                                
                                Destroy(piece.transform.gameObject);
                                CmdDestroyPiece(piece.transform.gameObject);
                                player.pieceCount += 1;
                                uiAudio.PlayOneShot(eatingSfx);
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
                                if (fakeTarget == null)
                                {
                                    fakeTarget = Instantiate(fakeFlag, piece.transform);
                                    StartCoroutine(SpawnBillboard(fakeTarget.transform));
                                    StartCoroutine(buttonToggle.ButtonEnable(fakeConfirm.transform));
                                }

                                else
                                {
                                    StartCoroutine(DespawnBillboard(fakeTarget));
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