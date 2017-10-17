using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using UnityEngine;

[Serializable]
public class AppSettings {

    public bool Music, SFX, internetCheck;

    const string fileName = "/AppSettings.bin";

    public AppSettings() {
        Music = true;
        SFX = true;
        internetCheck = false;
    }

    public static void SaveSettings(AppSettings ProfilePlayer) {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + fileName);
        bf.Serialize(file, ProfilePlayer);
        file.Close();
    }

    public static AppSettings LoadSettings() {
        if (File.Exists(Application.persistentDataPath + fileName)) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + fileName, FileMode.Open);
            AppSettings data;

            try {
                data = (AppSettings)bf.Deserialize(file);
                file.Close();
            } catch (Exception) {
                file.Close();
                File.Delete(Application.persistentDataPath + fileName);
                data = new AppSettings();
                SaveSettings(data);
            }

            return data;
        }
        return new AppSettings();
    }

}
