using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [HideInInspector]
    public SpawnPiece plate;

    [HideInInspector]
    public GameManager gameManager;

    [HideInInspector]
    public ScrollArray playerScrolls;

    [HideInInspector]
    public SetFlagType smellFlagType;

    public string playerName;
    public int playerHealth;
    public GameObject drinkMenu, recommendFlag, typeFlag;
    public bool actionable;
    public bool isEating;
    public bool isRecommending;
    public bool isSmelling;

    private GameObject currentRecommend, smellTarget, recommendedPiece;
    private Transform playerCanvas, cancelButton;
    private List<GameObject> smellTargets = new List<GameObject>();
    private string pieceType;

    void Start()
    {
        playerHealth = 2;
        plate = GameObject.FindWithTag("Plate").GetComponent<SpawnPiece>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerCanvas = GameObject.Find("PlayerCanvas").transform;
        cancelButton = playerCanvas.Find("CancelButton").transform;
        cancelButton.gameObject.SetActive(false);
        playerScrolls = gameObject.GetComponent<ScrollArray>();

        isEating = false;
        isRecommending = false;
        recommendedPiece = null;
        isSmelling = false;
    }

    public void StartAction()
    {
        //cancelButton.gameObject.SetActive(true);
    }

    public void CancelAction()
    {
        //cancelButton.gameObject.SetActive(false);
    }

    public void Poison()
    {
        if (playerHealth >= 1){playerHealth -= 1;}
    }

    public void Health()
    {
        if (playerHealth < 2){playerHealth += 1;}
    }

    public void Quake()
    {
        plate.Shuffle();
        playerScrolls.AddScrollAmount(-1, 3);
    }

    public void Slap()
    {
        if (!actionable)
        {
            gameManager.NextPlayer();
            playerScrolls.AddScrollAmount(-1, 0);
        }
    }

    public void Skip()
    {
        gameManager.NextPlayer();
        playerScrolls.AddScrollAmount(-1, 1);
    }

    public void Smell(GameObject targetPiece)
    {
        if (isSmelling && smellTargets.Count <= 3)
        {
            if (!smellTargets.Contains(targetPiece))
            {
                smellTarget = Instantiate(typeFlag, new Vector3(targetPiece.transform.position.x + 0.75f, targetPiece.transform.position.y + 1f, targetPiece.transform.position.z), Quaternion.identity);
                smellTarget.transform.SetParent(targetPiece.transform);
                smellTargets.Add(targetPiece);
                pieceType = targetPiece.GetComponent<FoodPiece>().type;
            }

            else
            {
                Destroy(smellTarget);
                smellTargets.Remove(targetPiece);
            }
        }

        playerScrolls.AddScrollAmount(-1, 2);
    }

    public void OrderDrink()
    {
        GameObject openDrinkMenu = Instantiate(drinkMenu);
        openDrinkMenu.transform.SetParent(transform.GetChild(1));
        playerScrolls.AddScrollAmount(-1, 4);
    }

    public void Eat()
    {
        if (!isEating)
        {
            isEating = true;
            isRecommending = false;
            isSmelling = false;
            Debug.Log("Is Eating");
        }

        else if (isEating)
        {
            ResetActions(false);
            Debug.Log("Stopped Eating");
        }
    }

    public void Recommend()
    {
        if (!isRecommending)
        {
            isRecommending = true;
            isSmelling = false;
            isEating = false;
            Debug.Log("Start Recommend");
        }
        else if (isRecommending)
        {
            ResetActions(false);
            Debug.Log("Stopped Recommend");
        }
    }

    public void ActionToggle(bool toggle)
    {
        actionable = toggle;
    }

    public void ResetActions(bool deactivate)
    {
        isEating = false;
        isRecommending = false;
        isSmelling = false;

        if (deactivate){actionable = false;}
    }


    void Update()
    {

        if (actionable)
        {
            if (Input.GetKeyDown("1")){Poison();}

            if (Input.GetKeyDown("2")){Quake();}

            if (Input.GetKeyDown("3")){Slap();}

            if (Input.GetKeyDown("4")){Health();}

            if (Input.GetKeyDown("5")){OrderDrink();}

            if (Input.GetKeyDown("6")){Eat();}

            if (Input.GetKeyDown("7")){isRecommending = true; Debug.Log("Ready to recommend");}


            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit  piece;
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
             
                if (Physics.Raycast(ray, out piece))
                {
                    if (piece.transform.tag == "FoodPiece")
                    {
                        if (isEating)
                        {
                
                            string foodType = piece.transform.gameObject.GetComponent<FoodPiece>().type;
                            Debug.Log(foodType);

                            if (foodType == "Slap"){playerScrolls.AddScrollAmount(1, 0);}

                            if (foodType == "Skip"){playerScrolls.AddScrollAmount(1, 1);}

                            if (foodType == "Smell"){playerScrolls.AddScrollAmount(1, 2);}

                            if (foodType == "Quake"){playerScrolls.AddScrollAmount(1, 3);}

                            if (foodType == "Order"){playerScrolls.AddScrollAmount(1, 4);}

                            if (foodType == "Poison"){Poison();}

                            if (foodType == "Health"){Health();}

                            Destroy(piece.transform.gameObject);
                            isEating = false;   
                        }

                        if (isRecommending)
                        {
                            if (recommendedPiece == null)
                            {
                                if (piece.transform.childCount == 0)
                                {
                                    Debug.Log("First  Recommend");
                                    currentRecommend = Instantiate(recommendFlag, new Vector3(piece.transform.position.x + 0.75f, piece.transform.position.y + 1f, piece.transform.position.z), Quaternion.identity);
                                    currentRecommend.transform.SetParent(piece.transform);
                                    recommendedPiece = piece.transform.gameObject;
                                }

                            }

                            else
                            {
                                if (piece.transform.gameObject == recommendedPiece)
                                {
                                    Debug.Log("Erasing recommend");
                                    Destroy(currentRecommend);
                                    recommendedPiece = null;
                                }

                                else
                                {
                                    Debug.Log("replacing recommend");
                                    Destroy(currentRecommend);
                                    currentRecommend = Instantiate(recommendFlag, new Vector3(piece.transform.position.x + 0.75f, piece.transform.position.y + 1f, piece.transform.position.z), Quaternion.identity);
                                    currentRecommend.transform.SetParent(piece.transform);
                                    recommendedPiece = piece.transform.gameObject;
                                }
                            }

                            isRecommending = false;
                        }

                        // if (isSmelling)
                        // {
                        //     Smell(piece);
                        //     smellFlag.smellFlagType.SetFlag(pieceType);
                        // }
                    }
                }
            }
        }
    }
}
