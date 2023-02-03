using UnityEngine;

public class DiscoveryButton : MonoBehaviour
{
    private float discoveryTimer;
    private float discoveryTimerMax;

    private void Start()
    {
        discoveryTimer = 0f;
        discoveryTimerMax = 3f;
    }

    private void Update()
    {
        discoveryTimer += Time.deltaTime;

        if (discoveryTimer > discoveryTimerMax)
        {
            gameObject.SetActive(false);
            ResetTimer();
        }
    }
    
    public void ResetTimer()
    {
        discoveryTimer = 0; 
    }

}

    