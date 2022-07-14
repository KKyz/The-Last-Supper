using System.Collections;
using System.Collections.Generic;
using Mirror;
using Mirror.Examples.Chat;
using UnityEngine;
using UnityEngine.UI;

public class OrderDrink : MonoBehaviour
{
    public Sprite psnWine, normalWine;
    private Transform buttons, wines;
    private PlayerFunctions playerFunctions;
    private PlayerManager victim;
    private bool takeHealth;
    private bool[] psnArray = new bool[4];
    private GameObject psnButton, orderButton;
    private int buttonCount;
    private List<PlayerManager> victims = new List<PlayerManager>();
    private StateManager stateManager;
    private EnableDisableScrollButtons buttonToggle;

    public void Start()
    {
        takeHealth = false;
        victim = null;
        buttonCount = 0;
        victims.Clear();
        psnButton = transform.Find("SwitchButton").gameObject;
        orderButton = transform.Find("OrderButton").gameObject;
        buttons = transform.Find("Buttons");
        wines = transform.Find("Wines");
        stateManager = GameObject.Find("StateManager").GetComponent<StateManager>();
        playerFunctions = GameObject.Find("PlayerCanvas").GetComponent<PlayerFunctions>();
        buttonToggle =  buttonToggle = GameObject.Find("PlayerCanvas").GetComponent<EnableDisableScrollButtons>();

        if (stateManager.currentPlayer.GetComponent<PlayerManager>().health == 2)
        {psnButton.SetActive(true);}
        else {psnButton.SetActive(false);}

        /* This for loop doesn't work on client end as the client somehow cannot see stateManager.players,
         despite it being perfectly visible in both server and clients' inspector of StateManager*/
        
        foreach (uint playerID in stateManager.players)
        {
            GameObject player = NetworkServer.spawned[playerID].gameObject;
            if (player != stateManager.currentPlayer)
            {
                victims.Add(player.GetComponent<PlayerManager>());
                buttons.GetChild(buttonCount).gameObject.SetActive(true);
                buttons.GetChild(buttonCount).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = player.name;
                buttonCount += 1;
            }
        }
    }

    public void Update()
    {
        if (victim != null)
        {orderButton.SetActive(true);}
        else {orderButton.SetActive(false);}

        if (victim != null && stateManager.currentPlayer.GetComponent<PlayerManager>().health >= 2)
        {psnButton.SetActive(true);}
        else {psnButton.SetActive(false);}
    }

    public void SelectVictim(int vicNum)
    {
        victim = victims[vicNum];
        takeHealth = true;
        ChangeAmount();
    }

    // Updates wine sprites according to which booleans within psnArray are set to true
    private void UpdateWine()
    {
        if (victim != null)
        {
            for (int i = 0; i <= psnArray.Length - 1; i++)
            {
                if (psnArray[i])
                {
                    Debug.Log("This has poison");
                    wines.GetChild(i).GetComponent<Image>().sprite = psnWine;
                }

                else
                {
                    wines.GetChild(i).GetComponent<Image>().sprite = normalWine;
                }
            }
        }
    }

    public void CloseMenu()
    {
        victims.Clear();
        buttonToggle.ToggleButtons(2);
        gameObject.GetComponent<SpawnMenu>().SlideOutMenu();
    }

    public void ChangeAmount()
    {

        for (int i = 0; i < 4; i++)
        {
            psnArray[i] = false;
        }

        if (!takeHealth)
        {
            takeHealth = true;
            for (int i = 0; i < 2; i++)
            {
                int j = Random.Range(0, 4);
                psnArray[j] = true;

                int k = -1;
                while (k != j)
                {
                    k = Random.Range(0, 4);
                }

                psnArray[k] = true;

            }
            psnButton.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Poison One Drink \n (No Cost)";
            
        }
        else 
        {
            takeHealth = false;
            for (int i = 0; i < 1; i++)
            {
                int j = Random.Range(0, 4);
                {
                    psnArray[j] = true;
                }
            }
            psnButton.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Poison Two Drinks \n (Costs One Heart)";
        }
        
        UpdateWine();
    }

    public void Order()
    {
        /*This function runs fine on server side (even though you can't see psn(0-3) changing),
        but only changes psnArray in the local instance of victim, and not victim in ALL instances*/
        
        victim.psn0 = psnArray[0];
        victim.psn1 = psnArray[1];
        victim.psn2 = psnArray[2];
        victim.psn3 = psnArray[3];
        victim.SyncPsn();
        
        victim.orderVictim = true;

        if (takeHealth)
        {
            playerFunctions.Poison(false);
        }
        
        playerFunctions.RemoveDrinkScroll();

        CloseMenu();
    }
}
