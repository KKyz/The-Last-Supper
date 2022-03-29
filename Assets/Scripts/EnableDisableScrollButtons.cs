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

    void Start()
    {
        actionButtons.SetActive(true);
        scrollButtons.SetActive(true);
    }

    public void ToggleButtons(int isActive)
    {
        menuMode = isActive;
        Debug.Log("menuMode is: "+ menuMode.ToString());
        
        // Opens Scrolls Menu
        if (isActive == 1)
        {
            StartCoroutine(SpawnButtons(scrollButtons));
            
            StartCoroutine(DespawnButtons(actionButtons));

            cancelButton.SetActive(false);
        }

        // Opens Actions Menu
        else if (isActive == 2)
        {
            StartCoroutine(DespawnButtons(scrollButtons));
            
            StartCoroutine(SpawnButtons(actionButtons));

            cancelButton.SetActive(false);
        }

        //Enables Cancel Button
        else if (isActive == 3)
        {
            StartCoroutine(DespawnButtons(scrollButtons));
            
            StartCoroutine(DespawnButtons(actionButtons));

            StartCoroutine(SpawnButtons(cancelButton));
        }

        // (isActive == 4) Disables everything (except for Slap)
        // (isActive == 5) Disables every button except skip (used for DrinkMenu)
        // (isActive == 6) Disables everything (no exceptions)

        else if (isActive == 4 || isActive == 5 || isActive == 6)
        {
            StartCoroutine(DespawnButtons(scrollButtons));
            
            StartCoroutine(DespawnButtons(actionButtons));

            StartCoroutine(DespawnButtons(cancelButton));
        }
        
    }
    
    IEnumerator SpawnButtons(GameObject buttons)
    {
        buttons.SetActive(true);
        
        if (buttons.transform.childCount > 1)
        {
            // Spawn Children of buttons (e.g. action buttons, scroll buttons)
            foreach (Transform button in buttons.transform)
            {
                if (button.name != "SlapButton" || button.name != "RecommendButton")
                {
                    if (!button.gameObject.activeInHierarchy)
                    {
                        yield return new WaitForSeconds(0.2f);
                        button.gameObject.SetActive(true);
                        Vector3 goalPos = button.position;
                        button.position = new Vector3((goalPos.x - 10), goalPos.y, 0);
                        LeanTween.move(button.gameObject, goalPos, 0.2f);
                    }
                }
            }
        }
        
        // Spawn individual button (e.g. cancel button)
        else
        {
            yield return new WaitForSeconds(0.2f);
            Vector3 goalPos = buttons.transform.position;
            buttons.transform.position = new Vector3((goalPos.x - 10), goalPos.y, 0);
            buttons.gameObject.SetActive(true);
            LeanTween.move(buttons, goalPos, 0.2f);
        }
    }
    
    IEnumerator DespawnButtons(GameObject buttons)
    {
        if (buttons.transform.childCount > 1)
        {
            // Despawn Children of buttons (e.g. action buttons, scroll buttons)
            foreach (Transform button in buttons.transform)
            {
                if (button.gameObject.activeInHierarchy)
                {
                    Vector3 startPos = button.position;
                    Vector3 goalPos = new Vector3(startPos.x, (startPos.y - 100), 0);
                    yield return new WaitForSeconds(0.2f);
                    //LeanTween.alpha(button.gameObject, 0, 0.1f);
                    LeanTween.move(button.gameObject, goalPos, 0.2f);
                }
            }

            foreach (Transform button in buttons.transform)
            {
                yield return new WaitForSeconds(0.3f);
                button.gameObject.SetActive(false);
            }
        }

        // Despawn individual button (e.g. cancel button)
        else
        {
            if (buttons.gameObject.activeInHierarchy)
            {
                Vector3 startPos = buttons.transform.position;
                Vector3 goalPos = new Vector3(startPos.x, (startPos.y - 100), 0);
                yield return new WaitForSeconds(0.2f);
                LeanTween.alpha(buttons.gameObject, 0, 0.1f);
                LeanTween.move(buttons.gameObject, goalPos, 0.2f);
            }

            yield return new WaitForSeconds(0.3f);
            buttons.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (menuMode == 1)
        {
            for (int i = 0; i <= 4; i++)
            {
                if (playerScrollArray.GetValue((i + 1)).amount > 0){scrollButtons.transform.gameObject.SetActive(true);}
                else{scrollButtons.transform.gameObject.SetActive(false);}
            }           
        }

        if (menuMode == 2 && playerManager != null && !playerManager.hasRecommended)
        {StartCoroutine(SpawnButtons(recommendButton));}
        else
        {recommendButton.SetActive(false);}

        if (menuMode == 4 && playerScrollArray != null && playerScrollArray.GetValue(0).amount > 0)
        {StartCoroutine(SpawnButtons(slapButton));}
        else
        {slapButton.SetActive(false);}

        if (playerScrollArray != null && playerScrollArray.GetValue(1).amount > 0)
        {
            if (menuMode == 1 || menuMode == 5)
            {StartCoroutine(SpawnButtons(skipButton));}
            else
            {skipButton.SetActive(false);}
        }

    }
}
