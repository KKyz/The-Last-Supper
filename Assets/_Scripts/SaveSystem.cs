using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SavePlayer(GameManager player)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.kay";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(player);
        
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData LoadPlayer()
    {
        string path = Application.persistentDataPath + "/player.kay";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            return data;
        }
        
        return null;
    }

    public static void ClearPlayer()
    {
        string path = Application.persistentDataPath + "/player.kay";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public static void SaveCheck(GameManager player)
    {
        string path = Application.persistentDataPath + "/player.kay";

        if (!File.Exists(path))
        {
            SavePlayer(player);
        }
    }
}
