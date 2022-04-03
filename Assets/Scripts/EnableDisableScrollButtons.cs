using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableDisableScrollButtons : MonoBehaviour
{
    [HideInInspector]
    public ScrollArray playerScrollArray;

    [HideInInspector]
    public PlayerManager playerManager;

    public GameObject slapButton, cancelButton, recommendButton, skipButton;
    public GameObject actionButtons;
    public GameObject scrollButtons;
    
    [SerializeField]
    private int menuMode;
    
    public void ToggleButtons(int isActive)
    {
        menuMode = isActive;
        Debug.Log("menuMode is: "+ menuMode.ToString());
        
        // Opens Scrolls Menu
        // (isActive == 5) Disables every button except skip (used for DrinkMenu)
        if (isActive == 1 || isActive == 5)
        {
            StopCoroutine(DespawnButtons(scrollButtons));
            StartCoroutine(SpawnButtons(scrollButtons));
            
            StartCoroutine(DespawnButtons(actionButtons));

            StartCoroutine(DespawnButtons(cancelButton));
        }

        // Opens Actions Menu
        else if (isActive == 2)
        {
            StartCoroutine(DespawnButtons(scrollButtons));
            
            StopCoroutine(DespawnButtons(actionButtons));
            StartCoroutine(SpawnButtons(actionButtons));

            StartCoroutine(DespawnButtons(cancelButton));
        }

        //Enables Cancel Button
        else if (isActive == 3)
        {
            StartCoroutine(DespawnButtons(scrollButtons));
            
            StartCoroutine(DespawnButtons(actionButtons));

            StopCoroutine(DespawnButtons(cancelButton));
            StartCoroutine(SpawnButtons(cancelButton));
        }

        // (isActive == 4) Disables everything (except for Slap)
        // (isActive == 6) Disables everything (no exceptions)

        else if (isActive == 4 || isActive == 6)
        {
            StopCoroutine(SpawnButtons(scrollButtons));
            StartCoroutine(DespawnButtons(scrollButtons));
            
            StopCoroutine(SpawnButtons(actionButtons));
            StartCoroutine(DespawnButtons(actionButtons));

            StopCoroutine(SpawnButtons(cancelButton));
            StartCoroutine(DespawnButtons(cancelButton));
        }
        
    }
    
    public IEnumerator SpawnButtons(GameObject buttons)
    {
        buttons.SetActive(true);
        
        if (buttons.transform.childCount > 2)
        {
            Debug.Log("Spawn multiple " + buttons.name);
            foreach (Transform button in buttons.transform)
            {
                if (button.gameObject.activeInHierarchy)
                {
                    yield return new WaitForSeconds(0.2f);
                    Vector3 goalPos = button.position;
                    CanvasGroup buttonCG = button.GetComponent<CanvasGroup>();
                    Vector3 startPos = new Vector3(goalPos.x, (goalPos.y - 300), 0);
                    buttonCG.alpha = 0f;
                    button.position = startPos;
                    LeanTween.alphaCanvas(buttonCG, 1, 0.5f);
                    LeanTween.move(button.gameObject, goalPos, 0.3f);
                }
            }
        }
        
        // Spawn individual button (e.g. cancel button)
        else
        {
            yield return new WaitForSeconds(0.2f);
            Vector3 goalPos = buttons.transform.position;
            CanvasGroup buttonCG = buttons.GetComponent<CanvasGroup>();
            Vector3 startPos = new Vector3(goalPos.x, (goalPos.y - 300), 0);
            buttonCG.alpha = 0f;
            buttons.transform.position = startPos;
            LeanTween.alphaCanvas(buttonCG, 1, 0.5f);
            LeanTween.move(buttons.gameObject, goalPos, 0.3f);
        }
    }
    
    public IEnumerator DespawnButtons(GameObject buttons)
    {
        if (buttons.transform.childCount > 2)
        {
            Debug.Log("Despawn multiple " + buttons.name);
            // Despawn Children of buttons (e.g. action buttons, scroll buttons)
            foreach (Transform button in buttons.transform)
            {
                if (button.gameObject.activeInHierarchy)
                {
                    Vector3 startPos = button.position;
                    CanvasGroup buttonCG = button.GetComponent<CanvasGroup>();
                    Vector3 goalPos = new Vector3(startPos.x, (startPos.y - 300), 0);
                    LeanTween.alphaCanvas(buttonCG, 0, 0.1f);
                    LeanTween.move(button.gameObject, goalPos, 0.3f);
                    yield return new WaitForSeconds(0.3f);
                    button.position = startPos;

                }
            }
        }

        // Despawn individual button (e.g. cancel button)
        else
        {
            Debug.Log("Despawn individual " + buttons.name);
            
            if (buttons.gameObject.activeInHierarchy)
            {
                Vector3 startPos = buttons.transform.position;
                CanvasGroup buttonCG = buttons.GetComponent<CanvasGroup>();
                Vector3 goalPos = new Vector3(startPos.x, (startPos.y - 300), 0);
                LeanTween.alphaCanvas(buttonCG, 0, 0.1f);
                LeanTween.move(buttons.gameObject, goalPos, 0.3f);
                yield return new WaitForSeconds(0.3f);
                buttons.transform.position = startPos;
            }
        }
        
        buttons.SetActive(false);
    }

    void Update()
    {
        if (menuMode == 1)
        {
            for (int i = 1; i <= 5; i++)
            {
                if (playerScrollArray.GetValue((i)).amount > 0){scrollButtons.transform.GetChild(i).gameObject.SetActive(true);}
                else{scrollButtons.transform.GetChild(i).gameObject.SetActive(false);}
            }           
        }

        if (menuMode == 2 && playerManager != null && !playerManager.hasRecommended)
        {recommendButton.SetActive(true);}
        else
        {recommendButton.SetActive(false);}

        if (menuMode == 4 && playerScrollArray != null && playerScrollArray.GetValue(0).amount > 0 && !slapButton.activeInHierarchy)
        {StartCoroutine(SpawnButtons(slapButton));}
        else
        {slapButton.SetActive(false);}

        if (playerScrollArray != null && playerScrollArray.GetValue(1).amount > 0 && !skipButton.activeInHierarchy)
        {
            if (menuMode == 5)
            {StartCoroutine(SpawnButtons(skipButton));}
            else if (menuMode == 1)
            {skipButton.SetActive(true);}
            else
            {skipButton.SetActive(false);}
        }

    }
}
