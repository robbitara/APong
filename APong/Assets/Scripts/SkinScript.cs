using UnityEngine;
using UnityEngine.UI;

public class SkinScript : MonoBehaviour {

    public Color bgColor;
    public bool UIColor, isUsing;
    public Sprite Skin;
    public string skinName;
    public int pointsToUnlock, ID;
    public GameObject backgroundSkin;

    GameManager Manager;

    void Awake() {
        isUsing = false;
        Manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
    }

    public void ChangeBGColor() {

        if (transform.Find("AlertIcon").gameObject.activeInHierarchy) {
            transform.Find("AlertIcon").gameObject.SetActive(false);
        }

        GameManager.PlaySound(Manager.GetComponent<AudioSource>(), 0f);

        SkinManager.RandomSkin[GameManager.Player.skinID].transform.Find("Ticked").gameObject.SetActive(false);
        SkinManager.EliteSkin[GameManager.Player.skinID].transform.Find("Ticked").gameObject.SetActive(false);

        SkinManager.RandomSkin[GameManager.Player.skinID].GetComponent<SkinScript>().isUsing = false;
        SkinManager.EliteSkin[GameManager.Player.skinID].GetComponent<SkinScript>().isUsing = false;

        GameManager.Player.SkinType = gameObject.tag;

        isUsing = true;

        transform.Find("Ticked").gameObject.SetActive(true);

        SkinManager.GlobalColor = bgColor;

        backgroundSkin.GetComponent<SpriteRenderer>().sprite = Skin;
        GameManager.Player.skinID = ID;
        GameManager.Player.skinColor = UIColor;
    }
}
