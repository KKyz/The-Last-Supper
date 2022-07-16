using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EnableDisableScrollButtons : NetworkBehaviour
{
    public ScrollArray playerScrollArray;

    public PlayerManager playerManager;

    private PlayerFunctions playerFunctions;

    public GameObject slapButton, cancelButton, recommendButton, skipButton;
    public Transform actionButtons, scrollButtons;
    
    public int menuMode;

    void Start()
    {
        actionButtons.gameObject.SetActive(true);
        scrollButtons.gameObject.SetActive(true);
        playerFunctions = gameObject.GetComponent<PlayerFunctions>();

        StartCoroutine(PostStartCall());
    }

    private IEnumerator PostStartCall()
    {
        yield return new WaitForEndOfFrame();

        playerManager = playerFunctions.player;
        playerScrollArray = playerFunctions.playerScrolls;
        
    }
    
    [Client]
    public void ToggleButtons(int isActive)
    {
        menuMode = isActive;

        // Opens Scrolls Menu
        if (isActive == 1)
        {

            foreach (Transform button in scrollButtons)
            {
                button.gameObject.SetActive(true);
            }

            foreach (Transform button in actionButtons)
            {
                button.gameObject.SetActive(false);
            }

            cancelButton.SetActive(false);
            playerFunctions.infoText.GetComponent<InfoText>().CloseInfoText();
        }

        // Opens Actions Menu
        else if (isActive == 2)
        {

            foreach (Transform button in scrollButtons)
            {
                button.gameObject.SetActive(false);
            }

            foreach (Transform button in actionButtons)
            {
                if (button.name == "SlapButton" || button.name == "RecommendButton")
                {continue;}
                button.gameObject.SetActive(true);
            }

            cancelButton.SetActive(false);
            playerFunctions.infoText.GetComponent<InfoText>().CloseInfoText();
        }

        //Enables Cancel Button
        else if (isActive == 3)
        {
            
            foreach (Transform button in scrollButtons)
            {
                button.gameObject.SetActive(false);
            }

            foreach (Transform button in actionButtons)
            {
                button.gameObject.SetActive(false);
            }

            cancelButton.SetActive(true);
        }

        //Disables everything (except for Slap)

        else if (isActive == 4)
        {
            foreach (Transform button in scrollButtons)
            {
                button.gameObject.SetActive(false);
            }

            foreach (Transform button in actionButtons)
            {
                button.gameObject.SetActive(false);
            }  

            cancelButton.SetActive(false);
        }

        //Disables every button except skip (used for DrinkMenu)

        else if (isActive == 5)
        {
            foreach (Transform button in scrollButtons)
            {
                button.gameObject.SetActive(false);
            }

            foreach (Transform button in actionButtons)
            {
                button.gameObject.SetActive(false);
            }  

            cancelButton.SetActive(false);
        }

        //Disables everything (no exceptions, used for end of game or disconnect)

        else if (isActive == 6)
        {
            foreach (Transform button in scrollButtons)
            {
                button.gameObject.SetActive(false);
            }

            foreach (Transform button in actionButtons)
            {
                button.gameObject.SetActive(false);
            }  

            cancelButton.SetActive(false);
        }
    }

    void Update()
    {
        if (menuMode == 1)
        {
            for (int i = 1; i < scrollButtons.childCount; i++)
            {
                if (playerScrollArray.GetValue((i)).amount > 0){scrollButtons.GetChild(i).gameObject.SetActive(true);}
                else{scrollButtons.GetChild(i).gameObject.SetActive(false);}
            }           
        }

        if (menuMode == 2 && playerManager != null && !playerManager.hasRecommended)
        {recommendButton.SetActive(true);}
        else
        {recommendButton.SetActive(false);}

        if (menuMode == 4 && playerScrollArray != null && playerScrollArray.GetValue(0).amount > 0)
        {slapButton.SetActive(true);}
        else
        {slapButton.SetActive(false);}

        if (playerScrollArray != null && playerScrollArray.GetValue(1).amount > 0)
        {
            if (menuMode == 1 || menuMode == 5)
            {skipButton.SetActive(true);}
            else
            {skipButton.SetActive(false);}
        }

    }
}