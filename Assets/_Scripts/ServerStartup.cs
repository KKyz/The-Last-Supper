
using System.Collections;
using UnityEngine;
using PlayFab;
using System;
using System.Collections.Generic;
using PlayFab.MultiplayerAgent.Model;
using Mirror;

public class ServerStartup : MonoBehaviour
{
}
/*
#if UNITY_SERVER || UNITY_EDITOR
    [SerializeField] float _serverShutdownTimer = 500f;
    [SerializeField] bool _debugging = true;

    private List<ConnectedPlayer> _connectedPlayers;

    protected GameManager _room;
    protected GameManager Room {
        get {
            if (_room != null) { return _room; }
            return _room = (GameManager)NetworkManager.singleton;
      }
    }

    private void Start () {
        if (Room.Config.buildType == BuildType.REMOTE_SERVER) {
            StartRemoteServer ();
        }
    }

    private void OnDisable () {
        if (Room.Config.buildType == BuildType.REMOTE_SERVER) {
            CleanUpRemoteServer ();
        }
    }

    public void StartLocalServer () {
        if (Room.Config.buildType == BuildType.LOCAL_SERVER) {
            Room.StartServer ();
        }
    }

    public void StartRemoteServer () {
        _connectedPlayers = new List<ConnectedPlayer> ();
        PlayFabMultiplayerAgentAPI.Start ();
        PlayFabMultiplayerAgentAPI.IsDebugging = _debugging;
        PlayFabMultiplayerAgentAPI.OnMaintenanceCallback += OnMaintenance;
        PlayFabMultiplayerAgentAPI.OnShutDownCallback += OnShutdown;
        PlayFabMultiplayerAgentAPI.OnServerActiveCallback += OnServerActive;
        PlayFabMultiplayerAgentAPI.OnAgentErrorCallback += OnAgentError;

        Room.OnPlayerAdded += OnPlayerAdded;
        Room.OnPlayerRemoved += OnPlayerRemoved;

        StartCoroutine (ReadyForPlayers ());
        StartCoroutine (ShutdownServerInXTime (_serverShutdownTimer));
    }

    private void CleanUpRemoteServer () {
        Room.OnPlayerAdded -= OnPlayerAdded;
        Room.OnPlayerRemoved -= OnPlayerRemoved;
    }

    IEnumerator ReadyForPlayers () {
        yield return new WaitForSeconds (.5f);
        PlayFabMultiplayerAgentAPI.ReadyForPlayers ();
    }

    private void OnServerActive () {
        Room.StartServer ();
        Debug.Log ("Server Started From Agent Activation");
    }

    private void OnPlayerRemoved (string playfabId) {
        ConnectedPlayer player = _connectedPlayers.Find (x => x.PlayerId.Equals (playfabId, StringComparison.OrdinalIgnoreCase));
        _connectedPlayers.Remove (player);
        PlayFabMultiplayerAgentAPI.UpdateConnectedPlayers (_connectedPlayers);

        CheckPlayerCountToShutdown ();
    }

    private void OnPlayerAdded (string playfabId) {
        _connectedPlayers.Add (new ConnectedPlayer (playfabId));
        PlayFabMultiplayerAgentAPI.UpdateConnectedPlayers (_connectedPlayers);
    }

    private void OnAgentError (string error) {
        Debug.Log (error);
    }

    private void OnMaintenance (DateTime? NextScheduledMaintenanceUtc) {
        Debug.LogFormat ("Maintenance scheduled for: {0}", NextScheduledMaintenanceUtc.Value.ToLongDateString ());
        foreach (var conn in Room.Connections) {
            conn.Connection.Send (new MaintenanceMessage () {
                ScheduledMaintenanceUTC = (DateTime)NextScheduledMaintenanceUtc
            });
        }
    }

    private void OnShutdown () {
        StartShutdownProcess ();
    }

    private void CheckPlayerCountToShutdown () {
        if (_connectedPlayers.Count <= 0) {
            StartShutdownProcess ();
        }
    }

    private void StartShutdownProcess () {
        Debug.Log ("Server is shutting down");
        foreach (var conn in Room.Connections) {
            conn.Connection.Send (new ShutdownMessage ());
        }
        StartCoroutine (Shutdown ());
    }

    IEnumerator Shutdown () {
        yield return new WaitForSeconds (5f);
        Application.Quit ();
    }

    IEnumerator ShutdownServerInXTime (float time = 300f) {
        yield return new WaitForSeconds (time);
        StartShutdownProcess ();
    }

#endif
}*/