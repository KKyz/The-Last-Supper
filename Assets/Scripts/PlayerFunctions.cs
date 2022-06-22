using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class PlayerFunctions : NetworkBehaviour
{
    //[HideInInspector]
    public SpawnPiece plate;

    //[HideInInspector]
    public ScrollArray playerScrolls;

    //[HideInInspector]
    public PlayerManager player;

    //[HideInInspector]
    public GameManager gameManager;

    //[HideInInspector] 
    public StateManager stateManager;
    
    //[HideInInspector]
    public MealManager mealManager;

    public GameObject drinkMenu, drinkPlate, recommendFlag, typeFlag, receipt;

    private EnableDisableScrollButtons buttonToggle;
    public string currentState;
    private GameObject currentRecommend, smellTarget, smellConfirm, newDrinkPlate, newReceipt, openDrinkMenu;
    private ShowHealth healthBar;
    private string pieceType;
    private List<GameObject> smellTargets = new List<GameObject>();

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        stateManager = GameObject.Find("StateManager").GetComponent<StateManager>();
        mealManager = GameObject.Find("StateManager").GetComponent<MealManager>();
        buttonToggle = transform.GetComponent<EnableDisableScrollButtons>();
        healthBar = GameObject.Find("HealthBar").GetComponent<ShowHealth>();
        smellConfirm = transform.GetChild(2).gameObject;

        currentState = "Idle";
        player = null;
        newReceipt = null;
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
                player = newPlayer.GetComponent<PlayerManager>();
                playerScrolls = newPlayer.GetComponent<ScrollArray>();
            }
        }
    }
    
    //[Command]
    public void CmdStartAction()
    {
        buttonToggle.ToggleButtons(3);
    }

    //[Command]
    public void CmdCancelAction()
    {
        buttonToggle.ToggleButtons(2);
    }

    //[Command]
    public void CmdPoison()
    {
        if (player.health >= 1){player.health -= 1; healthBar.SetHealth(player.health);}
    }

    //[Command]
    public void CmdHealth()
    {
        if (player.health < 2){player.health += 1; healthBar.SetHealth(player.health);}
    }

    //[Command]
    public void CmdQuake()
    {
        plate = GameObject.FindWithTag("Plate").GetComponent<SpawnPiece>();
        plate.Shuffle();
        playerScrolls.AddScrollAmount(-1, 3);
        player.scrollCount += 1;
    }

    //[Command]
    public void CmdSlap()
    {
        RpcResetActions();
        stateManager.SetCurrentPlayer();
        playerScrolls.AddScrollAmount(-1, 0);
        player.scrollCount += 1;
    }

    //[Command]
    public void CmdSkip()
    {
        player.orderVictim = false;
        RpcResetActions();
        stateManager.SetCurrentPlayer();
        playerScrolls.AddScrollAmount(-1, 1);
        player.scrollCount += 1;
    }

    //[Command]
    public void CmdSmell()
    {
        currentState = "Smelling";
        CmdStartAction();
    }

    //[Command]
    public void CmdConfirmSmell()
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
        CmdCancelAction();
    }

    public void CmdFakePoison()
    {
        currentState = "Poisoning";
        CmdStartAction();
    }

    public void CmdSwap()
    {
        currentState = "Swapping";
        CmdStartAction();
    }

    public void CmdEject()
    {
        stateManager.NextEject();
        playerScrolls.AddScrollAmount(-1, 5);
        player.scrollCount += 1;
    }

    
    public void CmdOrderDrink()
    {
        openDrinkMenu = Instantiate(drinkMenu, transform.position, Quaternion.identity);
        openDrinkMenu.GetComponent<SpawnMenu>().SlideInMenu();
        openDrinkMenu.transform.SetParent(transform);
        buttonToggle.ToggleButtons(6);
        playerScrolls.AddScrollAmount(-1, 4);
        player.scrollCount += 1;
    }

    
    private void CmdReceiveDrink()
    {
        newDrinkPlate = Instantiate(drinkPlate, transform.position, Quaternion.identity);
        newDrinkPlate.GetComponent<SpawnMenu>().SlideInMenu();
        newDrinkPlate.transform.SetParent(transform);
        buttonToggle.ToggleButtons(5);
    }

    //[Command]
    public void CmdEat()
    {
        currentState = "Eating";
        CmdStartAction();
    }

    //[Command]
    public void CmdRecommend()
    {
        currentState = "Recommending";
        CmdStartAction();
    }

    //[Command]
    public void CmdEncourage()
    {
        stateManager.NextEncourage();
        playerScrolls.AddScrollAmount(-1, 5);
        player.scrollCount += 1;
    }

    public void CmdDie()
    {
        RpcResetActions();
        buttonToggle.ToggleButtons(6);
        stateManager.removeActivePlayer();
        newReceipt = Instantiate(receipt, transform.position, Quaternion.identity);
        newReceipt.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "You Lose";
        newReceipt.GetComponent<ShowStats>().LoadStats(player);
        newReceipt.transform.SetParent(transform);
    }

    private void CmdWin()
    {
        RpcResetActions();
        buttonToggle.ToggleButtons(5);
        newReceipt = Instantiate(receipt, transform.position, Quaternion.identity);
        newReceipt.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "You Win";
        newReceipt.GetComponent<ShowStats>().LoadStats(player);
        newReceipt.transform.SetParent(transform);
    }
    
    public void RpcResetActions()
    {
        currentState = "Idle";
        player.orderVictim = false;
        CmdCancelAction();

        if (openDrinkMenu != null){openDrinkMenu.GetComponent<SpawnMenu>().SlideOutMenu();}
        if (newDrinkPlate != null){newDrinkPlate.GetComponent<SpawnMenu>().SlideOutMenu();}

        foreach (GameObject targetPiece in smellTargets)
        {
            foreach(Transform flag in targetPiece.transform)
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
        {smellConfirm.SetActive(false);}
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
        Vector3 goalPos = billboard.transform.position;
        Vector3 startPos = new Vector3(goalPos.x, (goalPos.y + 1f), goalPos.z);
        billboard.transform.position = startPos;
        billboard.GetComponent<SpriteRenderer>().color = new Color (1, 1, 1, 0);
        LeanTween.alpha(billboard, 1, 0.9f);
        LeanTween.move(billboard, goalPos, 0.6f);
        yield return 0;
    }
    
    [Command(requiresAuthority = false)]
    private void CmdCreateRecommend(GameObject flag)
    {
        NetworkServer.Spawn(flag);
        StartCoroutine(SpawnBillboard(flag));
    }

    [Command(requiresAuthority = false)]
    private void DestroyPiece(GameObject piece)
    {
        NetworkServer.Destroy(piece);
        StartCoroutine(mealManager.CheckNPieces());
    }

    void Update()
    {
        if (player != null && stateManager.currentPlayer == player.gameObject && !player.actionable && buttonToggle.menuMode != 2)
        {
            player.actionable = true;
            buttonToggle.ToggleButtons(2);
        }
        else if(player != null && stateManager.currentPlayer != player.gameObject && buttonToggle.menuMode != 4)
        {
            player.actionable = false;
            player.hasRecommended = false;
            buttonToggle.ToggleButtons(4);
        }

        if (player != null && player.actionable)
        {

            if (Input.GetKeyDown("1")){CmdPoison();}

            if (Input.GetKeyDown("2")){CmdQuake();}

            if (Input.GetKeyDown("3")){CmdSlap();}

            if (Input.GetKeyDown("4")){CmdHealth();}

            if (Input.GetKeyDown("5")){CmdOrderDrink();}

            if (Input.GetKeyDown("6")){CmdEat();}

            if (Input.GetKeyDown("7")){currentState = "Recommending"; Debug.Log("Ready to recommend");}
            
            if (Input.GetKeyDown("8")){CmdSmell(); Debug.Log("Ready to smell");}

            if (Input.GetKeyDown("9")){CmdEncourage(); Debug.Log("EncouragedNext");}
            
            if (Input.GetKeyDown("0")){CmdDie();}

            if (player.health <= 0 && newReceipt == null)
            {CmdDie();}
            
            if(stateManager.activePlayers == 1 && newReceipt == null && stateManager.gameCanEnd && player.health > 0)
            {CmdWin();}

            if (player.orderVictim && newDrinkPlate == null)
            {CmdReceiveDrink();}

            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit  piece;
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

                            if (foodType == "Poison")
                            {
                                mealManager.pPieces -= 1;
                            }

                            if (foodType == "Normal")
                            {
                                mealManager.nPieces -= 1;
                            }
                            
                            if (foodType == "Slap"){playerScrolls.AddScrollAmount(1, 0);}

                            if (foodType == "Skip"){playerScrolls.AddScrollAmount(1, 1);}

                            if (foodType == "Smell"){playerScrolls.AddScrollAmount(1, 2);}

                            if (foodType == "Quake"){playerScrolls.AddScrollAmount(1, 3);}

                            if (foodType == "Order"){playerScrolls.AddScrollAmount(1, 4);}
                            
                            if (foodType == "Encourage"){playerScrolls.AddScrollAmount(1, 5);}

                            if (foodType == "Poison"){CmdPoison();}

                            if (foodType == "Health"){CmdHealth();}

                            if (mealManager.nPieces <= 0)
                            {
                                mealManager.NextCourse();
                                plate = GameObject.FindWithTag("Plate").GetComponent<SpawnPiece>();
                            }

                            if (player.piecesEaten >= 10)
                            {
                                CmdHealth();
                                player.piecesEaten = 0;
                            }
                            
                            DestroyPiece(piece.transform.gameObject);
                            player.pieceCount += 1;
                            currentState = "Idle";
                            CmdCancelAction();
                            RpcResetActions();
                            stateManager.SetCurrentPlayer();
                        }

                        else if (currentState == "Recommending")
                        {
                            if (player.recommendedPiece == null)
                            {
                                //If the piece doesn't have any flags already, create one
                                if (piece.transform.childCount == 0)
                                {
                                    currentRecommend = Instantiate(recommendFlag, new Vector3(piece.transform.position.x + 0.75f, piece.transform.position.y + 1f, piece.transform.position.z), Quaternion.identity);
                                    CmdCreateRecommend(currentRecommend);
                                    currentRecommend.transform.LookAt(Camera.main.transform.position);
                                    currentRecommend.transform.SetParent(piece.transform);
                                    player.recommendedPiece = piece.transform.gameObject;
                                }

                            }

                            //In case of deleting a flag
                            else
                            {
                                //Just destroy flag
                                if (piece.transform.gameObject == player.recommendedPiece)
                                {
                                    StartCoroutine(DespawnBillboard(currentRecommend));
                                    player.recommendedPiece = null;
                                }

                                //Replace flag with a new one at a different piece
                                else
                                {
                                    StartCoroutine(DespawnBillboard(currentRecommend));
                                    currentRecommend = Instantiate(recommendFlag, new Vector3(piece.transform.position.x + 0.75f, piece.transform.position.y + 1f, piece.transform.position.z), Quaternion.identity);
                                    CmdCreateRecommend(currentRecommend);
                                    currentRecommend.transform.LookAt(Camera.main.transform.position);
                                    currentRecommend.transform.SetParent(piece.transform);
                                    player.recommendedPiece = piece.transform.gameObject;
                                }
                            }
                            
                            currentState = "Idle";
                            player.hasRecommended = true;
                            CmdCancelAction();
                        }

                        else if (currentState == "Smelling")
                        {
                            if (!smellTargets.Contains(piece.transform.gameObject))
                            {
                                if (smellTargets.Count <= 2)
                                {
                                    smellTarget = Instantiate(typeFlag, new Vector3(piece.transform.position.x + 0.75f, piece.transform.position.y + 1f, piece.transform.position.z), Quaternion.identity);
                                    StopCoroutine(DespawnBillboard(smellTarget));
                                    StartCoroutine(SpawnBillboard(smellTarget));
                                    smellTarget.transform.LookAt(Camera.main.transform.position);
                                    smellTarget.transform.SetParent(piece.transform);
                                    smellTargets.Add(piece.transform.gameObject);
                                }

                                if (smellTargets.Count >= 3)
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
                                smellConfirm.SetActive(false);
                            }
                        }

                        else if (currentState == "Poisoning")
                        {
                            piece.transform.GetComponent<FoodPiece>().FakePsn();
                        }

                        else if (currentState == "Swapping")
                        {
                            Transform tempTransform;
                            Transform piece1 = null;
                            Transform piece2 = null;

                            if (piece1 == null && piece2 == null)
                            {
                                piece1 = piece.transform;
                            }

                            else if (piece1 != null && piece2 == null)
                            {
                                piece2 = piece.transform;
                            }

                            else if (piece.transform == piece1)
                            {
                                piece1 = null;
                            }

                            else if (piece.transform == piece2)
                            {
                                piece2 = null;
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