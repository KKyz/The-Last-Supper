using EZCameraShake;
using Mirror;
using UnityEngine;

public class CameraActions : MonoBehaviour
{
    private StateManager stateManager;
    private CameraShaker camShaker;

    public void OnStartGame()
    {
        stateManager = GameObject.Find("StateManager(Clone)").GetComponent<StateManager>();
        camShaker = GetComponent<CameraShaker>();
    }

    public void FaceCenter()
    {
        foreach (NetworkIdentity player in stateManager.activePlayers)
        {
            stateManager.centerPos = GameObject.Find("StateManager(Clone)").transform.position;

            Vector3 lookDir = (stateManager.centerPos - player.transform.position) * 0.01f;
            Transform playerModel = player.transform.Find("Model").GetChild(0);
            playerModel.LookAt(player.transform.position + lookDir);
        }
    }

    public void ShakeCamera(float length)
    {
        camShaker.ShakeOnce(5f, 15f, 0.1f, length);
    }
}
