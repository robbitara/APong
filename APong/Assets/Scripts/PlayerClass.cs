using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using UnityEngine;

[Serializable]
public class PlayerClass {

    const string fileName = "/Profile.bin";

    private int _gamesPlayed;
    private int _gifts;

    public int highscore, skinID;
    public string SkinType;
    public DateTime saveDate;

    public int gifts {
        get {
            return _gifts;
        }

        set {
            if (_gamesPlayed == 5) {
                _gifts = value;
            }
        }
    }

    public int gamesPlayed {
        get {
            return _gamesPlayed;
        }
        set {
            if (_gamesPlayed >= 0 && _gamesPlayed < 5) {
                _gamesPlayed = value;
            }
        }
    }

    // Metodo costruttore di default

    public PlayerClass() {
        highscore = 0;
        skinID = 0;
        _gamesPlayed = 0;
        _gifts = 1;
        SkinType = "RandomSkin";
    }

    public void ResetGamesPlayed() {
        if (saveDate.Day != DateTime.Now.Day) {
            _gamesPlayed = 0;
        }

        if (_gamesPlayed == 5) {
            _gamesPlayed = -1;
        }
    }

    public bool Rewarded() {
        if (gamesPlayed == -1) {
            return true;
        }
        return false;
    }

    public static void SaveProfile(PlayerClass Player, bool SavesDate) {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + fileName);
        if (SavesDate) {
            Player.saveDate = DateTime.Now;
        }
        bf.Serialize(file, Player);
        file.Close();
    }

    public static PlayerClass LoadProfile() {
        if (File.Exists(Application.persistentDataPath + fileName)) {
            PlayerClass data;
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + fileName, FileMode.Open);

            try {
                data = (PlayerClass)bf.Deserialize(file);
                file.Close();
            }
            catch (Exception) {
                file.Close();
                File.Delete(Application.persistentDataPath + fileName);
                data = new PlayerClass();
                SaveProfile(data, true);
            }

            return data;
        }
        return new PlayerClass();
    }

}