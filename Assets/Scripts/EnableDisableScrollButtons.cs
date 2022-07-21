using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UI;

public class EnableDisableScrollButtons : NetworkBehaviour
{
    public ScrollArray playerScrollArray;

    public PlayerManager playerManager;

    private PlayerFunctions playerFunctions;

    private GameObject slapButton, recommendButton, skipButton;
    private Transform actionButtons, scrollButtons, cancelButton;
    
    public int menuMode;
    public float disableTime, enableTime;

    void Start()
    {
        playerFunctions = gameObject.GetComponent<PlayerFunctions>();
        slapButton = transform.Find("SlapButton").gameObject;
        actionButtons = transform.Find("ActionButtons").transform;
        scrollButtons = transform.Find("ScrollButtons").transform;
        cancelButton = transform.Find("CancelButton").transform;
        recommendButton = actionButtons.Find("RecommendButton").gameObject;
        skipButton = scrollButtons.Find("SkipButton").gameObject;

        StartCoroutine(PostStartCall());
    }

    private IEnumerator PostStartCall()
    {
        yield return new WaitForEndOfFrame();

        playerManager = playerFunctions.player;
        playerScrollArray = playerFunctions.playerScrolls;
    }

    private IEnumerator ButtonEnable(Transform button)
    {
        //If parent object of buttons (e.g. ActionButtons, ScrollButtons)
        if (button.childCount > 1)
        {
            HorizontalLayoutGroup horiLayout = button.GetComponent<HorizontalLayoutGroup>();
            horiLayout.enabled = false;
            
            foreach (Transform buttonChild in button)
            {
                if (!buttonChild.CompareTag("ConditionButton"))
                {
                    buttonChild.gameObject.SetActive(true);
                }
                
                if (buttonChild.gameObject.activeInHierarchy)
                {
                    //Sets Fade-In
                    CanvasGroup buttonCanvas = buttonChild.GetComponent<CanvasGroup>();
                    buttonCanvas.alpha = 0f;
                    LeanTween.alphaCanvas(buttonCanvas, 1, 0.6f);

                    //Slide-In From Left
                    Vector3 goalPos = buttonChild.position;
                    Vector3 startPos = new Vector3((goalPos.x - 300), goalPos.y, 0);
                    buttonChild.position = startPos;
                    LeanTween.moveX(buttonChild.gameObject, goalPos.x, enableTime - 0.1f);
                    
                    yield return new WaitForSeconds(enableTime);
                }
            }

            horiLayout.enabled = true;
        }
        
        else
        {
            if (!button.gameObject.activeInHierarchy)
            {
                button.gameObject.SetActive(true);

                //Fade-In
                CanvasGroup buttonCanvas = button.GetComponent<CanvasGroup>();
                buttonCanvas.alpha = 0f;
                LeanTween.alphaCanvas(buttonCanvas, 1, 0.6f);

                //Slide-In From Left
                Vector3 goalPos = button.position;
                Vector3 startPos = new Vector3((goalPos.x - 300), goalPos.y, 0);
                button.position = startPos;
                LeanTween.moveX(button.gameObject, goalPos.x, enableTime - 0.1f);
            }
        }
    }

    private IEnumerator ButtonDisable(Transform button)
    {
        if (button.childCount > 1)
        {
            HorizontalLayoutGroup horiLayout = button.GetComponent<HorizontalLayoutGroup>();
            horiLayout.enabled = false;
            
            //If parent object of buttons (e.g. ActionButtons, ScrollButtons)
            foreach (Transform buttonChild in button)
            {
                if (buttonChild.gameObject.activeInHierarchy)
                {
                    //Fade-Out
                    CanvasGroup buttonCanvas = buttonChild.GetComponent<CanvasGroup>();
                    LeanTween.alphaCanvas(buttonCanvas, 0, disableTime - 0.05f);
                    
                    //Move Downwards
                    Vector3 startPos = buttonChild.position;
                    float goalPos = startPos.y - 100;
                    LeanTween.moveY(buttonChild.gameObject, goalPos, disableTime - 0.1f);
                    
                    //Reset positions and disable
                    yield return new WaitForSeconds(disableTime);
                    buttonChild.position = startPos;
                    buttonChild.gameObject.SetActive(false);
                    
                }
            }
            
            horiLayout.enabled = true;
        }
        
        else
        {
            if (button.gameObject.activeInHierarchy)
            {
                //Fade-Out
                CanvasGroup buttonCanvas = button.GetComponent<CanvasGroup>();
                LeanTween.alphaCanvas(buttonCanvas, 0, disableTime - 0.05f);

                //Move Downwards
                Vector3 startPos = button.position;
                float goalPos = startPos.y - 100;
                LeanTween.moveY(button.gameObject, goalPos, disableTime - 0.1f);

                //Reset positions and disable
                yield return new WaitForSeconds(disableTime);
                button.position = startPos;
                button.gameObject.SetActive(false);
            }
        }
    }

    [Client]
    public void ToggleButtons(int isActive)
    {
        menuMode = isActive;

        // Opens Scrolls Menu
        if (isActive == 1)
        {

            StartCoroutine(ButtonEnable(scrollButtons));

            StartCoroutine(ButtonDisable(actionButtons));

            StartCoroutine(ButtonDisable(cancelButton));
            
            playerFunctions.infoText.GetComponent<InfoText>().CloseInfoText();
        }

        // Opens Actions Menu
        else if (isActive == 2)
        {

            StartCoroutine(ButtonDisable(scrollButtons));

            StartCoroutine(ButtonEnable(actionButtons));

            StartCoroutine(ButtonDisable(cancelButton));
            
            playerFunctions.infoText.GetComponent<InfoText>().CloseInfoText();
        }

        //Enables Cancel Button
        else if (isActive == 3)
        {
            
            StartCoroutine(ButtonDisable(scrollButtons));

            StartCoroutine(ButtonDisable(actionButtons));

            StartCoroutine(ButtonEnable(cancelButton));
            
        }

        //Disables everything (except for Slap)

        else if (isActive == 4)
        {
            StartCoroutine(ButtonDisable(scrollButtons));

            StartCoroutine(ButtonDisable(actionButtons));

            StartCoroutine(ButtonDisable(cancelButton));
        }

        //Disables every button except skip (used for DrinkMenu)

        else if (isActive == 5)
        {
            StartCoroutine(ButtonDisable(scrollButtons));

            StartCoroutine(ButtonDisable(actionButtons));

            StartCoroutine(ButtonDisable(cancelButton));
        }

        //Disables everything (no exceptions, used for end of game or disconnect)

        else if (isActive == 6)
        {
            StartCoroutine(ButtonDisable(scrollButtons));

            StartCoroutine(ButtonDisable(actionButtons));

            StartCoroutine(ButtonDisable(cancelButton));
        }
    }

    void Update()
    {
        if (menuMode == 1)
        {
            for (int i = 1; i < scrollButtons.childCount; i++)
            {
                if (playerScrollArray.GetValue((i)).amount > 0 && !scrollButtons.GetChild(i).gameObject.activeInHierarchy)
                {
                    scrollButtons.GetChild(i).gameObject.SetActive(true);
                }
            }           
        }

        if (menuMode == 2 && playerManager != null && !playerManager.hasRecommended && !recommendButton.activeInHierarchy)
        {recommendButton.SetActive(true);}

        if (menuMode == 4 && playerScrollArray != null && playerScrollArray.GetValue(0).amount > 0)
        {
            StartCoroutine(ButtonEnable(slapButton.transform));
        }
        else
        {
            slapButton.SetActive(false);
        }

        if (playerScrollArray != null && playerScrollArray.GetValue(1).amount > 0 && !skipButton.activeInHierarchy)
        {
            if (menuMode == 1)
            {
                skipButton.SetActive(true);
            }

            if (menuMode == 5)
            {
                StartCoroutine(ButtonEnable(skipButton.transform));
            }
        }

    }
}