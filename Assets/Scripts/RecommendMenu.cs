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
    private Transform toggles;
    private ToggleGroup playerToggles;
    void Start()
    {
        targetPlayer = null;
        int playerCount = 0;
        stateManager = GameObject.Find("StateManager").GetComponent<StateManager>();
        toggles = transform.Find("Players");
        playerToggles = toggles.GetComponent<ToggleGroup>();
        players.Clear();
        
        foreach (uint playerID in stateManager.activePlayers)
        {
            PlayerManager player = stateManager.spawnedPlayers[playerID];
            if (player.gameObject != stateManager.currentPlayer)
            {
                players.Add(player.GetComponent<PlayerManager>());
                toggles.GetChild(playerCount).gameObject.SetActive(true);
                toggles.GetChild(playerCount).GetComponentInChildren<TMPro.TextMeshProUGUI>().text = player.name;
                playerCount += 1;
            }
        }
        
        toggles.GetChild(4).gameObject.SetActive(true);
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
