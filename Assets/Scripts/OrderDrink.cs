using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Mirror.Examples.Chat;
using UnityEngine;
using UnityEngine.UI;

public class OrderDrink : NetworkBehaviour
{
    public Sprite psnWine, normalWine;
    private Transform toggles, wines;
    private PlayerFunctions playerFunctions;
    private PlayerManager victim;
    private bool takeHealth;
    private bool[] psnArray = new bool[4];
    private Button psnButton, orderButton;
    private readonly List<PlayerManager> victims = new();
    private ToggleGroup playerToggles;
    private StateManager stateManager;

    public void Start()
    {
        takeHealth = false;
        victim = null;
        int buttonCount = 0;
        victims.Clear();
        psnButton = transform.Find("SwitchButton").GetComponent<Button>();
        toggles = transform.Find("Players");
        playerToggles = toggles.GetComponent<ToggleGroup>();
        orderButton = transform.Find("OrderButton").GetComponent<Button>();
        wines = transform.Find("Wines");
        stateManager = GameObject.Find("StateManager").GetComponent<StateManager>();
        playerFunctions = GameObject.Find("PlayerCanvas").GetComponent<PlayerFunctions>();

        foreach (uint playerID in stateManager.activePlayers)
        {
            PlayerManager player = stateManager.spawnedPlayers[playerID];
            if (player.gameObject != stateManager.currentPlayer)
            {
                victims.Add(player.GetComponent<PlayerManager>());
                toggles.GetChild(buttonCount).gameObject.SetActive(true);
                toggles.GetChild(buttonCount).GetComponentInChildren<TMPro.TextMeshProUGUI>().text = player.name;
                buttonCount += 1;
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
