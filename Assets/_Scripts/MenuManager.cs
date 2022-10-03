using System.Collections;
using Mirror.Discovery;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public AudioClip selectSfx, cancelSfx;
    private GameObject blur;
    private GameManager gameManager;
    private AudioSource uiAudio;
    private Camera titleCam;
    private Transform startMenu, settingsMenu, gameSetup, gameFind;
    private NetworkDiscovery networkDiscovery;

    public void Start()
    {
        blur = transform.Find("Blur").gameObject;
        FadeInOut fade = GameObject.Find("Fade").GetComponent<FadeInOut>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        networkDiscovery = GameObject.Find("GameManager").GetComponent<NetworkDiscovery>();
        uiAudio = GetComponent<AudioSource>();
        startMenu = transform.Find("Start");
        settingsMenu = transform.Find("Settings");
        gameSetup = transform.Find("GameSetup");
        gameFind = transform.Find("GameSearch");
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
    
    public IEnumerator BGMFadeOut (float fadeTime)
    {
        float startVolume = uiAudio.volume;
 
        while (uiAudio.volume > 0) {
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
        if (PlayerPrefs.GetString("PlayerName") == null || PlayerPrefs.GetString("PlayerName") == "" || PlayerPrefs.GetString("PlayerName").Length > 5)
        {
            OpenSubMenu(settingsMenu);
        }
        else
        {
            gameManager.StartHost();
            //networkDiscovery.AdvertiseServer();
            OpenSubMenu(gameSetup);
        }
    }
    
    public void ConditionStartClient(Transform subMenu)
    {
        if (PlayerPrefs.GetString("PlayerName") == null || PlayerPrefs.GetString("PlayerName") == "" || PlayerPrefs.GetString("PlayerName").Length > 5)
        {
            OpenSubMenu(settingsMenu);
        }
        else
        {
            //networkDiscovery.StartDiscovery();
            gameManager.StartClient();
            OpenSubMenu(subMenu);
        }
    }

    private IEnumerator SetActiveOff(GameObject menuObj)
    {
        yield return new WaitForSeconds(1f);
        //menuObj.SetActive(false);
    }

    public void CloseSubMenu(Transform subMenu)
    {
        blur.SetActive(false);

        foreach (Transform menuObj in subMenu)
        {
            menuObj.GetComponent<MenuAnimations>().ExitAnim();
            StartCoroutine(SetActiveOff(menuObj.gameObject));
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
