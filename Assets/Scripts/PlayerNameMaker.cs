using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerNameMaker : MonoBehaviour
{
    public TMP_InputField newPlayerName;

    public Button confirmButton;
    private GameObject warningMessage;

    public void Awake()
    {
        warningMessage = confirmButton.transform.Find("Warning").gameObject;
    }
    void Update()
    {
        if (newPlayerName.text.Length > 0 && newPlayerName.text.Length <= 5)
        {
            confirmButton.interactable = true;
            warningMessage.SetActive(false);
        }
        else
        {
            confirmButton.interactable = false;
            warningMessage.SetActive(true);
        }
    }

    public void UpdateName()
    {
        PlayerPrefs.SetString("PlayerName", newPlayerName.text);
    }
}
