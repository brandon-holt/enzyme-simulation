using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveScores(World score, string fileExtension)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + fileExtension;
        FileStream stream = new FileStream(path, FileMode.Create);

        WorldData data = new WorldData(score);

        formatter.Serialize(stream, data);

        stream.Close();
    }

    public static WorldData LoadScores(string path)
    {
        //string path = Application.persistentDataPath + fileExtension;

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            WorldData data = formatter.Deserialize(stream) as WorldData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.Log("Save file not found in " + path);
            return null;
        }
    }

}