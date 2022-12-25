using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager: MonoBehaviour
{
    public TMP_Text[] playerNames = new TMP_Text[4];
    public Toggle[] playerReadyToggles = new Toggle[4];
    public Button startGameButton;
    public Transform setupButtons, tableSetup;
    
    private GameManager room;
    private TMP_Dropdown restaurantDropdown;
    private TMP_Dropdown menuDropdown;

    public void Init()
    {
        room = GameObject.Find("GameManager").GetComponent<GameManager>();
        tableSetup = transform.Find("TableSetup");
        restaurantDropdown = tableSetup.Find("RestaurantDropdown").GetComponent<TMP_Dropdown>();
        menuDropdown = tableSetup.Find("MenuDropdown").GetComponent<TMP_Dropdown>();
        
        //Add restaurants and menus
        restaurantDropdown.ClearOptions();
        menuDropdown.ClearOptions();
        List<string> restaurantNames = new List<string>();
        foreach (var restaurant in room.restaurants)
        {
            restaurantNames.Add(restaurant.name);
        }
        
        restaurantDropdown.AddOptions(restaurantNames);
        SelectRestaurant(0);
    }

    public void SelectRestaurant(int index)
    {
        room.currentRestaurant = room.restaurants[index].gameObject;
        UpdateMenus();
    }

    public void SelectMenu(int index)
    {
        room.currentMenu = index;
    }

    private void UpdateMenus()
    {
        List<string> menuNames = new List<string>();

        foreach (var menu in room.currentRestaurant.GetComponent<RestaurantContents>().menus)
        {
            menuNames.Add(menu.menuName);
        }
        
        menuDropdown.AddOptions(menuNames);
    }

    public void DisableSetupButtons()
    {
        foreach (Transform button in setupButtons)
        {
            button.GetComponent<Button>().interactable = false;
        }
    }

    public void UpdateDisplay()
    {
        for (int i = 0; i < playerNames.Length; i++)
        {
            playerNames[i].text = "Waiting...";
            playerReadyToggles[i].isOn = false;
        }

        for (int i = 0; i < room.roomPlayers.Count; i++)
        {
            playerNames[i].text = room.roomPlayers[i].displayName;
            playerReadyToggles[i].isOn = room.roomPlayers[i].isReady;
        }
    }

    public void ReadyUp()
    {
        room.localRoomPlayer.CmdReadyUp();
    }
}
