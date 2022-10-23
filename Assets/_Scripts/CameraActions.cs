using EZCameraShake;
using Mirror;
using UnityEngine;

public class CameraActions : MonoBehaviour
{
    public Transform playerCam;
    public Vector3 zoomOutPos;
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
        playerCam.LookAt(stateManager.centerPos);
    }

    public void UpdateCameraLook()
    {
        foreach (NetworkIdentity player in stateManager.activePlayers)
        {
            stateManager.centerPos = GameObject.Find("StateManager(Clone)").transform.position;

            Vector3 lookDir = (stateManager.centerPos - player.transform.position) * 0.01f;
            Transform playerModel = player.transform.Find("Model").GetChild(0);
            playerModel.LookAt(player.transform.position + lookDir);

            player.GetComponent<CameraActions>().playerCam.LookAt(stateManager.platePos);
        }
    }

    public void ZoomIn()
    {
        playerCam = transform.Find("Camera");
        Vector3 direction = zoomOutPos - stateManager.platePos;
        Vector3 zoomInPos = zoomOutPos - (direction * 0.4f);
        LeanTween.move(playerCam.gameObject, zoomInPos, 1f).setEaseOutSine();
    }
    
    public void ZoomOut()
    {
        LeanTween.move(playerCam.gameObject, zoomOutPos, 1f).setEaseOutSine();
        UpdateCameraLook();
    }

    public void ShakeCamera(float length)
    {
        camShaker.ShakeOnce(5f, 15f, 0.1f, length);
    }
}
