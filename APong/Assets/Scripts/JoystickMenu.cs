using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JoystickMenu : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler {

    Image bg_joy, pad;
    Vector2 inputVector, zoomed_button, pos;
    GameManager manager;
    public bool overButton;
    bool dragging;
    GameObject nowOver;

    float zoomValue, distance;

    GameObject[] buttons;

    void Awake() {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        bg_joy = GetComponent<Image>();
        pad = transform.Find("Pad").GetComponent<Image>();

        nowOver = null;
        zoomValue = 1.5f;
        distance = 150f;
        dragging = false;
        overButton = false;
    }

    void Start() {
        buttons = new GameObject[GetComponent<Transform>().childCount];
        for (int i=1; i < buttons.Length; i++) {
            buttons[i] = GetComponent<Transform>().GetChild(i).gameObject;
        }

        zoomed_button = new Vector2(buttons[1].GetComponent<RectTransform>().sizeDelta.x * zoomValue, buttons[1].GetComponent<RectTransform>().sizeDelta.y * zoomValue);
        pad.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, bg_joy.rectTransform.sizeDelta.y / 3);

        StartCoroutine(GameManager.WaitForJob(0.5f, () => {
            pad.GetComponent<Animation>().Play();
        }));
    }

    void Update() {
        if (!dragging) {
            pad.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(pad.GetComponent<RectTransform>().anchoredPosition, new Vector2(0, bg_joy.rectTransform.sizeDelta.y / 3), Time.deltaTime *4f);
        }
    }

    public void OnEndDrag(PointerEventData ped) {
        if (overButton) {
            nowOver.GetComponent<RectTransform>().sizeDelta = new Vector2(zoomed_button.x / zoomValue, zoomed_button.y / zoomValue);

            if (nowOver.name == "Play") {
                nowOver.GetComponent<RectTransform>().anchoredPosition = new Vector2(200f, 0);
                Camera.main.GetComponent<CameraScript>().Right = true;
                manager.StartGame();
            }

            if (nowOver.name == "Custom") {
                nowOver.GetComponent<RectTransform>().anchoredPosition = new Vector2(-200f, 0);
                Camera.main.GetComponent<CameraScript>().Left = true;
            }

            if (nowOver.name == "Leaderboard") {
                PlayGamesScript.ShowLeaderBoard();
                nowOver.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -200f);
            }

            GameManager.PlaySound(manager.GetComponent<AudioSource>(), 0f);

        }

        dragging = false;
    }

    public void OnPointerDown(PointerEventData ped) {
        if (RectTransformUtility.RectangleContainsScreenPoint(pad.rectTransform, ped.position, ped.pressEventCamera)) {

            if (pad.GetComponent<Animation>().isPlaying) {
                pad.GetComponent<Animation>().Stop();
            }

            dragging = true;
        } else {
            dragging = false;
        }
    }

    public void OnDrag(PointerEventData ped) {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(bg_joy.rectTransform, ped.position, ped.pressEventCamera, out pos)) {

            pos.x = (pos.x / bg_joy.rectTransform.sizeDelta.x);
            pos.y = (pos.y / bg_joy.rectTransform.sizeDelta.y);

            inputVector = new Vector2(pos.x * 2, pos.y * 2);
            inputVector = inputVector.normalized;

            if (dragging) {
                pad.rectTransform.anchoredPosition = new Vector2((inputVector.x * bg_joy.rectTransform.sizeDelta.x / 3), (inputVector.y * bg_joy.rectTransform.sizeDelta.y / 3));
            }
        }

        checkDistance();
    }

    void checkDistance() {
        if (!overButton) {
            if (Vector2.Distance(pad.rectTransform.anchoredPosition, buttons[1].GetComponent<RectTransform>().anchoredPosition) < distance) {
                overButton = true;
                nowOver = buttons[1];
            }

            if (Vector2.Distance(pad.rectTransform.anchoredPosition, buttons[2].GetComponent<RectTransform>().anchoredPosition) < distance) {
                overButton = true;
                nowOver = buttons[2];
            }

            if (Vector2.Distance(pad.rectTransform.anchoredPosition, buttons[3].GetComponent<RectTransform>().anchoredPosition) < distance) {
                overButton = true;
                nowOver = buttons[3];
            }

        }

        if (nowOver != null) {

            nowOver.GetComponent<RectTransform>().sizeDelta = zoomed_button;

            if (nowOver.name == "Custom") {
                nowOver.GetComponent<RectTransform>().anchoredPosition = new Vector2(-250f, 0);
            }

            if (nowOver.name == "Play") {
                nowOver.GetComponent<RectTransform>().anchoredPosition = new Vector2(250f, 0);
            }

            if (nowOver.name == "Leaderboard") {
                nowOver.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -250f);
            }

            if (Vector2.Distance(pad.rectTransform.anchoredPosition, nowOver.GetComponent<RectTransform>().anchoredPosition) > distance) {

                if (nowOver.name == "Custom") {
                    nowOver.GetComponent<RectTransform>().anchoredPosition = new Vector2(-200f, 0);
                }

                if (nowOver.name == "Play") {
                    nowOver.GetComponent<RectTransform>().anchoredPosition = new Vector2(200f, 0);
                }

                if (nowOver.name == "Leaderboard") {
                    nowOver.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -200f);
                }

                nowOver.GetComponent<RectTransform>().sizeDelta = new Vector2(zoomed_button.x / zoomValue, zoomed_button.y / zoomValue);
                overButton = false;
                nowOver = null;
            }
        }
    }

    float GetAngle(Vector2 a, Vector2 b) {
        return ((Mathf.Atan2(b.y - a.y, b.x - a.x)) * 180 / Mathf.PI);
    }

}
