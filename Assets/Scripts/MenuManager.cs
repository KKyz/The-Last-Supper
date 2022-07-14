using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public GameObject startMenu, blur;

    private void Start()
    {
        blur = transform.Find("Blur").gameObject;
        startMenu = transform.Find("Start").gameObject;
        OpenMainMenu();
    }

    public void OpenMenu(GameObject MenuCont)
    {
        MenuCont.SetActive(true);
        startMenu.SetActive(false);

        foreach (Transform menuObj in MenuCont.transform)
        {
            if (menuObj.CompareTag("TitleMenu"))
            {
                menuObj.GetComponent<CanvasGroup>().alpha = 1f;
                Vector3 goalPos = menuObj.position;
                Vector3 startPos = new Vector3((goalPos.x + 400f), goalPos.y, goalPos.z);
                menuObj.position = startPos;
                LeanTween.move(menuObj.gameObject, goalPos, 0.2f);
            }

            if (menuObj.CompareTag("CancelBtn"))
            {
                menuObj.GetComponent<CanvasGroup>().alpha = 1f;
                Vector3 goalPos = menuObj.position;
                Vector3 startPos = new Vector3((goalPos.x - 400f), goalPos.y, goalPos.z);
                menuObj.position = startPos;
                LeanTween.move(menuObj.gameObject, goalPos, 0.2f);
            }
            
            // Add tween command to blur BG
        }
    }

    private void OpenMainMenu()
    {
        foreach (Transform button in startMenu.transform)
        {
            CanvasGroup buttonCG = button.GetComponent<CanvasGroup>();
            buttonCG.alpha = 0f;
            LeanTween.alphaCanvas(buttonCG, 1, 0.8f);
        }
    }

    public void CloseMenu(GameObject MenuCont)
    {
        StartCoroutine(CloseMenuTween(MenuCont));
        startMenu.SetActive(true);
        
        OpenMainMenu();
    }

    private IEnumerator CloseMenuTween(GameObject MenuCont)
    {
        foreach (Transform menuObj in MenuCont.transform)
        {
            if (menuObj.CompareTag("TitleMenu"))
            {
                Vector3 startPos = menuObj.position;
                Vector3 goalPos = new Vector3((startPos.x + 400f), startPos.y, startPos.z);
                LeanTween.move(menuObj.gameObject, goalPos, 0.2f);
            }

            if (menuObj.CompareTag("CancelBtn"))
            {
                Vector3 startPos = menuObj.position;
                Vector3 goalPos = new Vector3((startPos.x - 400f), startPos.y, startPos.z);
                LeanTween.move(menuObj.gameObject, goalPos, 0.2f);
            }
            
            // Add tween command to unblur BG
        }

        yield return new WaitForSeconds(1f);
        MenuCont.SetActive(false);
    }
}
