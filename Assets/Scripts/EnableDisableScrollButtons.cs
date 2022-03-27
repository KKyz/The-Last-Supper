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
    public Transform actionButtons;
    public Transform scrollButtons;
    
    [SerializeField]
    private int menuMode;

    void Start()
    {
        actionButtons.gameObject.SetActive(true);
        scrollButtons.gameObject.SetActive(true);
    }

    public void ToggleButtons(int isActive)
    {
        menuMode = isActive;
        
        // Opens Scrolls Menu
        if (isActive == 1)
        {

            foreach (GameObject button in scrollButtons)
            {
                StartCoroutine(SpawnButtons(button));
            }

            foreach (GameObject button in actionButtons)
            {
                if (button.activeInHierarchy)
                {StartCoroutine(DespawnButtons(button));}
            }

            cancelButton.SetActive(false);
        }

        // Opens Actions Menu
        else if (isActive == 2)
        {

            foreach (GameObject button in scrollButtons)
            {
                if (button.activeInHierarchy)
                {StartCoroutine(DespawnButtons(button));}
            }

            foreach (GameObject button in actionButtons)
            {
                if (button.name == "SlapButton" || button.name == "RecommendButton")
                {continue;}

                StartCoroutine(SpawnButtons(button));
            }

            cancelButton.SetActive(false);
        }

        //Enables Cancel Button
        else if (isActive == 3)
        {
            
            foreach (GameObject button in scrollButtons)
            {
                if (button.activeInHierarchy)
                {StartCoroutine(DespawnButtons(button));}
            }

            foreach (GameObject button in actionButtons)
            {
                if (button.activeInHierarchy)
                {StartCoroutine(DespawnButtons(button));}
            }

            StartCoroutine(SpawnButtons(cancelButton));
        }

        // (isActive == 4) Disables everything (except for Slap)
        // (isActive == 5) Disables every button except skip (used for DrinkMenu)
        // (isActive == 6) Disables everything (no exceptions)

        else if (isActive == 4 || isActive == 5 || isActive == 6)
        {
            foreach (GameObject button in scrollButtons)
            {
                if (button.activeInHierarchy)
                {StartCoroutine(DespawnButtons(button));}
            }

            foreach (GameObject button in actionButtons)
            {
                if (button.activeInHierarchy)
                {StartCoroutine(DespawnButtons(button));}
            }  

            StartCoroutine(DespawnButtons(cancelButton));
        }
        
    }

    void Update()
    {
        if (menuMode == 1)
        {
            for (int i = 0; i <= 4; i++)
            {
                if (playerScrollArray.GetValue((i + 1)).amount > 0){scrollButtons.GetChild(i).gameObject.SetActive(true);}
                else{scrollButtons.GetChild(i).gameObject.SetActive(false);}
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

    IEnumerator SpawnButtons(GameObject button)
    {
        yield return new WaitForSeconds(1f);
        button.SetActive(true);
        float targetPos = button.transform.position.y;
        //button.transform.position = transform.position + new Vector3(0, -5f, 0);
        //LeanTween.moveY(button, targetPos, 3f);
    }

    IEnumerator DespawnButtons(GameObject button)
    {
        yield return new WaitForSeconds(1f);
        float targetPos = transform.position.y - 5f;
        //LeanTween.moveY(button, targetPos, 3f);
        button.SetActive(false);
    }
}
