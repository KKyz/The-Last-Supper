using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public TMP_Text[] playerNames = new TMP_Text[4];
    public Toggle[] playerReadyToggles = new Toggle[4];
    public Button startGameButton;
    public Toggle tagToggle;
    public Transform setupButtons, tableSetup, customizeButton, guestsList;

    private GameManager gameManager;
    private TMP_Dropdown restaurantDropdown;
    private TMP_Dropdown menuDropdown;
    private TMP_Text estTimeText;
    private Image restaurantThumbnail;

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
            restaurantNames.Add(restaurant.name);
        }

        restaurantDropdown.AddOptions(restaurantNames);
        SelectRestaurant(0);
        ToggleTagTournament(false);
    }

    private void CheckMenuConditions(RestaurantContents restaurant, List<String> menuNames)
    {
        foreach (var menu in restaurant.menus)
        {
            if (menu.condition == RestaurantContents.DataEnum.GamesWon)
            {
                if (gameManager.gamesWon >= menu.conditionVal)
                {
                    menuNames.Add(menu.menuName);
                    restaurant.availableMenus.Add(menu);
                }
            }
            else if (menu.condition == RestaurantContents.DataEnum.GamesLost)
            {
                if (gameManager.gamesJoined - gameManager.gamesWon >= menu.conditionVal)
                {
                    menuNames.Add(menu.menuName);
                    restaurant.availableMenus.Add(menu);
                }
            }
            else if (menu.condition == RestaurantContents.DataEnum.GamesJoined)
            {
                if (gameManager.gamesJoined >= menu.conditionVal)
                {
                    menuNames.Add(menu.menuName);
                    restaurant.availableMenus.Add(menu);
                }
            }
            else if (menu.condition == RestaurantContents.DataEnum.TeamGamesWon)
            {
                if (gameManager.teamGamesWon >= menu.conditionVal)
                {
                    menuNames.Add(menu.menuName);
                    restaurant.availableMenus.Add(menu);
                }
            }
            else if (menu.condition == RestaurantContents.DataEnum.None)
            {
                menuNames.Add(menu.menuName);
                restaurant.availableMenus.Add(menu);
            }
        }
    }

    public void ToggleTagTournament(bool toggle)
    {
        gameManager.tagTournament = toggle;
        
        int counter = 0;

        if (toggle)
        {
            foreach (var player in gameManager.roomPlayers)
            {
                if (counter < 1)
                {
                    gameManager.team1.Add(player.netIdentity);
                    counter++;
                }

                else
                {
                    gameManager.team2.Add(player.netIdentity);
                }
            }
            
            foreach (Transform guest in guestsList)
            {
                guest.Find("TeamColor").gameObject.SetActive(true);
            }
            
            UpdatePlayerTeams();
        }

        else
        {
            gameManager.team1.Clear();
            gameManager.team2.Clear();

            foreach (Transform guest in guestsList)
            {
                guest.Find("TeamColor").gameObject.SetActive(false);
            }

            tagToggle.isOn = false;
        }
    }

    public void ChangePlayerTeams(int index)
    {
        if (gameManager.tagTournament)
        {
            NetworkIdentity playerID = gameManager.roomPlayers[index].netIdentity;
        
            if (gameManager.team1.Contains(playerID))
            {
                gameManager.team1.Remove(playerID);
                gameManager.team2.Add(playerID);
                Debug.LogWarning("Swapped to team 2: " + playerID);
            }
            else
            {
                gameManager.team2.Remove(playerID);
                gameManager.team1.Add(playerID); 
                Debug.LogWarning("Swapped to team 1: " + playerID);
            }

            if (gameManager.team2.Count > 2)
            {
                startGameButton.interactable = false;
            }
            else
            {
                startGameButton.interactable = true;
            }
        
            UpdatePlayerTeams();
        }
    }

    private void UpdatePlayerTeams()
    {
        if (gameManager.tagTournament)
        {
            foreach (var player in gameManager.team1)
            {
                Button TeamToggle = guestsList.GetChild(gameManager.team1.IndexOf(player)).GetComponentInChildren<Button>();
                TeamToggle.gameObject.SetActive(true);
                TeamToggle.image.color = new Color(255, 0, 0);
            }
        
            foreach (var player in gameManager.team2)
            {
                Button TeamToggle = guestsList.GetChild(gameManager.team2.IndexOf(player)).GetComponentInChildren<Button>();
                TeamToggle.gameObject.SetActive(true);
                TeamToggle.image.color = new Color(0, 0, 255);
            }
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
        CheckMenuConditions(currentRestaurant, menuNames);
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
