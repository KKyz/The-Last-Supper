using Mirror;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.MultiplayerAgent.Helpers;
using PlayFab.MultiplayerModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientStartup : MonoBehaviour
{
    
}
/*
#if !UNITY_SERVER
    protected GameManager room;
    protected GameManager Room {
        get {
            if (room != null) { return room; }
            return room = (GameManager)NetworkManager.singleton;
        }
    }

    void Start () {
        if (Room.Config.buildType == BuildType.LOCAL_CLIENT || Room.Config.buildType == BuildType.REMOTE_CLIENT) {
            PlayFabAuthService.OnLoginSuccess += OnLoginSuccess;

            GameManager.OnClientDisconnected += OnDisconnected;
            GameManager.OnClientConnected += OnConnected;

            NetworkClient.RegisterHandler<ShutdownMessage> (OnServerShutDown);
            NetworkClient.RegisterHandler<MaintenanceMessage> (OnMaintenanceMessage);
        }
    }

    private void OnMaintenanceMessage (MaintenanceMessage message) {
        Debug.Log ("Maintenance Shutdown scheduled");
        Debug.Log ($"Maintenance is scheduled for: {message.ScheduledMaintenanceUTC.ToString ("MM-DD-YYYY hh:mm:ss")}");
    }

    private void OnServerShutDown (ShutdownMessage message) {
        Debug.Log ("Shutdown In Progress");
        Debug.Log ("Server has issued a shutdown.");
        NetworkClient.Disconnect ();
    }

    private void OnConnected (NetworkConnection conn) {
        Debug.Log ("Connected!");
        Debug.Log ("You are connected to the server");

        NetworkClient.connection.Send (new ReceiveAuthenticateMessage () {
            PlayFabId = PlayFabAuthService.PlayFabId
        });
    }

    private void OnDisconnected (NetworkConnection conn) {
        Debug.Log ("Disconnected!");
        Debug.Log ("You were disconnected from the server");
    }

    private void OnLoginSuccess (LoginResult success) {
        Debug.Log ("Login Successful");
        Debug.Log ($"You logged in successfully. ID: {success.PlayFabId}");

        if (string.IsNullOrWhiteSpace (Room.Config.ipAddress)) {
            //We need to grab an IP and Port from a server based on the buildId. Copy this and add it to your Configuration.
            RequestMultiplayerServer ();
        } else {
            ConnectRemoteClient ();
        }
    }

    private void ConnectRemoteClient (RequestMultiplayerServerResponse response = null) {
        if (response == null) {
            Room.networkAddress = Room.Config.ipAddress;
            Room.Transport.Port = Room.Config.port;
        } else {
            Debug.Log ("**** ADD THIS TO YOUR CONFIGURATION **** -- IP: " + response.IPV4Address + " Port: " + (ushort)response.Ports[0].Num);
            Room.networkAddress = response.IPV4Address;
            Room.Transport.Port = (ushort)response.Ports[0].Num;
        }

        Room.StartClient ();
    }

    private void RequestMultiplayerServer () {
        Debug.Log ("[ClientStartUp].RequestMultiplayerServer");
        RequestMultiplayerServerRequest requestData = new RequestMultiplayerServerRequest ();
        requestData.BuildId = Room.Config.buildId;
        requestData.SessionId = System.Guid.NewGuid ().ToString ();
        requestData.PreferredRegions = new List<string> () { "EastUs" };

        PlayFabMultiplayerAPI.RequestMultiplayerServer (requestData, OnRequestMultiplayerServer, OnRequestMultiplayerServerError);
    }

    private void OnRequestMultiplayerServer (RequestMultiplayerServerResponse response) {
        Debug.Log (response.ToString ());
        ConnectRemoteClient (response);
    }

    private void OnRequestMultiplayerServerError (PlayFabError error) {
        Debug.Log (error.ErrorMessage);
        Debug.Log (error.ErrorDetails);
    }

#endif
}*/