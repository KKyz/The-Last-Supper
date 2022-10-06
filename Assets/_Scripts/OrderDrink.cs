using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class OrderDrink : NetworkBehaviour
{
    public Sprite psnWine, normalWine;
    private Transform wines;
    private PlayerFunctions playerFunctions;
    private PlayerManager victim;
    private bool takeHealth;
    private readonly bool[] psnArray = new bool[4];
    private Button psnButton, orderButton;
    private readonly List<PlayerManager> victims = new();
    private ToggleGroup playerToggles;
    private StateManager stateManager;

    public void Start()
    {
        takeHealth = false;
        victim = null;
        int playerCount = 0;
        victims.Clear();
        psnButton = transform.Find("SwitchButton").GetComponent<Button>();
        playerToggles = transform.Find("Players").GetComponent<ToggleGroup>();
        orderButton = transform.Find("OrderButton").GetComponent<Button>();
        wines = transform.Find("Wines");
        stateManager = GameObject.Find("StateManager(Clone)").GetComponent<StateManager>();
        playerFunctions = GameObject.Find("PlayerCanvas").GetComponent<PlayerFunctions>();

        foreach (NetworkIdentity player in stateManager.activePlayers)
        {
            PlayerManager playerManager = player.GetComponent<PlayerManager>();
            if (player.gameObject != stateManager.currentPlayer)
            {
                victims.Add(playerManager);
                playerToggles.transform.GetChild(playerCount).gameObject.SetActive(true);
                playerToggles.transform.GetChild(playerCount).GetComponentInChildren<TMPro.TextMeshProUGUI>().text = player.name;
                playerCount += 1;
            }
        }
    }

    public void Update()
    {
        if (victim != null)
        {orderButton.interactable = true;}
        else {orderButton.interactable = false;}

        if (victim != null && stateManager.currentPlayer.GetComponent<PlayerManager>().health >= 2)
        {psnButton.interactable = true;}
        else {psnButton.interactable = false;}
    }

    public void SelectVictim()
    {
        Toggle playerToggle = playerToggles.ActiveToggles().FirstOrDefault();
        victim = victims[playerToggle.transform.GetSiblingIndex()];
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
        if (takeHealth)
        {
            playerFunctions.Poison(false);
        }
        
        playerFunctions.RemoveDrinkScroll();

        stateManager.CmdSyncOrder(psnArray, victim);
        
        CloseMenu();
    }
}
