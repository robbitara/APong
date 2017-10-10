using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IDragHandler, IPointerDownHandler {

    GameManager Manager;
    public RectTransform bg_joy, pad;
    public Vector2 inputVector;
    public Rigidbody2D rb2d;
    bool check;

    void Start() {
        Manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        rb2d = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        bg_joy = GetComponent<RectTransform>();
        pad = transform.GetChild(0).GetComponent<RectTransform>();
        pad.anchoredPosition = new Vector2(0, bg_joy.sizeDelta.y / 3);
    }

    void FixedUpdate() {
        if (check) {
            if (pad.anchoredPosition.normalized.x < 0) {
                rb2d.MoveRotation(Vector2.Angle(Vector2.up, pad.anchoredPosition.normalized));
            } else {
                rb2d.MoveRotation(Vector2.Angle(Vector2.up, pad.anchoredPosition.normalized) * -1);
            }
        }
    }

    public virtual void OnPointerDown(PointerEventData ped) {
        if (RectTransformUtility.RectangleContainsScreenPoint(pad, ped.position, ped.pressEventCamera)) {
            check = true;
            Manager.resetScene = false;
        } else {
            check = false;
        }
    }

    public virtual void OnDrag(PointerEventData ped) {
        if (check) {
            Vector2 pos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(bg_joy, ped.position, ped.pressEventCamera, out pos)) {
                pos.x = (pos.x / bg_joy.sizeDelta.x);
                pos.y = (pos.y / bg_joy.sizeDelta.y);

                inputVector = new Vector2(pos.x * 2, pos.y * 2).normalized;
                pad.anchoredPosition = new Vector2((inputVector.x * bg_joy.sizeDelta.x / 3), (inputVector.y * bg_joy.sizeDelta.y / 3));
            }
        }
    }
}
