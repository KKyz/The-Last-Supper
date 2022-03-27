using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderDrink : MonoBehaviour
{
    public Sprite psnWine, normalWine;
    private Transform buttons, wines;
    private PlayerManager victim;
    private bool takeHealth;
    private GameObject psnButton, orderButton;
    private int buttonCount;
    private List<PlayerManager> victims = new List<PlayerManager>();
    private GameManager gameManager;
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
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        buttonToggle =  buttonToggle = GameObject.Find("PlayerCanvas").GetComponent<EnableDisableScrollButtons>();

        if (gameManager.currentPlayer.GetComponent<PlayerManager>().health == 2)
        {psnButton.SetActive(true);}
        else {psnButton.SetActive(false);}

        foreach (GameObject player in gameManager.players)
        {
            if (player != gameManager.currentPlayer)
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

        if (victim != null && gameManager.currentPlayer.GetComponent<PlayerManager>().health == 2)
        {psnButton.SetActive(true);}
        else {psnButton.SetActive(false);}
    }

    public void SelectVictim(int vicNum)
    {
        victim = victims[vicNum];
        takeHealth = true;
        ChangeAmount();
    }

    private void UpdateWine()
    {
        if (victim != null)
        {
            foreach (bool poison in victim.psnArray)
            {
                foreach (Transform wine in wines)
                {
                    if (poison)
                    {
                        wine.gameObject.GetComponent<Image>().sprite = psnWine;
                    }
                    else {wine.gameObject.GetComponent<Image>().sprite = normalWine;}
                }
            }
        }
    }

    public void CloseMenu()
    {
        victims.Clear();
        buttonToggle.ToggleButtons(2);
        Destroy(gameObject);
    }

    public void ChangeAmount()
    {

        for (int i = 0; i < 4; i++)
        {
            victim.psnArray[i] = false;
        }

        if (!takeHealth)
        {
            takeHealth = true;
            for (int i = 0; i < 2; i++)
            {
                int j = Random.Range(0, 4);
                victim.psnArray[j] = true;

                int k = -1;
                while (k != j)
                {
                    k = Random.Range(0, 4);
                }

                victim.psnArray[k] = true;

            }
            UpdateWine();
            psnButton.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Poison One Drink \n (No Cost)";
            
        }
        else 
        {
            takeHealth = false;
            for (int i = 0; i < 1; i++)
            {
                int j = Random.Range(0, 4);
                {
                    victim.psnArray[j] = true;
                }
            }
            UpdateWine();
            psnButton.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Poison Two Drinks \n (Costs One Heart)";
        }
    }

    public void Order()
    {
        victim.orderVictim = true;

        if (takeHealth)
        {
            gameManager.currentPlayer.GetComponent<PlayerManager>().health -= 1;
        }

        CloseMenu();
    }
}
