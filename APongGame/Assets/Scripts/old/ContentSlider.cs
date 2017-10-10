using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class ContentSlider : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler {
    RectTransform content;
    public RectTransform indicator;
    float transition_speed, x;
    Vector2 mouse_pos, page1, page2, tab1, tab2;

	// Use this for initialization
	void Start () {
        transition_speed = Time.deltaTime * 6;
		content = GetComponent<RectTransform> ();
        page1 = new Vector2(-240, content.anchoredPosition.y);
        page2 = new Vector2(-720, content.anchoredPosition.y);
        tab1 = new Vector2(-120, 10);
        tab2 = new Vector2(120, 10);
    }

    public void OnBeginDrag(PointerEventData ped) {
        StopAllCoroutines();
        x = (ped.position.x - content.anchoredPosition.x);
    }

    public void OnDrag(PointerEventData ped) {
        float pos = ped.position.x - x;
        content.anchoredPosition = new Vector2(pos, content.anchoredPosition.y);
    }

	public void OnEndDrag(PointerEventData ped) {
        float d1 = Vector2.Distance(content.anchoredPosition, page1);
        float d2 = Vector2.Distance(content.anchoredPosition, page2);
        StartCoroutine(fix_pos(d1, d2));
	}

    // Coroutine per posizionamento pagina e indicatore dopo rilascio drag
    IEnumerator fix_pos(float d1, float d2) {
        if (d1 < d2) {
            while (Vector2.Distance(content.anchoredPosition, page1) > 0.1f) {
                content.anchoredPosition = Vector2.Lerp(content.anchoredPosition, page1, transition_speed);
                indicator.anchoredPosition = Vector2.Lerp(indicator.anchoredPosition, tab1, transition_speed);
                yield return null;
            }
        } else {
            while (Vector2.Distance(content.anchoredPosition, page2) > 0.1f) {
                content.anchoredPosition = Vector2.Lerp(content.anchoredPosition, page2, transition_speed);
                indicator.anchoredPosition = Vector2.Lerp(indicator.anchoredPosition, tab2, transition_speed);
                yield return null;
            }
        }
        yield return null;
    }


    // Funzione e coroutine utilizzate per il movimento della pagina dopo click su pulsanti header
    public void switch_p(int page) {
        StopAllCoroutines();
        StartCoroutine(switch_tab(page));
    }

    IEnumerator switch_tab(int page) {
        if (page == 1) {
            while (Vector2.Distance(content.anchoredPosition, page1) > 0.1f) {
                content.anchoredPosition = Vector2.Lerp(content.anchoredPosition, page1, transition_speed);
                yield return null;
            }
        }

        if (page == 2) {
            while (Vector2.Distance(content.anchoredPosition, page2) > 0.1f) {
                content.anchoredPosition = Vector2.Lerp(content.anchoredPosition, page2, transition_speed);
                yield return null;
            }
        }
        yield return null;
    }
    
}
