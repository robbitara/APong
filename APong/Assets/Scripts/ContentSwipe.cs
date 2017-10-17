using UnityEngine.UI;
using UnityEngine;

public class ContentSwipe : MonoBehaviour {

    float deadZone = 50f;
    Vector3 ClickPos;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {	
        if (Camera.main.transform.position.x < -5f) {

            if (Input.GetMouseButtonDown(0)) {
                ClickPos = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0)) {
                Vector2 deltaPos = ClickPos - Input.mousePosition;

                if (Mathf.Abs(deltaPos.x) > deadZone) {
                    Camera.main.GetComponent<CameraScript>().GoCenter();
                }
            }
            
        }
	}
}
