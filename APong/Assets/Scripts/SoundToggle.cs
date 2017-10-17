using UnityEngine;
using UnityEngine.UI;

public class SoundToggle : MonoBehaviour {

    public Sprite soundOn, soundOff;

    void Start() {
        if (gameObject.name == "FXToggle") {
            ChooseSprite(GameManager.GameSettings.SFX);
        } else {
            ChooseSprite(GameManager.GameSettings.Music);
        }
    }

    public void Toggle() {
        if (gameObject.name == "FXToggle") {
            GameManager.GameSettings.SFX = !GameManager.GameSettings.SFX;
            ChooseSprite(GameManager.GameSettings.SFX);
        } else {
            GameManager.GameSettings.Music = !GameManager.GameSettings.Music;
            ChooseSprite(GameManager.GameSettings.Music);
            MusicManager.SetMusic();
        }

        AppSettings.SaveSettings(GameManager.GameSettings);
    }

    void ChooseSprite (bool Choose) {
        if (Choose) {
            transform.Find("InnerIcon").GetComponent<Image>().sprite = soundOn;
        } else {
            transform.Find("InnerIcon").GetComponent<Image>().sprite = soundOff;
        }
    }
}
