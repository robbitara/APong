using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorMove : MonoBehaviour {

    RectTransform indicatore;
    Vector2 skin, tweaks;
    float transition_speed;

    // Use this for initialization
    void Start () {
        transition_speed = Time.deltaTime * 6;
        skin = new Vector2(-120, 10);
        tweaks = new Vector2(120, 10);
        indicatore = GetComponent<RectTransform>();
        indicatore.anchoredPosition = skin;
	}


    // Funzione e coroutine per slide indicatore pagina
    public void move(int tab) {
        StopAllCoroutines();
        StartCoroutine(switch_tab(tab));
    }

    IEnumerator switch_tab(int page) {
        if (page == 1) {
            while (Vector2.Distance(indicatore.anchoredPosition, skin) > 0.1f) {
                indicatore.anchoredPosition = Vector2.Lerp(indicatore.anchoredPosition, skin, transition_speed);
                yield return null;
            }
        }

        if (page == 2) {
            while (Vector2.Distance(indicatore.anchoredPosition, tweaks) > 0.1f) {
                indicatore.anchoredPosition = Vector2.Lerp(indicatore.anchoredPosition, tweaks, transition_speed);
                yield return null;
            }
        }
        yield return null;
    }
}
