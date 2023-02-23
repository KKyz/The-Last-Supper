using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public TMP_Text[] playerNames = new TMP_Text[4];
    public Toggle[] playerReadyToggles = new Toggle[4];
    public Button startGameButton;
    public Transform setupButtons, tableSetup, customizeButton;

    private GameManager gameManager;
    private TMP_Dropdown restaurantDropdown;
    private TMP_Dropdown menuDropdown;
    private TMP_Text estTimeText;
    private Image restaurantThumbnail;
    private List<PlayerLobby> team1, team2 = new();

    public void Init()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        tableSetup = transform.Find("TableSetup");
        customizeButton = transform.Find("CustomizeBtn");
        restaurantDropdown = tableSetup.Find("RestaurantDropdown").GetComponent<TMP_Dropdown>();
        menuDropdown = tableSetup.Find("MenuDropdown").GetComponent<TMP_Dropdown>();
        estTimeText = tableSetup.Find("EstTimeText").GetComponent<TMP_Text>();
        restaurantThumbnail = tableSetup.Find("Thumbnail").GetComponent<Image>();

        estTimeText.text = "Est. Play Time: 0 Mins. Per Session";

        //Add restaurants and menus
        restaurantDropdown.ClearOptions();
        menuDropdown.ClearOptions();
        List<string> restaurantNames = new List<string>();
        foreach (var restaurant in gameManager.restaurants)
        {
            restaurantNames.Add(restaurant.restaurantName);
        }

        restaurantDropdown.AddOptions(restaurantNames);
        SelectRestaurant(0);
    }

    public void ToggleTeamMode(bool toggle)
    {
        gameManager.teamGame = toggle;
        
        int team1Length = (int)(gameManager.roomPlayers.Count / 2);
        int team2Length = gameManager.roomPlayers.Count - team1Length;

        if (toggle)
        {
            for (int i = 0; i < team1Length; i++)
            {
                team1.Add(gameManager.roomPlayers[i]);
            }

            if (gameManager.roomPlayers.Count > 1)
            {
                for (int i = team1Length; i < team2Length; i++)
                {
                   team2.Add(gameManager.roomPlayers[i]);
                }
            }
        }

        else
        {
            gameManager.team1.Clear();
            gameManager.team2.Clear();
        }
        
        UpdatePlayerTeams();
    }

    public void ChangePlayerTeams(int index)
    {
    }

    public void UpdatePlayerTeams()
    {
        for (int i = 0; i < gameManager.team1.Count; i++)
        {
            Button TeamToggle = playerNames[i].transform.Find("TeamColour").GetComponent<Button>();
            TeamToggle.gameObject.SetActive(true);
            TeamToggle.image.color = Color.red;
        }
        
        for (int i = 0; i < gameManager.team2.Count; i++)
        {
            Button TeamToggle = playerNames[i].transform.Find("TeamColour").GetComponent<Button>();
            TeamToggle.gameObject.SetActive(true);
            TeamToggle.image.color = Color.blue;
        }
    }

    public void SelectRestaurant(int index)
    {
        gameManager.currentRestaurant = gameManager.restaurants[index].gameObject;
        UpdateMenus();
    }

    public void SelectMenu(int index)
    {
        gameManager.currentMenu = index;
    }

    private void UpdateMenus()
    {
        RestaurantContents currentRestaurant = gameManager.currentRestaurant.GetComponent<RestaurantContents>(); 
        List<string> menuNames = new List<string>();

        foreach (var menu in currentRestaurant.menus)
        {
            menuNames.Add(menu.menuName);
        }
        menuDropdown.ClearOptions();
        menuDropdown.AddOptions(menuNames);
        
        estTimeText.text = "Est. Play Time: " + currentRestaurant.estPlayTime + " Mins. Per Session";

        if (currentRestaurant.thumbnail != null)
        {
            restaurantThumbnail.sprite = currentRestaurant.thumbnail; 
        }
    }

    public void DisableSetupButtons()
    {
        foreach (Transform button in setupButtons)
        {
            button.GetComponent<Button>().interactable = false;
        }

        restaurantDropdown.GetComponent<TMP_Dropdown>().interactable = false;
        menuDropdown.gameObject.GetComponent<TMP_Dropdown>().interactable = false;
        customizeButton.GetComponent<Button>().interactable = false;
    }

    public void UpdateDisplay()
    {
        for (int i = 0; i < playerNames.Length; i++)
        {
            playerNames[i].text = "Waiting...";
            playerReadyToggles[i].isOn = false;
        }

        for (int i = 0; i < gameManager.roomPlayers.Count; i++)
        {
            playerNames[i].text = gameManager.roomPlayers[i].displayName;
            playerReadyToggles[i].isOn = gameManager.roomPlayers[i].isReady;
        }
    }

    public void ReadyUp()
    {
        gameManager.localRoomPlayer.CmdReadyUp();
    }
}
