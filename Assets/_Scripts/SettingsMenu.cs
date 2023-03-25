using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer bgmMixer, sfxMixer;

    private TMP_Dropdown resolutionDropdown;
    [HideInInspector] public GameManager gameManager;
    public TMP_Dropdown languageDropdown;
    private TMP_InputField nameInput;
    private Slider bgmSlider, sfxSlider;
    private List<Resolution> availableResolutions;
    private GameObject purchaseButton, restoreButton, fullScreenButton;

    void Start()
    {
        StartCoroutine(InitLocalization());
        
        nameInput = transform.Find("NameInput").GetComponent<TMP_InputField>();
        nameInput.text = PlayerPrefs.GetString("PlayerName", "");
        resolutionDropdown = transform.Find("Resolutions").GetComponent<TMP_Dropdown>();
        bgmSlider = transform.Find("BGMSlider").GetComponent<Slider>();
        sfxSlider = transform.Find("SFXSlider").GetComponent<Slider>();
        purchaseButton = GameObject.Find("BuyButton");
        restoreButton = GameObject.Find("RestoreButton");
        fullScreenButton = transform.Find("FSToggle").gameObject;

        Resolution[] resolutions = Screen.resolutions;
        availableResolutions = new List<Resolution>();
        resolutionDropdown.ClearOptions();

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
        fullScreenButton.SetActive(true);
        resolutionDropdown.gameObject.SetActive(true);
        #elif UNITY_IOS
        restoreButton.SetActive(true);
        purchaseButton.SetActive(true);
        fullScreenButton.SetActive(false);
        resolutionDropdown.gameObject.SetActive(false);
        #elif UNITY_ANDROID
        restoreButton.SetActive(true);
        purchaseButton.SetActive(true);
        fullScreenButton.SetActive(false);
        resolutionDropdown.gameObject.SetActive(false);
        #elif UNITY_STANDALONE
        restoreButton.SetActive(false);
        purchaseButton.SetActive(false);
        fullScreenButton.SetActive(true);
        resolutionDropdown.gameObject.SetActive(true);
        #endif
    }

    void RemoveAds()
    {
        PlayerPrefs.SetInt("Ads", 0);
    }


    private IEnumerator InitLocalization()
    {
        yield return LocalizationSettings.InitializationOperation;

        // Generate list of available Locales
        var options = new List<TMP_Dropdown.OptionData>();
        int selected = 0;
        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
        {
            var locale = LocalizationSettings.AvailableLocales.Locales[i];
            if (LocalizationSettings.SelectedLocale == locale)
                selected = i;
            options.Add(new TMP_Dropdown.OptionData(locale.name));
        }
        languageDropdown.options = options;

        languageDropdown.value = selected;
    }

    public void LocaleSelected(int index)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        Debug.LogWarning(LocalizationSettings.SelectedLocale);
        
        //Change fonts for special cases
        if (gameManager != null)
        {
            gameManager.ChangeFont(); 
        }
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
