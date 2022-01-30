using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableDisableScrollButtons : MonoBehaviour
{
    [HideInInspector]
    public ScrollArray playerScrollArray;

    public GameObject slapButton;
    public Transform actionButtons;
    public Transform scrollButtons;

    private bool isActionable;
    private bool showButtons;

    void Start()
    {
        showButtons = false;
        actionButtons.gameObject.SetActive(true);
        isActionable = transform.parent.GetComponent<PlayerManager>().actionable;
        playerScrollArray = transform.parent.GetComponent<ScrollArray>();

        ToggleScrolls(false);
    }

    public void ToggleScrolls(bool isActive)
    {
        showButtons = isActive;

        if (isActive)
        {
            foreach (Transform button in scrollButtons)
            {
                button.gameObject.SetActive(true);
            }

            foreach (Transform button in actionButtons)
            {
                button.gameObject.SetActive(false);
            }
        }

        else
        {
            foreach (Transform button in scrollButtons)
            {
                button.gameObject.SetActive(false);
            }

            foreach (Transform button in actionButtons)
            {
                button.gameObject.SetActive(true);
            }
        }
    }

    void Update()
    {
        if (showButtons)
        {
            for (int i = 0; i <= 3; i++)
            {
                if (playerScrollArray.GetValue((i +1)).amount > 0){scrollButtons.GetChild(i).gameObject.SetActive(true);}
                else{scrollButtons.GetChild(i).gameObject.SetActive(false);}
            }           
        }

        else 
        {
            if (playerScrollArray.GetValue(0).amount > 0){slapButton.SetActive(true);}
            else{slapButton.SetActive(false);}
        }   
    }
}
