using Mirror;
using PlayFab;
using System;

public struct ReceiveAuthenticateMessage : NetworkMessage {
    public string PlayFabId;
}

public struct ShutdownMessage : NetworkMessage { }

[Serializable]
public struct MaintenanceMessage : NetworkMessage {
    public DateTime ScheduledMaintenanceUTC;
}

public static class MaintenanceMessageFunctions {

    public static MaintenanceMessage Deserialize (this NetworkReader reader) {
        var json = PlayFab.PluginManager.GetPlugin<ISerializerPlugin> (PluginContract.PlayFab_Serializer);
        var scheduledMaintenanceUTC = json.DeserializeObject<DateTime> (reader.ReadString ());

        return new MaintenanceMessage () { ScheduledMaintenanceUTC = scheduledMaintenanceUTC };
    }

    public static void Serialize (this NetworkWriter writer, MaintenanceMessage message) {
        var json = PlayFab.PluginManager.GetPlugin<ISerializerPlugin> (PluginContract.PlayFab_Serializer);
        var str = json.SerializeObject (message.ScheduledMaintenanceUTC);
        writer.WriteString (str);
    }
}