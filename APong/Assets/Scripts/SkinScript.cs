using UnityEngine;

public class SkinScript : MonoBehaviour {

    public Color bgColor;
    public bool isUsing;
    public bool unlocked = false;
    public Sprite Skin;
    public string skinName;
    public int pointsToUnlock, ID;
    public GameObject backgroundSkin;

    GameManager Manager;

    void Awake() {
        isUsing = false;
        Manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
    }

    public void ChangeBGColor(bool isSilent) {

        if (transform.Find("AlertIcon").gameObject.activeInHierarchy) {
            transform.Find("AlertIcon").gameObject.SetActive(false);
        }

        if (!isSilent) {
            GameManager.PlaySound(Manager.GetComponent<AudioSource>(), 0f);
        }

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
    }
}
