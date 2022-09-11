using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public AudioClip selectSfx, cancelSfx;
    private GameObject blur;
    private AudioSource uiAudio;
    private Camera titleCam;
    private Transform startMenu, altMenu;

    public void Start()
    {
        blur = transform.Find("Blur").gameObject;
        FadeInOut fade = GameObject.Find("Fade").GetComponent<FadeInOut>();
        uiAudio = GetComponent<AudioSource>();
        startMenu = transform.Find("Start");
        altMenu = transform.Find("Settings");
        OpenSubMenu(startMenu);
        blur.SetActive(false);
        
        titleCam = Camera.main;
        Vector3 camGoalPos = titleCam.transform.position;
        Vector3 camStartPos = new Vector3(camGoalPos.x, camGoalPos.y, camGoalPos.z - 30f);
        titleCam.transform.position = camStartPos;
        LeanTween.moveZ(titleCam.gameObject, camGoalPos.z, 0.8f).setEaseOutSine();
        fade.FadeOut(0.5f);
    }

    public void SelectSfx()
    {
        uiAudio.PlayOneShot(selectSfx);
    }

    public void CancelSfx()
    {
        uiAudio.PlayOneShot(cancelSfx);
    }

    public void OpenSubMenu(Transform subMenu)
    {
        if (subMenu != startMenu)
        {blur.SetActive(true);}
        
        foreach (Transform menuObj in subMenu)
        {
            if (menuObj.name != "TableSetUp")
            {
                menuObj.gameObject.SetActive(true);
            }
        }
        
        foreach (Transform menuObj in subMenu)
        {
            if (menuObj.gameObject.activeInHierarchy)
            {
                menuObj.GetComponent<MenuAnimations>().EnterAnim();
            }
        }
    }

    public void NameConditionSubMenu(Transform subMenu)
        {
            if (PlayerPrefs.GetString("PlayerName") != null || PlayerPrefs.GetString("PlayerName") != "" || PlayerPrefs.GetString("PlayerName").Length <= 5)
            {
                OpenSubMenu(subMenu);
            }
            else
            {
                OpenSubMenu(altMenu);
            }
        }

    public void CloseSubMenu(Transform subMenu)
    {
        blur.SetActive(false);

        foreach (Transform menuObj in subMenu)
        {
            menuObj.GetComponent<MenuAnimations>().ExitAnim();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
