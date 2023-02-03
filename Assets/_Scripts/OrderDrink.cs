using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class OrderDrink : NetworkBehaviour
{
    public Sprite psnDrink, normalDrink;
    
    private Transform glasses;
    private PlayerFunctions playerFunctions;
    private PlayerManager victim;
    private readonly bool[] psnArray = new bool[4];
    private int psnMax;
    private Button orderButton, switchButton;
    private readonly List<PlayerManager> victims = new();
    private ToggleGroup playerToggles;
    private StateManager stateManager;

    public void Start()
    {
        victim = null;
        psnMax = 1;
        
        int playerCount = 0;
        victims.Clear();
        switchButton = transform.Find("SwitchButton").GetComponent<Button>();
        playerToggles = transform.Find("Players").GetComponent<ToggleGroup>();
        Debug.LogWarning(playerToggles.name);
        orderButton = transform.Find("OrderButton").GetComponent<Button>();
        glasses = transform.Find("Glasses");
        stateManager = GameObject.Find("StateManager(Clone)").GetComponent<StateManager>();
        playerFunctions = GameObject.Find("PlayerCanvas").GetComponent<PlayerFunctions>();
        
        for (int i = 0; i < 4; i++)
        {
            psnArray[i] = false;
        }

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
        
        PsnGlass(0);
    }

    public void Update()
    {
        if (victim != null)
        {
            if (psnArray.Length > 0)
            {
                orderButton.interactable = true;
            }
        }
        else
        {
            orderButton.interactable = false;
        }

        if (victim != null && stateManager.currentPlayer.GetComponent<PlayerManager>().health >= 2)
        {
            switchButton.interactable = true;
        }
        else
        {
            switchButton.interactable = false;
        }
    }

    public void SelectVictim()
    {
        if (playerToggles != null)
        {
            Toggle playerToggle = playerToggles.ActiveToggles().FirstOrDefault();

            foreach (Transform toggle in playerToggles.transform)
            {
                if (toggle.GetComponent<Toggle>().isOn)
                {
                    playerToggle = toggle.GetComponent<Toggle>();
                }
            }
            victim = victims[playerToggle.transform.GetSiblingIndex()];
        }
    }

    private void ResetPsnArray()
    {
        for (int i = 0; i < 4; i++)
        {
            psnArray[i] = false;
            glasses.GetChild(i).GetComponent<Image>().sprite = normalDrink;
        }
    }

    // Updates drink sprites according to which booleans within psnArray are set to true

    private int PoisonCount()
    {
        int counter = 0;
        foreach (var drink in psnArray)
        {
            if (drink)
            {
                counter += 1;
            }
        }

        return counter;
    }
    
    public void PsnGlass(int index)
    {
        if (!psnArray[index])
        {
            if (PoisonCount() < psnMax)
            {
                psnArray[index] = true;
                glasses.GetChild(index).GetComponent<Image>().sprite = psnDrink;
            }
        }
        else
        {
            psnArray[index] = false;
            glasses.GetChild(index).GetComponent<Image>().sprite = normalDrink;
        }
    }

    public void CloseMenu()
    {
        victims.Clear();
        gameObject.GetComponent<SpawnMenu>().SlideOutMenu(); 
    }

    public void ChangeAmount()
    {
        ResetPsnArray();
        PsnGlass(0);

        if (psnMax == 1)
        {
            psnMax = 2;
            switchButton.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Poison One Drink \n (No Cost)"; }
        else
        {
            psnMax = 1;
            switchButton.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Poison Two Drinks \n (Costs One Heart)";
        }
    }
    
    public void Order()
    {
        if (psnMax == 2)
        {
            playerFunctions.Poison(false);
        }
        
        playerFunctions.RemoveDrinkScroll();

        stateManager.CmdSyncOrder(psnArray, victim);
        
        CloseMenu();
    }
}
