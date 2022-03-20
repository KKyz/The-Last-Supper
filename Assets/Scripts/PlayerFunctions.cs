using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerFunctions : NetworkBehaviour
{
    [HideInInspector]
    public SpawnPiece plate;

    [HideInInspector]
    public ScrollArray playerScrolls;

    [HideInInspector]
    public PlayerManager player;

    [HideInInspector]
    public Camera playerCam;

    [HideInInspector]
    public GameManager gameManager;

    public GameObject drinkMenu, drinkPlate, recommendFlag, typeFlag;

    private EnableDisableScrollButtons buttonToggle;
    private bool isEating, isRecommending, isSmelling;
    private GameObject currentRecommend, smellTarget, smellConfirm, newDrinkPlate, openDrinkMenu;
    private ShowHealth healthBar;
    private string pieceType;
    private List<GameObject> smellTargets = new List<GameObject>();

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        buttonToggle = transform.GetComponent<EnableDisableScrollButtons>();
        healthBar = GameObject.Find("HealthBar").GetComponent<ShowHealth>();
        smellConfirm = transform.GetChild(2).gameObject;

        isEating = false;
        isRecommending = false;
        player = null;
        playerCam = null;
        isSmelling = false;
        smellConfirm.SetActive(false);
        smellTargets.Clear();
        buttonToggle.ToggleButtons(4);
        StartCoroutine(PostStartCall());
    }

    IEnumerator PostStartCall()
    {
        yield return new WaitForEndOfFrame();
        plate = GameObject.FindWithTag("Plate").GetComponent<SpawnPiece>();
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
        plate.Shuffle();
        playerScrolls.AddScrollAmount(-1, 3);
    }

    //[Command]
    public void CmdSlap()
    {
        gameManager.NextPlayer();
        playerScrolls.AddScrollAmount(-1, 0);
    }

    //[Command]
    public void CmdSkip()
    {
        player.orderVictim = false;
        gameManager.NextPlayer();
        playerScrolls.AddScrollAmount(-1, 1);
    }

    //[Command]
    public void CmdSmell()
    {
        isRecommending = false;
        isSmelling = true;
        isEating = false;
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
                if (flag.tag == "TypeFlag")
                {
                    flag.GetComponent<SetFlagType>().SetFlag(pieceType);
                }
            }
        }

        smellTargets.Clear();
        smellConfirm.SetActive(false);
        playerScrolls.AddScrollAmount(-1, 2);
        isSmelling = false;
        CmdCancelAction();
    }

    //[Command]
    public void CmdOrderDrink()
    {
        openDrinkMenu = Instantiate(drinkMenu, transform.position, Quaternion.identity);
        openDrinkMenu.transform.SetParent(transform);
        buttonToggle.ToggleButtons(6);
        playerScrolls.AddScrollAmount(-1, 4);
    }

    //[Command]
    public void CmdEat()
    {
        isEating = true;
        isRecommending = false;
        isSmelling = false;
        CmdStartAction();
    }

    //[Command]
    public void CmdRecommend()
    {
        isRecommending = true;
        isSmelling = false;
        isEating = false;
        CmdStartAction();
    }

    //[Command]
    public void CmdEncourage()
    {
        gameManager.NextEncourage();
        playerScrolls.AddScrollAmount(-1, 5);
    }

    //[ClientRpc]
    public void RpcActionToggle(bool toggle)
    {
        player.actionable = toggle;

        if (toggle)
        {
            buttonToggle.ToggleButtons(2);
            player.hasRecommended = false;
        }

        else
        {
            buttonToggle.ToggleButtons(4);
        }
    }

    //[ClientRpc]
    public void RpcResetActions(bool deactivate)
    {
        isEating = false;
        isRecommending = false;
        isSmelling = false;
        player.orderVictim = false;
        CmdCancelAction();

        if (openDrinkMenu != null){Destroy(openDrinkMenu); NetworkServer.Destroy(openDrinkMenu);}
        if (newDrinkPlate != null){Destroy(newDrinkPlate); NetworkServer.Destroy(newDrinkPlate);}

        foreach (GameObject targetPiece in smellTargets)
        {
            foreach(Transform flag in targetPiece.transform)
            {
                if (flag.tag == "TypeFlag")
                {
                    Destroy(flag.gameObject);
                    NetworkServer.Destroy(flag.gameObject);
                }
            }
        }

        smellTargets.Clear();
        smellConfirm.SetActive(false);

        if (deactivate){player.actionable = false;}
    }

    //[ClientRpc]
    public void RpcSync(GameObject newPlayer)
    {
        player = newPlayer.GetComponent<PlayerManager>();
        playerScrolls = newPlayer.GetComponent<ScrollArray>();
        buttonToggle.playerScrollArray = playerScrolls;
        buttonToggle.playerManager = player;
        playerCam = newPlayer.transform.GetChild(0).GetComponent<Camera>();
        healthBar.SetHealth(newPlayer.GetComponent<PlayerManager>().health);
    }

    void Update()
    {
        if (player != null && player.actionable)
        {

            if (Input.GetKeyDown("1")){CmdPoison();}

            if (Input.GetKeyDown("2")){CmdQuake();}

            if (Input.GetKeyDown("3")){CmdSlap();}

            if (Input.GetKeyDown("4")){CmdHealth();}

            if (Input.GetKeyDown("5")){CmdOrderDrink();}

            if (Input.GetKeyDown("6")){CmdEat();}

            if (Input.GetKeyDown("7")){isRecommending = true; Debug.Log("Ready to recommend");}
            
            if (Input.GetKeyDown("8")){CmdSmell(); Debug.Log("Ready to smell");}

            if (Input.GetKeyDown("9")){CmdEncourage(); Debug.Log("EncouragedNext");}

            if (player.orderVictim && newDrinkPlate == null)
            {
                newDrinkPlate = Instantiate(drinkPlate, transform.position, Quaternion.identity);
                newDrinkPlate.transform.SetParent(transform);
                buttonToggle.ToggleButtons(5);
            }

            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit  piece;
                var ray = playerCam.ScreenPointToRay(Input.mousePosition);
             
                if (Physics.Raycast(ray, out piece))
                {
                    if (piece.transform.tag == "FoodPiece")
                    {
                        //WHere the player is eating
                        if (isEating)
                        {
                
                            string foodType = piece.transform.gameObject.GetComponent<FoodPiece>().type;
                            Debug.Log(foodType);
                            
                            if (foodType == "Normal"){gameManager.nPieces -= 1;}
                            
                            if (foodType == "Slap"){playerScrolls.AddScrollAmount(1, 0);}

                            if (foodType == "Skip"){playerScrolls.AddScrollAmount(1, 1);}

                            if (foodType == "Smell"){playerScrolls.AddScrollAmount(1, 2);}

                            if (foodType == "Quake"){playerScrolls.AddScrollAmount(1, 3);}

                            if (foodType == "Order"){playerScrolls.AddScrollAmount(1, 4);}
                            
                            if (foodType == "Encourage"){playerScrolls.AddScrollAmount(1, 5);}

                            if (foodType == "Poison"){CmdPoison();}

                            if (foodType == "Health"){CmdHealth();}

                            Destroy(piece.transform.gameObject);
                            NetworkServer.Destroy(piece.transform.gameObject);
                            isEating = false;
                            CmdCancelAction();
                            gameManager.NextPlayer();   
                        }

                        if (isRecommending)
                        {
                            if (player.recommendedPiece == null)
                            {
                                //If the piece doesn't have any flags already, create one
                                if (piece.transform.childCount == 0)
                                {
                                    currentRecommend = Instantiate(recommendFlag, new Vector3(piece.transform.position.x + 0.75f, piece.transform.position.y + 1f, piece.transform.position.z), Quaternion.identity);
                                    NetworkServer.Spawn(currentRecommend);
                                    currentRecommend.transform.LookAt(playerCam.transform.position);
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
                                    Destroy(currentRecommend);
                                    NetworkServer.Destroy(currentRecommend);
                                    player.recommendedPiece = null;
                                }

                                //Replace flag with a new one at a different piece
                                else
                                {
                                    Destroy(currentRecommend);
                                    NetworkServer.Destroy(currentRecommend);
                                    NetworkServer.Destroy(piece.transform.gameObject);
                                    currentRecommend = Instantiate(recommendFlag, new Vector3(piece.transform.position.x + 0.75f, piece.transform.position.y + 1f, piece.transform.position.z), Quaternion.identity);
                                    NetworkServer.Spawn(currentRecommend);
                                    currentRecommend.transform.LookAt(playerCam.transform.position);
                                    currentRecommend.transform.SetParent(piece.transform);
                                    player.recommendedPiece = piece.transform.gameObject;
                                }
                            }
                            
                            isRecommending = false;
                            player.hasRecommended = true;
                            CmdCancelAction();
                        }

                        if (isSmelling)
                        {
                            if (!smellTargets.Contains(piece.transform.gameObject))
                            {
                                if (smellTargets.Count <= 2)
                                {
                                    smellTarget = Instantiate(typeFlag, new Vector3(piece.transform.position.x + 0.75f, piece.transform.position.y + 1f, piece.transform.position.z), Quaternion.identity);
                                    smellTarget.transform.LookAt(playerCam.transform.position);
                                    smellTarget.transform.SetParent(piece.transform);
                                    smellTargets.Add(piece.transform.gameObject);
                                }
                                if (smellTargets.Count >= 3)
                                {smellConfirm.SetActive(true);}
                            }

                            else
                            {
                                foreach (Transform child in piece.transform)
                                {
                                    if (child.gameObject.tag == "TypeFlag" && child.gameObject.name != "PlantedFlag")
                                    {
                                        Destroy(child.gameObject);
                                        NetworkServer.Destroy(child.gameObject);
                                    }
                                }

                                smellTargets.Remove(piece.transform.gameObject);
                                smellConfirm.SetActive(false);
                            }
                        }
                    }
                }
            }
        }
    }
}