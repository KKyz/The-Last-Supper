using EZCameraShake;
using Mirror;
using UnityEngine;

public class CameraActions : MonoBehaviour
{
    private StateManager stateManager;
    private CameraShaker camShaker;

    public void OnStartGame()
    {
        camShaker = GetComponent<CameraShaker>();
        stateManager = GameObject.Find("StateManager(Clone)").GetComponent<StateManager>();

        //Player model should be facing the center
        Vector3 lookDir = (stateManager.centerPos - transform.position) * 0.01f;
        Transform playerModel = transform.Find("Model").GetChild(0);
        playerModel.LookAt(transform.position +  lookDir);
        
        //Camera is looking ath the food more directly, so it should transform everything
        transform.Find("Camera").LookAt(stateManager.centerPos);
    }

    public void UpdatePlayerLook()
    {
        foreach (NetworkIdentity player in stateManager.activePlayers)
        {
            stateManager.centerPos = GameObject.Find("StateManager(Clone)").transform.position;

            Vector3 lookDir = (stateManager.centerPos - player.transform.position) * 0.01f;
            Transform playerModel = player.transform.Find("Model").GetChild(0);
            playerModel.LookAt(player.transform.position + lookDir);

            player.transform.Find("Camera").LookAt(stateManager.platePos);
        }
    }

    public void ShakeCamera(float length)
    {
        camShaker.ShakeOnce(5f, 15f, 0.1f, length);
    }
}
