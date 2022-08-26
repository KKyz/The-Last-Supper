using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public Sprite[] tutorials = new Sprite[10];
    
    private int tutorialCount;
    private Button nextBtn, prevBtn;
    private Image tutorialImage;

    private void Start()
    {
        tutorialImage = transform.Find("Image").GetComponent<Image>();
        prevBtn = transform.Find("BackBtn").GetComponent<Button>();
        nextBtn = transform.Find("NextBtn").GetComponent<Button>();
        tutorialCount = 0;
        UpdateTutorial();
    }

    private void UpdateTutorial()
    {
        if (tutorialImage.gameObject.activeInHierarchy)
        {
            if (tutorialCount < tutorials.Length - 1)
            {
                nextBtn.interactable = true;
            }
            else
            {
                nextBtn.interactable = false;
            }

            if (tutorialCount > 0)
            {
                prevBtn.interactable = true;
            }
            else
            {
                prevBtn.interactable = false;
            }
        }
        tutorialImage.sprite = tutorials[tutorialCount];
    }

    public void NextTutorial()
    {
        tutorialCount += 1;
        UpdateTutorial();
    }

    public void PrevTutorial()
    {
        tutorialCount -= 1;
        UpdateTutorial();
    }
}
