using System.Collections;
using UnityEngine.Advertisements;
using System.Collections.Generic;
using UnityEngine;

public class DisplayAd : MonoBehaviour
{
    public string myGameIdAndroid = "5203299";
    public string myGameIdIOS = "5203298";
    public string adUnitIdAndroid = "Interstitial_Android";
    public string adUnitIdIOS = "Interstitial_iOS";
    public string myAdUnitId; 
    public bool adStarted;
    private bool testMode = true;
    
    void Start()
    {
	    #if UNITY_IOS
	            Advertisement.Initialize(myGameIdIOS, testMode);
	            myAdUnitId = adUnitIdIOS;
	    #elif UNITY_ANDROID
		        Advertisement.Initialize(myGameIdAndroid, testMode);
		        myAdUnitId = adUnitIdAndroid;
	    #endif 
    }
    
    void Update()
    {
	    if (Advertisement.isInitialized &&  !adStarted)
	    {
		    Advertisement.Load(myAdUnitId);
		    Advertisement.Show(myAdUnitId);
		    adStarted = true;
	    }
    }
}
