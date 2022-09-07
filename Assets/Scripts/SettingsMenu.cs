using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer bgmMixer, sfxMixer;
    
    Resolution[] resolutions;

    private TMP_Dropdown resolutionDropdown;
    private TMP_InputField nameInput;

    private Slider bgmSlider, sfxSlider;
    //public Toggle minScrollBtn;

    void Start()
    {
        nameInput = GameObject.Find("NameInput").GetComponent<TMP_InputField>();
        nameInput.text = PlayerPrefs.GetString("PlayerName");
        resolutionDropdown = GameObject.Find("Resolutions").GetComponent<TMP_Dropdown>();
        bgmSlider = GameObject.Find("BGMSlider").GetComponent<Slider>();
        sfxSlider = GameObject.Find("SFXSlider").GetComponent<Slider>();
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolPref", 1f);
        bgmSlider.value = PlayerPrefs.GetFloat("BGMVolPref", 0.5f);
    }
    
    public void BgmVolume (float sliderValue)
    {
        bgmMixer.SetFloat("BGMVolume", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("BGMVolPref", sliderValue);
    }

    public void SfxVolume (float sliderValue)
    {
        sfxMixer.SetFloat("SFXVolume", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("SFXVolPref", sliderValue);
    }
    
    public void ToggleMinScroll()
    {
        // if (minScrollBtn.isOn)
        // {PlayerPrefs.SetInt("MinScrollInfo", 1);}
        // else
        // {PlayerPrefs.SetInt("MinScrollInfo", 0);}
        // Debug.Log(PlayerPrefs.GetInt("MinScrollInfo"));.
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
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
