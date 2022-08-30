using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class RecommendMenu : MonoBehaviour
{
    private PlayerManager targetPlayer;
    private List<PlayerManager> players = new();
    private StateManager stateManager;
    private ToggleGroup playerToggles;
    void Start()
    {
        targetPlayer = null;
        int playerCount = 0;
        stateManager = GameObject.Find("StateManager").GetComponent<StateManager>();
        playerToggles = transform.Find("Players").GetComponent<ToggleGroup>();
        players.Clear();
        
        foreach (uint playerID in stateManager.activePlayers)
        {
            PlayerManager player = stateManager.spawnedPlayers[playerID];
            if (player.gameObject != stateManager.currentPlayer)
            {
                players.Add(player.GetComponent<PlayerManager>());
                playerToggles.transform.GetChild(playerCount).gameObject.SetActive(true);
                playerToggles.transform.GetChild(playerCount).GetComponentInChildren<TMPro.TextMeshProUGUI>().text = player.name;
                playerCount += 1;
            }
        }
        
        playerToggles.transform.GetChild(4).gameObject.SetActive(true);
    }
    
    public void SelectPlayer()
    {
        Toggle playerToggle = playerToggles.ActiveToggles().FirstOrDefault();
        targetPlayer = players[playerToggle.transform.GetSiblingIndex()];
    }
    
    public void ConfirmRecommend()
    {
        gameObject.GetComponent<SpawnMenu>().SlideOutMenu();
    }
}
