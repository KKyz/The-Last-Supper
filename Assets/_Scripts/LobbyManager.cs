using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager: MonoBehaviour
{
    public TMP_Text[] playerNames = new TMP_Text[4];
    public Toggle[] playerReadyToggles = new Toggle[4];
    public Button startGameButton;
    public Transform setupButtons;
    
    private GameManager room;

    public void Start()
    {
        room = GameObject.Find("GameManager").GetComponent<GameManager>();
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
