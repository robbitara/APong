﻿using UnityEngine;
using UnityEngine.UI;

public class SkinManager : MonoBehaviour {

    public static GameObject[] EliteSkin, RandomSkin;
    public static Color GlobalColor;

    void Awake() {

        //StartCoroutine(GameManager.CheckInstance(() => setSkins()));

        GlobalColor = Color.clear;
        EliteSkin = GameObject.FindGameObjectsWithTag("EliteSkin");
        RandomSkin = GameObject.FindGameObjectsWithTag("RandomSkin");
    }

    void Update() {
        if (GlobalColor != Color.clear) {
            Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, GlobalColor, Time.deltaTime * 4f);
        }
    }

    public static void setSkins() {

        // imposto ID in base a posizione nell'inspector
        for (int i=0; i < RandomSkin.Length; i++) {
            RandomSkin[i].GetComponent<SkinScript>().ID = RandomSkin[i].transform.GetSiblingIndex();
        }

        // ordino l'array
        GameManager.ArraySort(RandomSkin, RandomSkin.Length, "Random");

        if (GameManager.Player.gifts != -1) {
            // Imposto le skin random in base ai gift del giocatore
            for (int i = 0; i < RandomSkin.Length; i++) {
                if (i < GameManager.Player.gifts) {
                    RandomSkin[i].GetComponent<Image>().color = RandomSkin[i].GetComponent<SkinScript>().bgColor;
                    int n = i;
                    RandomSkin[i].GetComponent<Button>().onClick.AddListener(() => RandomSkin[n].GetComponent<SkinScript>().ChangeBGColor(false));
                    RandomSkin[i].transform.Find("LabelContainer").Find("Text").GetComponent<Text>().text = RandomSkin[i].GetComponent<SkinScript>().skinName;
                    RandomSkin[i].transform.Find("Locked").gameObject.SetActive(false);
                    if (RandomSkin[i].GetComponent<SkinScript>().ID == GameManager.Player.skinID && GameManager.Player.SkinType == "RandomSkin") {
                        RandomSkin[i].GetComponent<SkinScript>().isUsing = true;
                    }
                } else {
                    RandomSkin[i].GetComponent<Image>().color = Color.black;
                    RandomSkin[i].transform.Find("LabelContainer").Find("Text").GetComponent<Text>().text = "Locked";
                    RandomSkin[i].GetComponent<Transform>().Find("Locked").gameObject.SetActive(true);
                }
            }
        } else {
            for (int i =0; i<RandomSkin.Length; i++) {
                RandomSkin[i].GetComponent<Image>().color = RandomSkin[i].GetComponent<SkinScript>().bgColor;
                int n = i;
                RandomSkin[i].GetComponent<Button>().onClick.AddListener(() => RandomSkin[n].GetComponent<SkinScript>().ChangeBGColor(false));
                RandomSkin[i].transform.Find("LabelContainer").Find("Text").GetComponent<Text>().text = RandomSkin[i].GetComponent<SkinScript>().skinName;
                RandomSkin[i].transform.Find("Locked").gameObject.SetActive(false);
                if (RandomSkin[i].GetComponent<SkinScript>().ID == GameManager.Player.skinID && GameManager.Player.SkinType == "RandomSkin") {
                    RandomSkin[i].GetComponent<SkinScript>().isUsing = true;
                }
            }
        }

        // imposto le skin elite in base al punteggio massimo del giocatore

        GameManager.ArraySort(EliteSkin, EliteSkin.Length, "Elite");

        for (int i = 0; i < EliteSkin.Length; i++) {
            EliteSkin[i].GetComponent<SkinScript>().ID = i;
            if (GameManager.Player.highscore >= EliteSkin[i].GetComponent<SkinScript>().pointsToUnlock) {
                EliteSkin[i].GetComponent<Image>().color = EliteSkin[i].GetComponent<SkinScript>().bgColor;
                int n = i;
                EliteSkin[i].GetComponent<Button>().onClick.AddListener(() => EliteSkin[n].GetComponent<SkinScript>().ChangeBGColor(false));
                EliteSkin[i].transform.Find("LabelContainer").transform.Find("Text").GetComponent<Text>().text = EliteSkin[i].GetComponent<SkinScript>().skinName;
                EliteSkin[i].transform.Find("UnlockPoints").GetComponent<Text>().text = "";

                if (EliteSkin[i].GetComponent<SkinScript>().ID == GameManager.Player.skinID && GameManager.Player.SkinType == "EliteSkin") {
                    EliteSkin[i].GetComponent<SkinScript>().isUsing = true;
                }

            } else {
                EliteSkin[i].GetComponent<Image>().color = new Color(0, 0, 0, 1);
                EliteSkin[i].transform.Find("UnlockPoints").GetComponent<Text>().text = EliteSkin[i].GetComponent<SkinScript>().pointsToUnlock.ToString();
                EliteSkin[i].transform.Find("LabelContainer").Find("Text").GetComponent<Text>().text = "Locked";
            }
        }

        // attivo spunta su skin attualmente in uso
        if (RandomSkin[GameManager.Player.skinID].GetComponent<SkinScript>().isUsing) {
            RandomSkin[GameManager.Player.skinID].transform.Find("Ticked").gameObject.SetActive(true);
            EliteSkin[GameManager.Player.skinID].transform.Find("Ticked").gameObject.SetActive(false);
        }

        else /*(EliteSkin[GameManager.Player.skinID].GetComponent<SkinScript>().isUsing)*/ {
            RandomSkin[GameManager.Player.skinID].transform.Find("Ticked").gameObject.SetActive(false);
            EliteSkin[GameManager.Player.skinID].transform.Find("Ticked").gameObject.SetActive(true);
        }
    }

    public static bool AllUnlocked() {
        if (GameManager.nextColor >= EliteSkin[EliteSkin.Length - 1].GetComponent<SkinScript>().pointsToUnlock) {
            return true;
        }
        return false;
    }

    public static void FindNextSkin() {

        if (AllUnlocked()) {
            return;
        }

        if (GameManager.Player.highscore < EliteSkin[0].GetComponent<SkinScript>().pointsToUnlock) {
            GameManager.nextColor = EliteSkin[0].GetComponent<SkinScript>().pointsToUnlock;
        } else {
            for (int i = 1; i < (EliteSkin.Length - 1); i++) {
                if (GameManager.Player.highscore >= EliteSkin[i].GetComponent<SkinScript>().pointsToUnlock) {
                    GameManager.nextColor = EliteSkin[i+1].GetComponent<SkinScript>().pointsToUnlock;
                }
            }
        }
    }
}
