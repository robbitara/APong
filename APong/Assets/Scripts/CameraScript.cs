﻿using UnityEngine;

public class CameraScript : MonoBehaviour {

    public bool Left, Right;
    public Vector3 CustomMenu, PlayArea, Center, ClickPos;

    float deadZone = 50f;

    void Start () {
        Left = false;
        Right = false;
        CustomMenu = new Vector3(-5.62f, 0, -10f);
        PlayArea = new Vector3(7f, 0, -10f);
        Center = transform.position;
	}

    public void GoLeft() {
        Left = true;
        Right = false;
    }

    public void GoRight() {
        Left = false;
        Right = true;
    }

    public void GoCenter() {
        Left = false;
        Right = false;
    }


    public void LerpCameraTo(Vector3 Direction) {
        if (transform.position != Direction) {
            transform.position = Vector3.Lerp(transform.position, Direction, Time.deltaTime * 4f);
        }
    }

    void Update() {

        if (transform.position.x < -5f) {
            if (Input.GetMouseButtonDown(0)) {
                ClickPos = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0)) {
                Vector2 deltaPos = ClickPos - Input.mousePosition;

                if (Mathf.Abs(deltaPos.x) > deadZone) {
                    GoCenter();
                }
            }
        }

        if (Left && !Right) {
            LerpCameraTo(CustomMenu);
        }

        if (Right && !Left) {
            LerpCameraTo(PlayArea);
        }

        if (!Left && !Right) {
            LerpCameraTo(Center);
        }
    }
}
