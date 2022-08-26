using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    Resolution[] resolutions;

    private TMP_Dropdown resolutionDropdown;
    private TMP_InputField nameInput;
    //public Toggle minScrollBtn;

    void Start()
    {
        nameInput = GameObject.Find("NameInput").GetComponent<TMP_InputField>();
        nameInput.text = PlayerPrefs.GetString("PlayerName");
        resolutionDropdown = GameObject.Find("Resolutions").GetComponent<TMP_Dropdown>();
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

    }

    public void SaveUsername()
    {
        PlayerPrefs.SetString("PlayerName", nameInput.text);
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
    }
}
