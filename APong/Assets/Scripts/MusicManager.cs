using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    public static AudioSource Music;

    void Awake() {
        Music = GetComponent<AudioSource>();
    }

    public static void SetMusic() {
        if (GameManager.Player.music) {
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
