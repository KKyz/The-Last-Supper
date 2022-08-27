using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using Unity.Mathematics;
using UnityEngine.UI;

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
    public TextMeshProUGUI infoText;
    
    public GameObject drinkMenu, talkMenu, drinkPlate, chalkBoard, recommendFlag, typeFlag, fakeFlag, swapFlag, receipt, vomitSplash, healthSplash, smokeSplash, scrollInfo;
    public Sprite[] scrollIcons;
    public string[] scrollDesc;
    public string currentState;
    public bool countTime;

    private EnableDisableScrollButtons buttonToggle;
    private GameObject smellTarget, smellConfirm, swapConfirm, fakeConfirm, openPopup, fakeTarget;
    private CanvasGroup fade;
    private CameraActions camActions;
    private ShowHealth healthBar;
    //private charAnimator playerAnim;
    private readonly List<GameObject> smellTargets = new();
    private readonly List<Transform> swapTargets = new();
    private bool startCourse;

    void Start()
    {
        stateManager = GameObject.Find("StateManager").GetComponent<StateManager>();
        mealManager = GameObject.Find("StateManager").GetComponent<MealManager>();
        buttonToggle = transform.GetComponent<EnableDisableScrollButtons>();
        healthBar = transform.Find("HealthBar").GetComponent<ShowHealth>();
        smellConfirm = transform.Find("SmellConfirm").gameObject;
        swapConfirm = transform.Find("SwapConfirm").gameObject;
        fakeConfirm = transform.Find("FakeConfirm").gameObject;
        fade = transform.Find("Fade").GetComponent<CanvasGroup>();
        infoText = transform.Find("Info").GetComponent<TextMeshProUGUI>();

        currentState = "Idle";
        player = null;
        openPopup = null;
        fakeTarget = null;
        smellTargets.Clear();
        swapTargets.Clear();
        FadeOut();

        StartCoroutine(PostStartCall());
    }

    private IEnumerator PostStartCall()
    {
        yield return new WaitForEndOfFrame();

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject newPlayer in players)
        {
            if (newPlayer.GetComponent<PlayerManager>().isLocalPlayer)
            {
                //The player variable is the local player
                player = newPlayer.GetComponent<PlayerManager>();
                camActions = newPlayer.GetComponent<CameraActions>();
                playerScrolls = newPlayer.GetComponent<ScrollArray>();
                //playerAnim = newPlayer.GetComponent<charAnimator>();
            }
        }
        
        countTime = true;

        if (!isServer)
        {
            ShowChalk();
        }
    }

    [Client]
    public void ShowInfoText(string info)
    {
        infoText.gameObject.SetActive(true);
        infoText.text = info;
        //infoText.GetComponent<CanvasGroup>().alpha = 0f;
        LeanTween.alpha(infoText.gameObject, 1f, 0.5f).setLoopPingPong();
    }

    [Client]
    private void StartAction()
    {
        buttonToggle.ToggleButtons(3);
        camActions.ZoomIn();
    }

    [Client]
    private void FadeIn()
    {
        fade.gameObject.SetActive(true);
        fade.alpha = 0f;
        LeanTween.alphaCanvas(fade, 1f, 1.5f);
    }

    [Client]
    private void FadeOut()
    {
        fade.gameObject.SetActive(true);
        fade.alpha = 1f;
        LeanTween.alphaCanvas(fade, 0, 1.5f);
        StartCoroutine(DisableFade());
    }

    private IEnumerator DisableFade()
    {
        yield return new WaitForSeconds(1.7f);
        fade.gameObject.SetActive(false);
    }

    [Client]
    public void FakeSplash()
    {
        GameObject sSplash = Instantiate(smokeSplash, new Vector3(0f, 0f, 0f), quaternion.identity);
        sSplash.transform.SetParent(transform, false);
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
                GameObject vSplash = Instantiate(vomitSplash, new Vector3(0f, 0f, 0f), quaternion.identity);
                vSplash.transform.SetParent(transform, false);
                
                //playerAnimation.QueueAnimation("Poisoned");
            }
        }
    }
    
    [Client]
    public void Health()
    {
        if (player.health < 3)
        {
            player.health += 1;
            healthBar.SetHealth(player.health);
            
            GameObject hSplash = Instantiate(healthSplash, new Vector3(0f, 0f, 0f), quaternion.identity);
            hSplash.transform.SetParent(transform, false);
        }
    }
    
    [Command(requiresAuthority = false)]
    public void CmdQuake()
    {
        RpcQuakeAnim();
    }

    [ClientRpc]
    public void RpcQuakeAnim()
    {
        StartCoroutine(QuakeFade());
    }
    
    public IEnumerator QuakeFade()
    {
        //playerAnimation.QueueAnimation("Quake");
        FadeIn();
        buttonToggle.ToggleButtons(6);
        yield return new WaitForSeconds(2f);
        
        if (stateManager.currentPlayer == player.gameObject)
        {
            GameObject.FindWithTag("Plate").GetComponent<SpawnPiece>().Shuffle();
            playerScrolls.AddScrollAmount(-1, 3);
            player.scrollCount += 1;
        }
        
        FadeOut();
    }
    
    [Client]
    public void Slap()
    {
        //playerAnimation.QueueAnimation("Slapping");
        ResetActions(true);
        stateManager.CmdNextPlayer();
        playerScrolls.AddScrollAmount(-1, 0);
        player.scrollCount += 1;
    }
    
    [Client]
    public void Skip()
    {
        //playerAnimation.QueueAnimation("Skip");
        player.orderVictim = false;
        ResetActions(true);
        stateManager.CmdNextPlayer();
        playerScrolls.AddScrollAmount(-1, 1);
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
    public void ConfirmSmell()
    {
        //playerAnimation.QueueAnimation("Smell");
        foreach (GameObject piece in smellTargets)
        {
            foreach (Transform flag in piece.transform)
            {
                if (flag.CompareTag("TypeFlag"))
                {
                    StartCoroutine(flag.GetComponent<SetFlagType>().SetFlag());
                }
            }
        }
        
        StartCoroutine(buttonToggle.ButtonDisable(smellConfirm.transform));
        playerScrolls.AddScrollAmount(-1, 2);
        player.scrollCount += 1;
        ResetActions(true);
    }
    
    public void ConfirmFake()
    {
        //playerAnimation.QueueAnimation("Fake");
        if (fakeTarget.transform.parent.GetComponent<FoodPiece>().type == "Normal")
        {
            StartCoroutine(mealManager.CheckNPieces());
        }
        
        fakeTarget.transform.parent.GetComponent<FoodPiece>().FakePsn();
        foreach (Transform flag in fakeTarget.transform.parent)
        {
            if (flag.CompareTag("TypeFlag"))
            {
                StartCoroutine(flag.GetComponent<SetFlagType>().SetFlag());
            }
        }
        playerScrolls.AddScrollAmount(-1, 7);
        player.scrollCount += 1;
        ResetActions(true);
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
        //playerAnimation.QueueAnimation("Swapping");
        uint playerID = player.transform.GetComponent<NetworkIdentity>().netId;
        Transform[] swapArray = {swapTargets[0], swapTargets[1]};
        CmdSyncSwap(swapTargets[0].GetComponent<FoodPiece>(), swapTargets[1].GetComponent<FoodPiece>(), playerID, swapArray);
        
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

        playerScrolls.AddScrollAmount(-1, 6);
        player.scrollCount += 1;
        ResetActions(true);
    }

    [Command(requiresAuthority = false)]
    public void CmdSyncSwap(FoodPiece target1, FoodPiece target2, uint connectionID, Transform[] targets)
    {
        //Add Authority
        (target1.type, target2.type) = (target2.type, target1.type);
        NetworkConnection conn = stateManager.spawnedPlayers[connectionID].GetComponent<NetworkIdentity>().connectionToClient;
        TargetUpdateFlag(conn, targets);
    }
    
    [TargetRpc]
    public void TargetUpdateFlag(NetworkConnection target, Transform[] targets)
    {
        foreach (Transform piece in targets)
        {
            Debug.Log(piece.GetComponent<FoodPiece>().type);
            foreach (Transform flag in piece)
            {
                if (flag.CompareTag("TypeFlag"))
                {
                    StartCoroutine(flag.GetComponent<SetFlagType>().SetFlag());
                }
            }
        }
    }

    [Client]
    private void Eject()
    {
        stateManager.CmdNextEject();
        ResetActions(true);
        stateManager.CmdNextPlayer();
        playerScrolls.AddScrollAmount(-1, 5);
        player.scrollCount += 1;
    }
    
    [Client]
    public void OrderDrink()
    {
        //playerAnimation.QueueAnimation("OrderDrink");
        openPopup = Instantiate(drinkMenu, new Vector3(0f, 400f, 0f), quaternion.identity);
        openPopup.GetComponent<SpawnMenu>().SlideInMenu();
        openPopup.transform.SetParent(transform, false);
        openPopup.transform.SetSiblingIndex(transform.childCount - 2);
        buttonToggle.ToggleButtons(6);
    }

    [Client]
    public void RemoveDrinkScroll()
    {
        playerScrolls.AddScrollAmount(-1, 4);
        player.scrollCount += 1;
    }
    
    [Client]
    private void ReceiveDrink()
    {
        //playerAnimation.QueueAnimation("Active");
        openPopup = Instantiate(drinkPlate, new Vector3(0f, 400f, 0f), quaternion.identity);
        openPopup.GetComponent<SpawnMenu>().SlideInMenu();
        openPopup.transform.SetParent(transform, false);
        openPopup.transform.name = "DrinkMenu";
        openPopup.transform.SetSiblingIndex(transform.childCount - 2);
        buttonToggle.ToggleButtons(5);
        player.orderVictim = false;
        ShowInfoText("Select a glass to drink from");
    }
    
    [Client]
    public void Eat()
    {
        //playerAnimation.QueueAnimation("Active");
        currentState = "Eating";
        StartAction();
        ShowInfoText("Select a piece to eat (eating will end your turn)");
    }

    [Client]
    public void Recommend()
    {
        //playerAnimation.QueueAnimation("Active");
        currentState = "Recommending";
        StartAction();
        ShowInfoText("Select a piece to recommend");
    }

    [Client]
    public void SpawnTalkMenu()
    {
        openPopup = Instantiate(talkMenu, new Vector3(0f, 400f, 0f), quaternion.identity);
        openPopup.GetComponent<SpawnMenu>().SlideInMenu();
        openPopup.transform.SetParent(transform, false);
        openPopup.transform.SetSiblingIndex(transform.childCount - 2);
        buttonToggle.ToggleButtons(6);
    }

    [Client]
    public void Encourage()
    {
        //playerAnimation.QueueAnimation("Encourage");
        stateManager.CmdNextEncourage();
        ResetActions(true);
        playerScrolls.AddScrollAmount(-1, 5);
        player.scrollCount += 1;
    }

    [Client]
    public void ShowScrollInfo(Sprite scrollImage, string scrollName, string scrollDes)
    {
        openPopup = Instantiate(scrollInfo, new Vector3(0f, 400f, 0f), quaternion.identity);

        openPopup.GetComponent<SpawnMenu>().SlideInMenu();
        openPopup.transform.SetParent(transform, false);
        openPopup.transform.SetSiblingIndex(transform.childCount - 2);
        
        openPopup.transform.Find("Icon").GetComponent<Image>().sprite = scrollImage;
        openPopup.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = ("You've got a " + scrollName + " scroll!");
        openPopup.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = scrollDes;
        buttonToggle.ToggleButtons(6);

       // StartCoroutine(HideMiniScrollInfo());
    }

    [Client]
    public void Die()
    {
        ResetActions(true);
        stateManager.CmdRemovePlayer(netId);
        openPopup = Instantiate(receipt, new Vector3(0f, 0f, 0f), quaternion.identity);
        openPopup.transform.Find("Banner2").GetComponent<TextMeshProUGUI>().text = "You Lose";
        openPopup.GetComponent<ShowStats>().LoadStats(player);
        openPopup.transform.SetParent(transform, false);
        openPopup.transform.SetSiblingIndex(transform.childCount - 2);
        buttonToggle.ToggleButtons(6);
        countTime = false;

        if (stateManager.activePlayers.Count > 1)
        {stateManager.CmdNextPlayer();}
    }

    [Client]
    private void Win()
    {
        Debug.Log("Winter Is Coming");
        ResetActions(true);
        openPopup = Instantiate(receipt, new Vector3(0f, 0f, 0f), quaternion.identity);
        openPopup.transform.Find("Banner2").GetComponent<TextMeshProUGUI>().text = "You Win";
        openPopup.GetComponent<ShowStats>().LoadStats(player);
        openPopup.transform.SetParent(transform, false);
        openPopup.transform.SetSiblingIndex(transform.childCount - 2);
        buttonToggle.ToggleButtons(6);
        countTime = false;
        PlayerPrefs.SetInt("gamesWon", PlayerPrefs.GetInt("gamesWon", 0) + 1);
    }

    /*This function doesn't work as clientRpc*/

    public void ShowChalk()
    {
        string chalkDescription = ""; 
        
        Vector3 chalkPos = new Vector3(0f, -40f, 0f);
        openPopup = Instantiate(chalkBoard, chalkPos, quaternion.identity);
        openPopup.transform.SetParent(transform, false);
        openPopup.transform.SetSiblingIndex(transform.childCount - 2);
        openPopup.GetComponent<SpawnMenu>().SlideInMenu();
        buttonToggle.ToggleButtons(6);
        
        SpawnPiece chalkData = GameObject.FindWithTag("Plate").GetComponent<SpawnPiece>();
        openPopup.transform.GetComponentInChildren<Image>().sprite = chalkData.chalkSprite;
        openPopup.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = chalkData.courseName;
        chalkDescription += "- " + chalkData.normalCount + " Empty Pieces";
        chalkDescription += "\n - " + chalkData.psnCount + " Poison Pieces";
        chalkDescription += "\n - " + chalkData.scrollCount + " Special Pieces";
        chalkDescription += "\n - " + (chalkData.normalCount +  chalkData.scrollCount + chalkData.scrollCount) +" Total Pieces";
        openPopup.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = chalkDescription;
    }

    [Client]
    public void ResetActions(bool skipScrollInfo)
    {
        currentState = "Idle";
        player.orderVictim = false;
        camActions.ZoomOut();

        if (infoText.gameObject.activeInHierarchy)
        {
            infoText.GetComponent<InfoText>().CloseInfoText();
        }

        if (openPopup != null)
        {
            if (openPopup.CompareTag("ScrollInfo") && skipScrollInfo)
            {
                openPopup.GetComponent<SpawnMenu>().SlideOutMenu();
            }
            else if (!openPopup.CompareTag("ScrollInfo"))
            {
                openPopup.GetComponent<SpawnMenu>().SlideOutMenu();
            }
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

        if (openPopup == null)
        {
            if (stateManager.currentPlayer == player.gameObject)
            {
                buttonToggle.ToggleButtons(2);
                //playerAnimation.QueueAnimation("Rest");
            }

            else
            {
                buttonToggle.ToggleButtons(4);
            }
        }

        smellTargets.Clear();
        swapTargets.Clear();
        StartCoroutine(buttonToggle.ButtonDisable(swapConfirm.transform));
        StartCoroutine(buttonToggle.ButtonDisable(smellConfirm.transform));
        StartCoroutine(buttonToggle.ButtonDisable(fakeConfirm.transform));
    }
    
    [Command(requiresAuthority = false)]
    private void DestroyPiece(GameObject piece)
    {
        //Add Authority to this function
        NetworkServer.Destroy(piece);
        StartCoroutine(mealManager.CheckNPieces());
    }
    
    public IEnumerator DespawnBillboard(GameObject billboard)
    {
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

    public IEnumerator SpawnBillboard(GameObject billboard, Transform parent)
    {
        billboard.transform.SetParent(parent);
        billboard.transform.LookAt(Camera.main.transform.position);
        Vector3 goalPos = billboard.transform.position;
        Vector3 startPos = new Vector3(goalPos.x, (goalPos.y + 1f), goalPos.z);
        billboard.transform.position = startPos;
        billboard.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        LeanTween.alpha(billboard, 1, 0.9f);
        LeanTween.move(billboard, goalPos, 0.6f);
        yield return 0;
    }

    private IEnumerator HideMiniScrollInfo()
    {
        yield return new WaitForSeconds(3f);
        openPopup.GetComponent<SpawnMenu>().SlideOutMenu();
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
            if (stateManager.currentPlayer == player.gameObject && (buttonToggle.menuMode == 4 || buttonToggle.menuMode == 6))
            {
                if (!player.actionable)
                {
                    player.actionable = true;
                    player.hasRecommended = false;
                    player.hasTalked = false;
                }

                if (!fade.gameObject.activeInHierarchy)
                {
                    buttonToggle.ToggleButtons(2);
                }
            }

            //else, set player mode as inactive
            else if (stateManager.currentPlayer != player.gameObject)
            {
                player.actionable = false;
                if (buttonToggle.menuMode != 4 && buttonToggle.menuMode != 3 && !fade.gameObject.activeInHierarchy)
                {
                    buttonToggle.ToggleButtons(4);
                }
            }
        }

        if (player != null)
        {
            if (player.health <= 0 && openPopup == null)
            {
                Die();
            }

            if (stateManager.activePlayers.Count == 1 && stateManager.gameCanEnd && player.health > 0 && openPopup == null)
            {
                Win();
            }

            if (player.actionable)
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
                    Eject();
                }

                if (Input.GetKeyDown("4"))
                {
                    currentState = "Recommending";
                }

                if (Input.GetKeyDown("c"))
                {
                    mealManager.NextCourse();
                }

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
                            ResetActions(true);
                        }
                        
                        else if (player.actionable)
                        {
                            //Where the player is eating
                            if (currentState == "Eating")
                            {

                                string foodType = piece.transform.gameObject.GetComponent<FoodPiece>().type;

                                if (foodType == "Slap")
                                {
                                    ShowScrollInfo(scrollIcons[0], foodType, scrollDesc[0]);
                                    playerScrolls.AddScrollAmount(1, 0);
                                }

                                if (foodType == "Skip")
                                {
                                    ShowScrollInfo(scrollIcons[1], foodType, scrollDesc[1]);
                                    playerScrolls.AddScrollAmount(1, 1);
                                }

                                if (foodType == "Smell")
                                {
                                    ShowScrollInfo(scrollIcons[2], foodType, scrollDesc[2]);
                                    playerScrolls.AddScrollAmount(1, 2);
                                }

                                if (foodType == "Quake")
                                {
                                    ShowScrollInfo(scrollIcons[3], foodType, scrollDesc[3]);
                                    playerScrolls.AddScrollAmount(1, 3);
                                }

                                if (foodType == "Order")
                                {
                                    ShowScrollInfo(scrollIcons[4], foodType, scrollDesc[4]);
                                    playerScrolls.AddScrollAmount(1, 4);
                                }

                                if (foodType == "Encourage")
                                {
                                    ShowScrollInfo(scrollIcons[5], "Taunt", scrollDesc[5]);
                                    playerScrolls.AddScrollAmount(1, 5);
                                }

                                if (foodType == "Swap")
                                {
                                    ShowScrollInfo(scrollIcons[6], foodType, scrollDesc[6]);
                                    playerScrolls.AddScrollAmount(1, 6);
                                }

                                if (foodType == "Fake")
                                {
                                    ShowScrollInfo(scrollIcons[7], "Decoy", scrollDesc[7]);
                                    playerScrolls.AddScrollAmount(1, 7);
                                }

                                if (foodType == "FakePoison")
                                {
                                    FakeSplash();
                                }

                                if (foodType == "Poison")
                                {
                                    Poison(true);
                                }

                                if (foodType == "Health")
                                {
                                    Health();
                                }

                                if (foodType == "Normal")
                                {
                                    mealManager.nPieces -= 1;
                                }

                                if (mealManager.nPieces <= 0)
                                {
                                    mealManager.NextCourse();
                                }

                                if (player.nPiecesEaten >= 10)
                                {
                                    Health();
                                    player.nPiecesEaten = 0;
                                }

                                DestroyPiece(piece.transform.gameObject);
                                player.pieceCount += 1;
                                currentState = "Idle";
                                ResetActions(false);
                                stateManager.CmdNextPlayer();
                            }

                            else if (currentState == "Smelling")
                            {
                                if (!smellTargets.Contains(piece.transform.gameObject))
                                {
                                    if (smellTargets.Count <= 2)
                                    {
                                        Vector3 pTrans = piece.transform.Find("FlagCenter").transform.position;
                                        smellTarget = Instantiate(typeFlag,
                                            new Vector3(pTrans.x + 0.75f, pTrans.y + 1f, pTrans.z),
                                            Quaternion.identity);
                                        StopCoroutine(DespawnBillboard(smellTarget));
                                        StartCoroutine(SpawnBillboard(smellTarget, piece.transform));
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
                                            StopCoroutine(SpawnBillboard(child.gameObject, null));
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
                                    Vector3 pTrans = piece.transform.Find("FlagCenter").transform.position;
                                    fakeTarget = Instantiate(fakeFlag,
                                        new Vector3(pTrans.x - 0.75f, pTrans.y + 1f, pTrans.z),
                                        Quaternion.identity);
                                    StartCoroutine(SpawnBillboard(fakeTarget, piece.transform));
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
                                        Vector3 pTrans = piece.transform.Find("FlagCenter").transform.position;
                                        GameObject newSwapFlag = Instantiate(swapFlag,
                                            new Vector3(pTrans.x - 0.75f, pTrans.y + 1f, pTrans.z),
                                            Quaternion.identity);
                                        StartCoroutine(SpawnBillboard(newSwapFlag, piece.transform));
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