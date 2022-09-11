using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : NetworkBehaviour
{
    public GameObject tableSetUp;
    public TMP_Text[] playerNames = new TMP_Text[4];
    public Toggle[] playerReadyToggles = new Toggle[4];
    public Button startGameButton;
    
    private GameManager room;

    public void Start()
    {
        room = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        if (isServer)
        {
            tableSetUp.SetActive(true);
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

    [Command(requiresAuthority = false)]
    public void CmdReadyUp()
    { 
        room.localRoomPlayer.isReady = !room.localRoomPlayer.isReady;
        
        room.UpdateReadyState();
    }
}
