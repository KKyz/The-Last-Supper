using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor.Experimental.GraphView;
using UnityEngine.SearchService;
using UnityEngine.UI;

public class EnableDisableScrollButtons : NetworkBehaviour
{
    public AudioClip selectSfx, cancelSfx, buttonSfx;

    private PlayerFunctions playerFunctions;
    private AudioSource sfxPlayer;
    private ScrollArray playerScrollArray;
    private PlayerManager playerManager;

    private GameObject slapButton, recommendButton, talkButton, outTalkButton, outRecommendButton, skipButton;
    private Transform actionButtons, scrollButtons, outsideButtons, cancelButton;

    public int menuMode;
    public float disableTime, enableTime;

    void Start()
    {
        playerFunctions = gameObject.GetComponent<PlayerFunctions>();
        actionButtons = transform.Find("ActionButtons").transform;
        scrollButtons = transform.Find("ScrollButtons").transform;
        outsideButtons = transform.Find("OutsideButtons").transform;
        cancelButton = transform.Find("CancelButton").transform;
        slapButton = outsideButtons.Find("SlapButton").gameObject;
        recommendButton = actionButtons.Find("RecommendButton").gameObject;
        talkButton = actionButtons.Find("TalkButton").gameObject;
        outRecommendButton = outsideButtons.Find("RecommendButton").gameObject;
        outTalkButton = outsideButtons.Find("TalkButton").gameObject;
        skipButton = scrollButtons.Find("SkipButton").gameObject;

        StartCoroutine(PostStartCall());
    }

    private IEnumerator PostStartCall()
    {
        yield return new WaitForEndOfFrame();

        playerManager = playerFunctions.player;
        playerScrollArray = playerFunctions.playerScrolls;
    }

    public void PlaySelectSfx()
    {
        sfxPlayer.clip = selectSfx;
        sfxPlayer.Play();  
    }

    public IEnumerator ButtonEnable(Transform button)
    {
        //If parent object of buttons (e.g. ActionButtons, ScrollButtons)
        if (button.childCount > 1)
        {
            foreach (Transform buttonChild in button)
            {
                if (!buttonChild.CompareTag("ConditionButton"))
                {
                    buttonChild.gameObject.SetActive(true);
                }

                if (buttonChild.gameObject.activeInHierarchy)
                {
                    buttonChild.GetComponent<Button>().interactable = false;
                    
                    //Sets Fade-In
                    CanvasGroup buttonCanvas = buttonChild.GetComponent<CanvasGroup>();
                    LeanTween.alphaCanvas(buttonCanvas, 1, 0.6f);
                    sfxPlayer.clip = buttonSfx;
                    sfxPlayer.Play();

                    yield return new WaitForSeconds(enableTime);
                    buttonChild.GetComponent<Button>().interactable = true;
                    buttonChild.GetComponent<AwakePos>().UpdatePos(buttonChild.position);
                }
            }
        }
        
        else
        {
            if (!button.gameObject.activeInHierarchy)
            {
                button.gameObject.SetActive(true);

                //Fade-In
                CanvasGroup buttonCanvas = button.GetComponent<CanvasGroup>();
                LeanTween.alphaCanvas(buttonCanvas, 1, 0.6f);
                sfxPlayer.clip = buttonSfx;
                sfxPlayer.Play();
            }
        }
    }

    public IEnumerator ButtonDisable(Transform button)
    {
        if (button.childCount > 1)
        {
            sfxPlayer.clip = cancelSfx;
            sfxPlayer.Play();
            //If parent object of buttons (e.g. ActionButtons, ScrollButtons)
            foreach (Transform buttonChild in button)
            {
                if (buttonChild.gameObject.activeInHierarchy)
                {
                    buttonChild.GetComponent<Button>().interactable = false;
                    
                    //Fade-Out
                    CanvasGroup buttonCanvas = buttonChild.GetComponent<CanvasGroup>();
                    LeanTween.alphaCanvas(buttonCanvas, 0, disableTime - 0.05f);
                    
                    yield return new WaitForSeconds(disableTime);
                }
            }

            foreach (Transform buttonChild in button)
            {
                if (buttonChild.gameObject.activeInHierarchy)
                {
                    buttonChild.GetComponent<Button>().interactable = true;
                    buttonChild.gameObject.SetActive(false);
                }
            }
        }
        
        else
        {
            if (button.gameObject.activeInHierarchy)
            {
                button.GetComponent<Button>().interactable = false;
                
                //Fade-Out
                CanvasGroup buttonCanvas = button.GetComponent<CanvasGroup>();
                LeanTween.alphaCanvas(buttonCanvas, 0, disableTime - 0.05f);
                sfxPlayer.clip = cancelSfx;
                sfxPlayer.Play();

                //Reset positions and disable
                yield return new WaitForSeconds(disableTime);
                button.GetComponent<Button>().interactable = true;
                button.gameObject.SetActive(false);
            }
        }
    }

    [Client]
    public void ToggleButtons(int isActive)
    {
        menuMode = isActive;
        Debug.Log("button mode is: " + menuMode);

        // Opens Scrolls Menu
        if (isActive == 1)
        {
            StartCoroutine(ButtonDisable(outsideButtons));
            
            StartCoroutine(ButtonEnable(scrollButtons));

            StartCoroutine(ButtonDisable(actionButtons));

            StartCoroutine(ButtonDisable(cancelButton));
            
            playerFunctions.infoText.GetComponent<InfoText>().CloseInfoText();
        }

        // Opens Actions Menu
        else if (isActive == 2)
        {
            StartCoroutine(ButtonDisable(outsideButtons));

            StartCoroutine(ButtonDisable(scrollButtons));

            StartCoroutine(ButtonEnable(actionButtons));

            StartCoroutine(ButtonDisable(cancelButton));
            
            playerFunctions.infoText.GetComponent<InfoText>().CloseInfoText();
        }

        //Enables Cancel Button
        else if (isActive == 3)
        {
            StartCoroutine(ButtonDisable(outsideButtons));
            
            StartCoroutine(ButtonDisable(scrollButtons));

            StartCoroutine(ButtonDisable(actionButtons));

            StartCoroutine(ButtonEnable(cancelButton));
            
        }

        //Disables everything (except for Slap, recommend, and Talk)

        else if (isActive == 4)
        {
            StartCoroutine(ButtonEnable(outsideButtons));
            
            StartCoroutine(ButtonDisable(scrollButtons));

            StartCoroutine(ButtonDisable(actionButtons));

            StartCoroutine(ButtonDisable(cancelButton));
        }

        //Disables every button except skip (used for DrinkMenu)

        else if (isActive == 5)
        {
            StartCoroutine(ButtonDisable(outsideButtons));
            
            StartCoroutine(ButtonDisable(scrollButtons));

            StartCoroutine(ButtonDisable(actionButtons));

            StartCoroutine(ButtonDisable(cancelButton));
        }

        //Disables everything (no exceptions, used for end of game or disconnect)

        else if (isActive == 6)
        {
            StartCoroutine(ButtonDisable(outsideButtons));
            
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

        if (playerManager != null)
        {
            if (!playerManager.hasRecommended)
            {
                if (menuMode == 2 && !recommendButton.activeInHierarchy)
                {
                    recommendButton.SetActive(true);
                }
            
                else if (menuMode == 4 && !outRecommendButton.activeInHierarchy)
                {
                    outRecommendButton.SetActive(true);  
                }
            
            }
        
            if (!playerManager.hasTalked)
            {
                if (menuMode == 2 && !talkButton.activeInHierarchy)
                {
                    talkButton.SetActive(true);
                }
            
                else if (menuMode == 4 && !outTalkButton.activeInHierarchy)
                {
                    outTalkButton.SetActive(true);  
                }
            
            }   
        }

        if (menuMode == 4 && playerScrollArray != null && playerScrollArray.GetValue(0).amount > 0)
        {
            slapButton.SetActive(true);
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