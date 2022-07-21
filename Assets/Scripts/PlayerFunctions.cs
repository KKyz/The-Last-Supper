using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using Unity.Mathematics;
using UnityEngine.UI;

public class PlayerFunctions : NetworkBehaviour
{
    //[HideInInspector]
    public SpawnPiece plate;

    //[HideInInspector]
    public ScrollArray playerScrolls;

    //[HideInInspector]
    public PlayerManager player;

    //[HideInInspector] 
    public StateManager stateManager;

    //[HideInInspector]
    public MealManager mealManager;
    
    [HideInInspector]
    public TextMeshProUGUI infoText;

    public GameObject drinkMenu, drinkPlate, recommendFlag, typeFlag, receipt, chalkContainer, vomitSplash, scrollInfo;
    public string currentState;

    
    private EnableDisableScrollButtons buttonToggle;
    private GameObject currentRecommend, smellTarget, smellConfirm, newDrinkPlate, newReceipt, openDrinkMenu, openChalk, openInfo;
    private ShowHealth healthBar;
    private string pieceType;
    private List<GameObject> smellTargets = new List<GameObject>();

    void Start()
    {
        stateManager = GameObject.Find("StateManager").GetComponent<StateManager>();
        mealManager = GameObject.Find("StateManager").GetComponent<MealManager>();
        buttonToggle = transform.GetComponent<EnableDisableScrollButtons>();
        healthBar = transform.Find("HealthBar").GetComponent<ShowHealth>();
        smellConfirm = transform.Find("SmellConfirm").gameObject;
        infoText = transform.Find("Info").GetComponent<TextMeshProUGUI>();

        currentState = "Idle";
        player = null;
        newReceipt = null;
        chalkContainer.SetActive(false);
        smellConfirm.SetActive(false);
        smellTargets.Clear();

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
                playerScrolls = newPlayer.GetComponent<ScrollArray>();
            }
        }
    }

    [Client]
    public void ShowInfoText(string info)
    {
        infoText.gameObject.SetActive(true);
        infoText.text = info;
    }

    [Client]
    private void StartAction()
    {
        buttonToggle.ToggleButtons(3);
        //Camera.zoomIn
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
                GameObject vSplash = Instantiate(vomitSplash, transform.position, quaternion.identity);
                vSplash.transform.SetParent(transform);
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
        }
    }
    
    [Client]
    public void Quake()
    {
        plate = GameObject.FindWithTag("Plate").GetComponent<SpawnPiece>();
        plate.Shuffle();
        buttonToggle.ToggleButtons(2);
        playerScrolls.AddScrollAmount(-1, 3);
        player.scrollCount += 1;
    }
    
    [Client]
    public void Slap()
    {
        ResetActions();
        stateManager.CmdNextPlayer();
        playerScrolls.AddScrollAmount(-1, 0);
        player.scrollCount += 1;
    }
    
    [Client]
    public void Skip()
    {
        player.orderVictim = false;
        ResetActions();
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
        foreach (GameObject piece in smellTargets)
        {
            pieceType = piece.transform.GetComponent<FoodPiece>().type;

            foreach (Transform flag in piece.transform)
            {
                if (flag.CompareTag("TypeFlag"))
                {
                    StartCoroutine(flag.GetComponent<SetFlagType>().SetFlag(pieceType));
                }
            }
        }

        smellTargets.Clear();
        smellConfirm.SetActive(false);
        playerScrolls.AddScrollAmount(-1, 2);
        player.scrollCount += 1;
        currentState = "Idle";
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

    [Client]
    public void Eject()
    {
        stateManager.CmdNextEject();
        ResetActions();
        stateManager.CmdNextPlayer();
        playerScrolls.AddScrollAmount(-1, 5);
        player.scrollCount += 1;
    }
    
    [Client]
    public void OrderDrink()
    {
        openDrinkMenu = Instantiate(drinkMenu, transform.position, Quaternion.identity);
        openDrinkMenu.GetComponent<SpawnMenu>().SlideInMenu();
        openDrinkMenu.transform.SetParent(transform);
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
        newDrinkPlate = Instantiate(drinkPlate, transform.position, Quaternion.identity);
        newDrinkPlate.GetComponent<SpawnMenu>().SlideInMenu();
        newDrinkPlate.transform.SetParent(transform);
        buttonToggle.ToggleButtons(5);
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
    public void Encourage()
    {
        stateManager.CmdNextEncourage();
        ResetActions();
        stateManager.CmdNextPlayer();
        playerScrolls.AddScrollAmount(-1, 5);
        player.scrollCount += 1;
    }

    [Client]
    public void DisplayScrollInfo(Sprite scrollImage, string scrollName, string scrollDesc)
    {
        if (openInfo == null)
        {
            openInfo = Instantiate(scrollInfo, transform.position, Quaternion.identity);
            openInfo.transform.SetParent(transform);
            
            openInfo.transform.Find("ScrollIcon").GetComponent<Image>().sprite = scrollImage;
            openInfo.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = scrollName;
            openInfo.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = scrollDesc;
        }
    }

    [Client]
    public void Die()
    {
        ResetActions();
        buttonToggle.ToggleButtons(6);
        stateManager.CmdRemoveActivePlayer();
        newReceipt = Instantiate(receipt, transform.position, Quaternion.identity);
        newReceipt.transform.Find("Banner2").GetComponent<TextMeshProUGUI>().text = "You Lose";
        newReceipt.GetComponent<ShowStats>().LoadStats(player);
        newReceipt.transform.SetParent(transform);
        
        if (stateManager.activePlayers > 1)
        {stateManager.CmdNextPlayer();}
    }

    [Client]
    private void Win()
    {
        ResetActions();
        buttonToggle.ToggleButtons(6);
        newReceipt = Instantiate(receipt, transform.position, Quaternion.identity);
        newReceipt.transform.Find("Banner2").GetComponent<TextMeshProUGUI>().text = "You Win";
        newReceipt.GetComponent<ShowStats>().LoadStats(player);
        newReceipt.transform.SetParent(transform);
    }

    /* This function doesn't work as clientRpc*/
    
    public void ShowChalk(Sprite chalkBoard)
    {
        openChalk = Instantiate(chalkContainer, transform.position, Quaternion.identity);
        openChalk.transform.SetParent(transform);
        openChalk.SetActive(true);
        openChalk.transform.Find("Board").GetComponent<Image>().sprite = chalkBoard;
        chalkContainer.GetComponent<SpawnMenu>().SlideInMenu();
    }

    [Client]
    public void ResetActions()
    {
        currentState = "Idle";
        player.orderVictim = false;
        buttonToggle.ToggleButtons(2);
        //Camera.ZoomOut()

        if (infoText.gameObject.activeInHierarchy)
        {
            infoText.GetComponent<InfoText>().CloseInfoText();
        }

        if (openDrinkMenu != null)
        {
            openDrinkMenu.GetComponent<SpawnMenu>().SlideOutMenu();
        }

        if (newDrinkPlate != null)
        {
            newDrinkPlate.GetComponent<SpawnMenu>().SlideOutMenu();
        }

        if (openInfo != null)
        {
            openInfo.GetComponent<SpawnMenu>().SlideOutMenu();
        }

        foreach (GameObject targetPiece in smellTargets)
        {
            foreach (Transform flag in targetPiece.transform)
            {
                if (flag.CompareTag("TypeFlag"))
                {
                    Destroy(flag.gameObject);
                    NetworkServer.Destroy(flag.gameObject);
                }
            }
        }

        smellTargets.Clear();
        if (smellConfirm.activeInHierarchy)
        {
            smellConfirm.SetActive(false);
        }
    }
    
    private IEnumerator DespawnBillboard(GameObject billboard)
    {
        Vector3 startPos = billboard.transform.position;
        Vector3 goalPos = new Vector3(startPos.x, (startPos.y + 1f), startPos.z);
        LeanTween.alpha(billboard, 0, 0.4f);
        LeanTween.move(billboard, goalPos, 0.6f);
        yield return new WaitForSeconds(0.6f);
        NetworkServer.Destroy(billboard);
    }

    private IEnumerator SpawnBillboard(GameObject billboard)
    {
        billboard.transform.LookAt(Camera.main.transform.position);
        Vector3 goalPos = billboard.transform.position;
        Vector3 startPos = new Vector3(goalPos.x, (goalPos.y + 1f), goalPos.z);
        billboard.transform.position = startPos;
        billboard.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        LeanTween.alpha(billboard, 1, 0.9f);
        LeanTween.move(billboard, goalPos, 0.6f);
        yield return 0;
    }

    /* The next set of functions: RpcSpawnBillboard, RpcDespawnBillboard,  CreateRecommend, RemoveRecommend
     are all created to manage recommend flags, but they don't work correctly on server side*/
    
    [ClientRpc]
    public void RpcSpawnBillboard(GameObject billboard)
    {
        //Use this method to call SpawnBillboard on ALL clients rather than just locally
        StartCoroutine(SpawnBillboard(billboard));
    }

    [Command(requiresAuthority = false)]
    public void RpcDespawnBillboard(GameObject billboard)
    {
        //Use this method to call SpawnBillboard on ALL clients rather than just locally
        StartCoroutine(DespawnBillboard(billboard));
    }

    [Command(requiresAuthority = false)]
    private void CmdCreateRecommend(Transform piece)
    {
        //Add Authority to this function
        //Spawns piece, assigns flag to piece, creates transition
        //This function doesn't set the same position for flag in client and server for some reason
        NetworkServer.Spawn(currentRecommend);
        currentRecommend.transform.SetParent(piece, true);
        piece.GetComponent<FoodPiece>().recommendFlag = currentRecommend;
        RpcSpawnBillboard(currentRecommend);
    }

    [Command(requiresAuthority = false)]
    private void CmdRemoveRecommend(Transform piece)
    {
        //Add Authority to this function
        //DespawnsPiece, strips piece's assigned flag
        RpcDespawnBillboard(piece.transform.GetComponent<FoodPiece>().recommendFlag);
        piece.transform.GetComponent<FoodPiece>().recommendFlag = null;
    }

    [Command(requiresAuthority = false)]
    private void DestroyPiece(GameObject piece)
    {
        //Add Authority to this function
        if (piece.GetComponent<FoodPiece>().recommendFlag != null)
        {
            NetworkServer.Destroy(piece.GetComponent<FoodPiece>().recommendFlag);
        }

        NetworkServer.Destroy(piece);
        StartCoroutine(mealManager.CheckNPieces());
    }

    void Update()
    {
        //Manages player options when playing
        if (player != null && stateManager.currentPlayer == player.gameObject && !player.actionable && buttonToggle.menuMode != 2 && openChalk == null)
        {
            player.actionable = true;
            buttonToggle.ToggleButtons(2);
        }

        else if (openChalk != null && buttonToggle.menuMode != 6)
        {
            player.actionable = false;
            buttonToggle.ToggleButtons(6);
        }

        else if (player != null && stateManager.currentPlayer != player.gameObject && buttonToggle.menuMode != 4 && newReceipt == null)
        {
            player.actionable = false;
            player.hasRecommended = false;
            buttonToggle.ToggleButtons(4);
        }

        if (player != null)
        {
            if (player.health <= 0 && newReceipt == null)
            {
                Die();
            }

            if (stateManager.activePlayers == 1 && newReceipt == null && stateManager.gameCanEnd && player.health > 0)
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
                    FakePoison();
                }

                if (Input.GetKeyDown("4"))
                {
                    Swap();
                }

                if (Input.GetKeyDown("5"))
                {
                    Eject();
                }

                if (Input.GetKeyDown("6"))
                {
                    OrderDrink();
                }

                if (Input.GetKeyDown("7"))
                {
                    currentState = "Recommending";
                    Debug.Log("Ready to recommend");
                }

                if (Input.GetKeyDown("8"))
                {
                    Smell();
                }

                if (player.orderVictim && newDrinkPlate == null)
                {
                    ReceiveDrink();
                }

                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit piece;
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out piece))
                    {
                        if (piece.transform.CompareTag("FoodPiece"))
                        {
                            //Where the player is eating
                            if (currentState == "Eating")
                            {

                                string foodType = piece.transform.gameObject.GetComponent<FoodPiece>().type;
                                Debug.Log(foodType);

                                player.piecesEaten += 1;

                                if (foodType == "Slap")
                                {
                                    playerScrolls.AddScrollAmount(1, 0);
                                }

                                if (foodType == "Skip")
                                {
                                    playerScrolls.AddScrollAmount(1, 1);
                                }

                                if (foodType == "Smell")
                                {
                                    playerScrolls.AddScrollAmount(1, 2);
                                }

                                if (foodType == "Quake")
                                {
                                    playerScrolls.AddScrollAmount(1, 3);
                                }

                                if (foodType == "Order")
                                {
                                    playerScrolls.AddScrollAmount(1, 4);
                                }

                                if (foodType == "Encourage")
                                {
                                    playerScrolls.AddScrollAmount(1, 5);
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
                                    plate = GameObject.FindWithTag("Plate").GetComponent<SpawnPiece>();
                                }

                                if (player.piecesEaten >= 10)
                                {
                                    Health();
                                    player.piecesEaten = 0;
                                }

                                DestroyPiece(piece.transform.gameObject);
                                player.pieceCount += 1;
                                currentState = "Idle";
                                ResetActions();
                                stateManager.CmdNextPlayer();
                            }

                            else if (currentState == "Recommending")
                            {
                                if (player.recommendedPiece == null)
                                {
                                    //If the piece doesn't have any flags already, create one
                                    if (piece.transform.GetComponent<FoodPiece>().recommendFlag == null)
                                    {
                                        currentRecommend = Instantiate(recommendFlag,
                                            new Vector3(piece.transform.position.x + 0.75f,
                                                piece.transform.position.y + 1f, piece.transform.position.z),
                                            Quaternion.identity);
                                        CmdCreateRecommend(piece.transform);
                                        player.recommendedPiece = piece.transform.gameObject;
                                    }

                                }

                                //In case of deleting a flag
                                else
                                {
                                    //Just destroy flag
                                    if (piece.transform.gameObject == player.recommendedPiece)
                                    {
                                        CmdRemoveRecommend(piece.transform);
                                        player.recommendedPiece = null;
                                    }

                                    //Replace flag with a new one at a different piece
                                    else
                                    {
                                        CmdRemoveRecommend(piece.transform);
                                        currentRecommend = Instantiate(recommendFlag,
                                            new Vector3(piece.transform.position.x + 0.75f,
                                                piece.transform.position.y + 1f, piece.transform.position.z),
                                            Quaternion.identity);
                                        CmdCreateRecommend(piece.transform);
                                        player.recommendedPiece = piece.transform.gameObject;
                                    }
                                }

                                currentState = "Idle";
                                player.hasRecommended = true;
                                ResetActions();
                            }

                            else if (currentState == "Smelling")
                            {
                                if (!smellTargets.Contains(piece.transform.gameObject))
                                {
                                    if (smellTargets.Count <= 2)
                                    {
                                        smellTarget = Instantiate(typeFlag,
                                            new Vector3(piece.transform.position.x + 0.75f,
                                                piece.transform.position.y + 1f, piece.transform.position.z),
                                            Quaternion.identity);
                                        StopCoroutine(DespawnBillboard(smellTarget));
                                        StartCoroutine(SpawnBillboard(smellTarget));
                                        smellTarget.transform.LookAt(Camera.main.transform.position);
                                        smellTarget.transform.SetParent(piece.transform);
                                        smellTargets.Add(piece.transform.gameObject);
                                    }

                                    if (smellTargets.Count >= 1)
                                    {
                                        smellConfirm.SetActive(true);
                                    }
                                }

                                else
                                {
                                    foreach (Transform child in piece.transform)
                                    {
                                        if (child.CompareTag("TypeFlag") && child.gameObject.name != "PlantedFlag")
                                        {
                                            StopCoroutine(SpawnBillboard(child.gameObject));
                                            StartCoroutine(DespawnBillboard(child.gameObject));
                                        }
                                    }

                                    smellTargets.Remove(piece.transform.gameObject);

                                    if (smellTargets.Count <= 0)
                                    {
                                        smellConfirm.SetActive(false);
                                    }
                                }
                            }

                            else if (currentState == "Poisoning")
                            {
                                piece.transform.GetComponent<FoodPiece>().FakePsn();
                                currentState = "Idle";
                                ResetActions();
                            }

                            else if (currentState == "Swapping")
                            {
                                //string stringHold;
                                //string piece1Type = null;
                                //string piece2Type = null;
                                //
                                // if (piece1Type == null)
                                // {
                                //     piece1Type = piece.transform.GetComponent<FoodPiece>().type;
                                // }
                                //
                                // else if (piece2Type == null)
                                // {
                                //     piece2Type = piece.transform.GetComponent<FoodPiece>().type;
                                // }
                                //
                                // while (piece1Type != null || piece2Type != null)
                                // {
                                //     if (piece1Type == null)
                                //     {
                                //         piece1Type = piece.transform;
                                //     }
                                // }
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