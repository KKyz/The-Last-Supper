using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer bgmMixer, sfxMixer;

    private TMP_Dropdown resolutionDropdown;
    private TMP_InputField nameInput;
    private Slider bgmSlider, sfxSlider;
    private List<Resolution> availableResolutions;
    private GameObject purchaseButton, restoreButton;
    
    void Start()
    {
        nameInput = transform.Find("NameInput").GetComponent<TMP_InputField>();
        nameInput.text = PlayerPrefs.GetString("PlayerName", "");
        resolutionDropdown = transform.Find("Resolutions").GetComponent<TMP_Dropdown>();
        bgmSlider = transform.Find("BGMSlider").GetComponent<Slider>();
        sfxSlider = transform.Find("SFXSlider").GetComponent<Slider>();
        purchaseButton = GameObject.Find("BuyButton");
        restoreButton = GameObject.Find("RestoreButton");
        
        Resolution[] resolutions = Screen.resolutions;
        availableResolutions = new List<Resolution>();
        resolutionDropdown.ClearOptions();
        
        //int currentRefreshRate = Screen.currentResolution.refreshRate;

        for (int i = 0; i < resolutions.Length; i++)
        {
            availableResolutions.Add(resolutions[i]);
        }
        
        
        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = availableResolutions[i].width + "x" + availableResolutions[i].height;
            options.Add(option);

            if (availableResolutions[i].width == Screen.currentResolution.width && availableResolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolPref", 1f);
        bgmSlider.value = PlayerPrefs.GetFloat("BGMVolPref", 0.5f);
        
        //Platform specific compilation for remove ads button
        #if UNITY_EDITOR
        restoreButton.SetActive(true);
        purchaseButton.SetActive(true);
        #elif UNITY_IOS
        restoreButton.SetActive(true);
        purchaseButton.SetActive(true);
        #elif UNITY_ANDROID
        restoreButton.SetActive(true);
        purchaseButton.SetActive(true);
        #else
        restoreButton.SetActive(false);
        purchaseButton.SetActive(false);
        #endif
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = availableResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, true);
        SetFullscreen(true);
    }
    
    public void BgmVolume (float sliderValue)
    {
        bgmMixer.SetFloat("BGMVolume", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("BGMVolPref", Mathf.Log10(sliderValue) * 20);
    }

    public void SfxVolume (float sliderValue)
    {
        sfxMixer.SetFloat("SFXVolume", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("SFXVolPref", Mathf.Log10(sliderValue) * 20);
    }
    
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void DeletePlayerData()
    {
        SaveSystem.ClearPlayer();
        PlayerPrefs.SetString("PlayerName", "");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
