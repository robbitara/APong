using UnityEngine;

public class MusicManager : MonoBehaviour {

    public static AudioSource Music;

    void Awake() {
        Music = GetComponent<AudioSource>();
    }

    public static void SetMusic() {
        if (GameManager.GameSettings.Music) {
            Music.Play();
        } else {
            Music.Stop();
        }
    }

	// Use this for initialization
	void Start () {
        SetMusic();
	}
}
