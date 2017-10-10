using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour {

    void Start() {
        InitAd();
    }

    public static void InitAd() {
        Advertisement.Initialize("1472599", true);
    }

    public static IEnumerator ShowAdWhenReady() {
        while (!Advertisement.IsReady()) {
            yield return null;
        }
        Advertisement.Show();
    }

}
