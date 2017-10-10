using UnityEngine;
using UnityEngine.UI;

public class SoundToggle : MonoBehaviour {

    public Sprite soundOn, soundOff;

    void Start() {
        if (gameObject.name == "FXToggle") {
            ChooseSprite(GameManager.Player.sound);
        } else {
            ChooseSprite(GameManager.Player.music);
        }
    }

    public void Toggle() {
        if (gameObject.name == "FXToggle") {
            GameManager.Player.sound = !GameManager.Player.sound;
            ChooseSprite(GameManager.Player.sound);
        } else {
            GameManager.Player.music = !GameManager.Player.music;
            ChooseSprite(GameManager.Player.music);
            MusicManager.SetMusic();
        }

        PlayerClass.SaveProfile(GameManager.Player);
    }

    void ChooseSprite (bool Choose) {
        if (Choose) {
            transform.Find("InnerIcon").GetComponent<Image>().sprite = soundOn;
        } else {
            transform.Find("InnerIcon").GetComponent<Image>().sprite = soundOff;
        }
    }
}
