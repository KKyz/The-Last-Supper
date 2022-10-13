using System.Collections;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class EnableDisableScrollButtons : NetworkBehaviour
{
    public AudioClip selectSfx, cancelSfx, buttonSfx;

    private PlayerFunctions playerFunctions;
    private AudioSource sfxPlayer;
    public ScrollArray playerScrollArray;
    private PlayerManager playerManager;

    private CanvasGroup slapButton, recommendButton, talkButton, outTalkButton, outRecommendButton, skipButton;
    private Transform actionButtons, scrollButtons, outsideButtons, cancelButton;

    public int menuMode;
    public float disableTime, enableTime;

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
                    buttonCg.interactable = true;
                }

                if (buttonCg.interactable)
                {
                    buttonCg.interactable = false;

                    //Sets Fade-In
                    LeanTween.alphaCanvas(buttonCg, 1f, 0.6f);
                    sfxPlayer.PlayOneShot(buttonSfx);

                    yield return new WaitForSeconds(enableTime);
                    buttonCg.interactable = true;
                    buttonCg.blocksRaycasts = true;
                    buttonChild.GetComponent<AwakePos>().UpdatePos(buttonChild.position);
                }
            }
        }

        else
        {
            CanvasGroup buttonCg = button.GetComponent<CanvasGroup>();
            if (!buttonCg.interactable)
            {
                buttonCg.interactable = true;
                buttonCg.blocksRaycasts = true;

                //Fade-In
                CanvasGroup buttonCanvas = button.GetComponent<CanvasGroup>();
                LeanTween.alphaCanvas(buttonCanvas, 1f, 0.6f);
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
                if (buttonCg.interactable)
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
            if (button.GetComponent<CanvasGroup>().interactable)
            {
                button.GetComponent<CanvasGroup>().interactable = false;
                button.GetComponent<CanvasGroup>().blocksRaycasts = false;

                //Fade-Out
                CanvasGroup buttonCanvas = button.GetComponent<CanvasGroup>();
                LeanTween.alphaCanvas(buttonCanvas, 0, disableTime - 0.05f);
            }
        }
    }

    private void CheckConditions()
    {
        if (menuMode == 1)
        {
            for (int i = 1; i < scrollButtons.childCount; i++)
            {
                Debug.LogWarning(playerScrollArray.GetValue(i).name);
                if (playerScrollArray.GetValue((i)).amount > 0 && !scrollButtons.GetChild(i).GetComponent<CanvasGroup>().interactable)
                {
                    scrollButtons.GetChild(i).gameObject.SetActive(true);
                    CanvasGroup scrollCg = scrollButtons.GetChild(i).GetComponent<CanvasGroup>();
                    scrollCg.interactable = true;
                    scrollCg.alpha = 0;
                    scrollCg.blocksRaycasts = true;
                }
                else
                {
                    scrollButtons.GetChild(i).gameObject.SetActive(false);
                    CanvasGroup scrollCg = scrollButtons.GetChild(i).GetComponent<CanvasGroup>();
                    scrollCg.interactable = false;
                    scrollCg.blocksRaycasts = false;
                }
            }
        }
        
        if (playerManager != null)
        {
            if (!playerManager.hasRecommended)
            {
                if (menuMode == 2 && !recommendButton.interactable)
                {
                    recommendButton.gameObject.SetActive(true);
                    recommendButton.alpha = 0;
                    recommendButton.interactable = true;
                    recommendButton.blocksRaycasts = true;
                }

                else if (menuMode == 4 && !outRecommendButton.interactable)
                {
                    outRecommendButton.gameObject.SetActive(true);
                    outRecommendButton.alpha = 0;
                    outRecommendButton.interactable = true;
                    outRecommendButton.blocksRaycasts = true;
                }

            }

            else if (menuMode == 2 || menuMode == 4)
            {
                recommendButton.gameObject.SetActive(false);
                recommendButton.interactable = false;
                recommendButton.blocksRaycasts = false;

                outRecommendButton.gameObject.SetActive(false);
                outRecommendButton.alpha = 0;
                outRecommendButton.interactable = false;
                outRecommendButton.blocksRaycasts = false;
            }

            if (!playerManager.hasTalked)
            {
                if (menuMode == 2 && !talkButton.interactable)
                {
                    talkButton.gameObject.SetActive(true);
                    talkButton.alpha = 0;
                    talkButton.interactable = true;
                    talkButton.blocksRaycasts = true;
                }

                else if (menuMode == 4 && !outTalkButton.interactable)
                {
                    outTalkButton.gameObject.SetActive(true);
                    outTalkButton.alpha = 0;
                    outTalkButton.interactable = true;
                    talkButton.blocksRaycasts = true;
                }

            }

            else if (menuMode == 2 || menuMode == 4)
            {
                talkButton.gameObject.SetActive(false);
                talkButton.interactable = false;
                talkButton.blocksRaycasts = false;

                outTalkButton.gameObject.SetActive(false);
                outTalkButton.alpha = 0;
                outTalkButton.interactable = false;
                outTalkButton.blocksRaycasts = false;
            }
        }

        if (menuMode == 4)
        {
            if (playerScrollArray != null && playerScrollArray.GetValue(0).amount > 0)
            {
                slapButton.gameObject.SetActive(true);
                slapButton.alpha = 0;
                slapButton.interactable = true;
                slapButton.blocksRaycasts = true;
            }

            else
            {
                slapButton.gameObject.SetActive(false);
                slapButton.interactable = false;
                slapButton.blocksRaycasts = false;
            }
        }

        if (playerScrollArray != null && playerScrollArray.GetValue(1).amount > 0 && !skipButton.interactable)
        {
            if (menuMode == 1)
            {
                skipButton.gameObject.SetActive(true);
                skipButton.alpha = 0;
                skipButton.interactable = true;
                skipButton.blocksRaycasts = true;
            }

            if (menuMode == 5)
            {
                StartCoroutine(ButtonEnable(skipButton.transform));
            }
        }

        else if (menuMode == 1 || menuMode == 5)
        {
            skipButton.gameObject.SetActive(false);
            skipButton.interactable = false;
            skipButton.blocksRaycasts = false;
        }

    }

    [Client]
    public void ToggleButtons(int isActive)
    {
        menuMode = isActive;
        Debug.LogWarning("MenuMode is: " + menuMode);

        CheckConditions();

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
            
            playerFunctions.infoText.GetComponent<InfoText>().CloseInfoText();
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
}