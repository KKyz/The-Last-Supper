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

    public void ShakeCamera(float length)
    {
        camShaker.ShakeOnce(5f, 15f, 0.1f, length); 
    }
}
