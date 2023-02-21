using System.Collections;
using UnityEngine;
using Mirror;

public class EnableDisableScrollButtons : MonoBehaviour
{
    public AudioClip selectSfx, cancelSfx, buttonSfx;

    private PlayerFunctions playerFunctions;
    private AudioSource sfxPlayer;
    public ScrollArray playerScrollArray;
    private PlayerManager playerManager;

    private CanvasGroup slapButton, recommendButton, talkButton, outTalkButton, outRecommendButton, skipButton, stealButton;
    private Transform actionButtons, scrollButtons, outsideButtons, cancelButton;

    public int menuMode;
    public float disableTime, enableTime;

    private IEnumerator currentAnimation;

    public void OnStartGame()
    {
        playerFunctions = GetComponent<PlayerFunctions>();
        sfxPlayer = GetComponent<AudioSource>();
        actionButtons = transform.Find("ActionButtons").transform;
        scrollButtons = transform.Find("ScrollButtons").transform;
        outsideButtons = transform.Find("OutsideButtons").transform;
        cancelButton = transform.Find("CancelButton").transform;
        slapButton = outsideButtons.Find("SlapButton").GetComponent<CanvasGroup>();
        recommendButton = actionButtons.Find("RecommendButton").GetComponent<CanvasGroup>();
        talkButton = actionButtons.Find("TalkButton").GetComponent<CanvasGroup>();
        outRecommendButton = outsideButtons.Find("RecommendButton").GetComponent<CanvasGroup>();
        outTalkButton = outsideButtons.Find("TalkButton").GetComponent<CanvasGroup>();
        skipButton = scrollButtons.Find("SkipButton").GetComponent<CanvasGroup>();
        stealButton = actionButtons.Find("StealButton").GetComponent<CanvasGroup>();

        StartCoroutine(PostStartCall());
    }

    private IEnumerator PostStartCall()
    {
        yield return new WaitForEndOfFrame();

        playerManager = playerFunctions.player;
        playerScrollArray = playerFunctions.playerScrolls;
        
        outTalkButton.gameObject.SetActive(false);
        talkButton.gameObject.SetActive(false);
        stealButton.gameObject.SetActive(false);
    }

    private void Update()
    {
        //Duct-tape measure, find a better solution next time
        
        if (menuMode == 6)
        {
            ToggleButtons(6);
        }
        
        if (menuMode == 2 || menuMode == 4)
        {
            CheckConditions();
        }

        if (menuMode == 4)
        {
            foreach (Transform button in actionButtons)
            {
                if (button.GetComponent<CanvasGroup>().alpha > 0)
                {
                    StartCoroutine(ButtonDisable(actionButtons));
                }
            }
        }
    }

    public void PlaySelectSfx()
    {
        sfxPlayer.PlayOneShot(selectSfx);
    }

    public void PlayCancelSfx()
    {
        sfxPlayer.PlayOneShot(cancelSfx);
    }

    public IEnumerator ButtonEnable(Transform button)
    {
        //If parent object of buttons (e.g. ActionButtons, ScrollButtons)
        if (button.childCount > 1)
        {
            foreach (Transform buttonChild in button)
            {
                CanvasGroup buttonCg = buttonChild.GetComponent<CanvasGroup>();
                if (!buttonChild.CompareTag("ConditionButton"))
                {
                    buttonCg.blocksRaycasts = true;
                }

                if (buttonCg.blocksRaycasts)
                {
                    buttonCg.interactable = false;
                    buttonCg.blocksRaycasts = true;

                    //Sets Fade-In
                    LeanTween.alphaCanvas(buttonCg, 1, 0.6f);
                    sfxPlayer.PlayOneShot(buttonSfx);

                    yield return new WaitForSeconds(enableTime);
                    buttonCg.interactable = true;
                }
            }
        }

        else
        {
            CanvasGroup buttonCg = button.GetComponent<CanvasGroup>();
            if (!buttonCg.blocksRaycasts)
            {
                buttonCg.interactable = true;
                buttonCg.blocksRaycasts = true;

                //Fade-In
                LeanTween.alphaCanvas(buttonCg, 1, 0.6f);
                sfxPlayer.PlayOneShot(buttonSfx);
            }
        }
    }

    public IEnumerator ButtonDisable(Transform button)
    {
        if (button.childCount > 1)
        {
            //If parent object of buttons (e.g. ActionButtons, ScrollButtons) 
            foreach (Transform buttonChild in button)
            {
                CanvasGroup buttonCg = buttonChild.GetComponent<CanvasGroup>();
                if (buttonCg.blocksRaycasts || (buttonCg.alpha > 0 && !buttonCg.blocksRaycasts))
                {
                    buttonCg.interactable = false;
                    buttonCg.blocksRaycasts = false;

                    LeanTween.alphaCanvas(buttonCg, 0, disableTime - 0.05f);

                    yield return new WaitForSeconds(disableTime);
                }
            }
        }

        else
        {
            CanvasGroup buttonCg = button.GetComponent<CanvasGroup>();
            if (buttonCg.blocksRaycasts)
            {
                buttonCg.interactable = false;
                buttonCg.blocksRaycasts = false;
                LeanTween.alphaCanvas(buttonCg, 0, disableTime - 0.05f);
            }
        }
    }

    private void CheckConditions()
    {
        if (menuMode == 1)
        {
            for (int i = 1; i < scrollButtons.childCount; i++)
            {
                if (playerScrollArray.GetScrollAmount(i) > 0 && !scrollButtons.GetChild(i).GetComponent<CanvasGroup>().blocksRaycasts)
                {
                    scrollButtons.GetChild(i).gameObject.SetActive(true);
                    CanvasGroup scrollCg = scrollButtons.GetChild(i).GetComponent<CanvasGroup>();
                    scrollCg.alpha = 0;
                    scrollCg.blocksRaycasts = true;
                }
                else
                {
                    scrollButtons.GetChild(i).gameObject.SetActive(false);
                    CanvasGroup scrollCg = scrollButtons.GetChild(i).GetComponent<CanvasGroup>();
                    scrollCg.blocksRaycasts = false;
                }
            }
        }

        if (playerManager != null)
        {
            if (!playerManager.hasRecommended)
            {
                if (menuMode == 2 && !recommendButton.blocksRaycasts)
                {
                    recommendButton.gameObject.SetActive(true);
                    recommendButton.alpha = 0;
                    recommendButton.blocksRaycasts = true;
                }

                else if (menuMode == 4 && !outRecommendButton.blocksRaycasts)
                {
                    outRecommendButton.gameObject.SetActive(true);
                    outRecommendButton.alpha = 0;
                    outRecommendButton.blocksRaycasts = true;
                }
            }

            else if (menuMode == 2 || menuMode == 4)
            {
                recommendButton.gameObject.SetActive(false);
                recommendButton.blocksRaycasts = false;

                outRecommendButton.gameObject.SetActive(false);
                outRecommendButton.blocksRaycasts = false;
            }

            if (!playerManager.hasTalked)
            {
                if (menuMode == 2 && !talkButton.blocksRaycasts)
                {
                    talkButton.gameObject.SetActive(true);
                    talkButton.alpha = 0;
                    talkButton.blocksRaycasts = true;
                }

                else if (menuMode == 4 && !outTalkButton.blocksRaycasts)
                {
                    outTalkButton.gameObject.SetActive(true);
                    outTalkButton.alpha = 0;
                    outTalkButton.blocksRaycasts = true;
                }

            }

            else if (menuMode == 2 || menuMode == 4)
            {
                talkButton.gameObject.SetActive(false);
                talkButton.blocksRaycasts = false;

                outTalkButton.gameObject.SetActive(false);
                outTalkButton.blocksRaycasts = false;
            }

            if (playerFunctions.stateManager.stealActive)
            {
                if (playerManager.canSteal)
                {
                    if (menuMode == 2 && !stealButton.blocksRaycasts)
                    {
                        stealButton.gameObject.SetActive(true);
                        stealButton.alpha = 0;
                        stealButton.blocksRaycasts = true;
                    }
                }
                
                else if (menuMode == 2)
                {
                    stealButton.gameObject.SetActive(false);
                    stealButton.blocksRaycasts = false;
                }
            }
        }

        if (menuMode == 4)
        {
            if (playerScrollArray != null && playerScrollArray.GetScrollAmount("Slap") > 0)
            {
                if (!slapButton.blocksRaycasts)
                {
                    slapButton.gameObject.SetActive(true);
                    slapButton.alpha = 0;
                    slapButton.blocksRaycasts = true;
                }
            }

            else
            {
                slapButton.gameObject.SetActive(false);
                slapButton.blocksRaycasts = false;
            }
        }
        
        else if (menuMode != 4 && slapButton.blocksRaycasts)
        {
            slapButton.gameObject.SetActive(false);
            slapButton.blocksRaycasts = false;
        }

        if (menuMode == 5)
        {
            if (playerScrollArray != null && playerScrollArray.GetScrollAmount("Skip") > 0)
            {
                if (!skipButton.blocksRaycasts)
                {
                    StartCoroutine(ButtonEnable(skipButton.transform));  
                }
            }
            
            else
            {
                Debug.LogWarning("Skip unactive");
                skipButton.gameObject.SetActive(false);
                skipButton.interactable = false;
                skipButton.blocksRaycasts = false;
            }
        }
        
    }
    
    public void ToggleButtons(int isActive)
    {
        menuMode = isActive;
        
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        
        CheckConditions();

        // Opens Scrolls Menu
        if (isActive == 1)
        {
            currentAnimation = ButtonEnable(scrollButtons);
            
            StartCoroutine(ButtonDisable(outsideButtons));

            StartCoroutine(ButtonDisable(actionButtons));

            StartCoroutine(ButtonDisable(cancelButton));
            
            playerFunctions.infoText.GetComponent<InfoText>().CloseInfoText();
        }

        // Opens Actions Menu
        else if (isActive == 2)
        {
            currentAnimation = ButtonEnable(actionButtons);
            
            StartCoroutine(ButtonDisable(outsideButtons));

            StartCoroutine(ButtonDisable(scrollButtons));

            StartCoroutine(ButtonDisable(cancelButton));
            
            playerFunctions.infoText.GetComponent<InfoText>().CloseInfoText();
        }

        //Enables Cancel Button
        else if (isActive == 3)
        {
            currentAnimation = ButtonEnable(cancelButton);
            
            StartCoroutine(ButtonDisable(outsideButtons));
            
            StartCoroutine(ButtonDisable(scrollButtons));

            StartCoroutine(ButtonDisable(actionButtons));

        }

        //Enables non-action state buttons

        else if (isActive == 4)
        {
            currentAnimation = ButtonEnable(outsideButtons);
            
            StartCoroutine(ButtonDisable(scrollButtons));

            StartCoroutine(ButtonDisable(actionButtons));

            StartCoroutine(ButtonDisable(cancelButton));
            
            playerFunctions.infoText.GetComponent<InfoText>().CloseInfoText();
        }

        //Disables every button except skip (used for DrinkMenu)

        else if (isActive == 5)
        {
            currentAnimation = null;
            
            StartCoroutine(ButtonDisable(outsideButtons));
            
            StartCoroutine(ButtonDisable(scrollButtons));

            StartCoroutine(ButtonDisable(actionButtons));

            StartCoroutine(ButtonDisable(cancelButton));
        }

        //Disables everything (no exceptions, used for end of game or disconnect)

        else if (isActive == 6)
        {
            currentAnimation = null;
            
            StartCoroutine(ButtonDisable(outsideButtons));
            
            StartCoroutine(ButtonDisable(scrollButtons));

            StartCoroutine(ButtonDisable(actionButtons));

            StartCoroutine(ButtonDisable(cancelButton));
        }

        if (currentAnimation != null)
        {
            StartCoroutine(currentAnimation);
        }
    }
}