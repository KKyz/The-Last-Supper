using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public AudioClip selectSfx, cancelSfx, startGameSfx;
    public GameObject gameManagerPrefab;
    public Transform content;
    private GameObject blur;
    private GameManager gameManager;
    private AudioSource uiAudio;
    private Camera titleCam;
    private Transform startMenu, settingsMenu, gameSetup, gameFind;
    private CustomNetworkDiscovery networkDiscovery;

    public void Start()
    {
        GameObject gm = GameObject.Find("GameManager");
        if (gm == null || !gm.TryGetComponent<GameManager>(out gameManager))
        { 
            gameManager = Instantiate(gameManagerPrefab).GetComponent<GameManager>();
            gameManager.gameObject.name = "GameManager";
        }
        
        gameManager.Init();
        gameManager.discoveryList = content;
        blur = transform.Find("Blur").gameObject;
        FadeInOut fade = GameObject.Find("Fade").GetComponent<FadeInOut>();
        networkDiscovery = GameObject.Find("GameManager").GetComponent<CustomNetworkDiscovery>();
        uiAudio = GetComponent<AudioSource>();
        startMenu = transform.Find("Start");
        settingsMenu = transform.Find("Settings");
        gameSetup = transform.Find("GameSetup");
        gameFind = transform.Find("GameSearch");
        OpenSubMenu(startMenu);
        blur.SetActive(false);
        GameObject.Find("Version").GetComponent<TextMeshProUGUI>().text = "Ver. " + Application.version;
        
        titleCam = Camera.main;
        Vector3 camGoalPos = titleCam.transform.position;
        Vector3 camStartPos = new Vector3(camGoalPos.x, camGoalPos.y, camGoalPos.z - 30f);
        titleCam.transform.position = camStartPos;
        LeanTween.moveZ(titleCam.gameObject, camGoalPos.z, 0.8f).setEaseOutSine();
        fade.FadeOut(0.5f);
    }

    public void CancelTableButton()
    {
        gameManager.GetComponent<CustomNetworkDiscovery>().StopAdvertising();
        try
        {
            gameManager.StopHost();
            gameManager.StopClient();
        }
        catch(NullReferenceException)
        {
            
        }
    }
    
    public void CancelSearchButton()
    {
        gameManager.GetComponent<CustomNetworkDiscovery>().StopDiscovery();
    }

    public void StartButton()
    {
        gameManager.StartGame();
    }

    public void SelectSfx()
    {
        uiAudio.PlayOneShot(selectSfx);
    }
    
    public void StartSfx()
    {
        uiAudio.PlayOneShot(startGameSfx);
    }

    public void CancelSfx()
    {
        uiAudio.PlayOneShot(cancelSfx);
    }

    public void Connect(DiscoveryResponse info)
    {
        networkDiscovery.StopDiscovery();
        gameManager.StartClient(info.uri);
        OpenSubMenu(gameSetup);
        CloseSubMenu(gameFind);
    }
    
    public IEnumerator BGMFadeOut (float fadeTime)
    {
        float startVolume = uiAudio.volume;
 
        while (uiAudio.volume > 0 && uiAudio != null){
            uiAudio.volume -= startVolume * Time.deltaTime / fadeTime;
 
            yield return null;
        }
 
        uiAudio.Stop ();
        uiAudio.volume = startVolume;
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

    public void ForceReturnToTitle()
    {
        OpenSubMenu(startMenu);
        CloseSubMenu(gameSetup);
    }

    public void ConditionStartHost(Transform subMenu)
    {
        if (PlayerPrefs.GetString("PlayerName") == null || PlayerPrefs.GetString("PlayerName") == "" || PlayerPrefs.GetString("PlayerName").Length > 7)
        {
            OpenSubMenu(settingsMenu);
        }
        else
        {
            gameManager.StartHost();
            networkDiscovery.AdvertiseServer();
            OpenSubMenu(gameSetup);
        }
    }
    
    public void ConditionStartClient(Transform subMenu)
    {
        if (PlayerPrefs.GetString("PlayerName") == null || PlayerPrefs.GetString("PlayerName") == "" || PlayerPrefs.GetString("PlayerName").Length > 7)
        {
            OpenSubMenu(settingsMenu);
        }
        else
        {
            networkDiscovery.StartDiscovery();
            OpenSubMenu(subMenu);
        }
    }

    private IEnumerator SetActiveOff()
    {
        yield return new WaitForSeconds(1f);
    }

    public void CloseSubMenu(Transform subMenu)
    {
        blur.SetActive(false);

        foreach (Transform menuObj in subMenu)
        {
            menuObj.GetComponent<MenuAnimations>().ExitAnim();
            StartCoroutine(SetActiveOff());
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
