using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Advertisements;
using System;

public class GameManager : MonoBehaviour {

    GameObject MenuShowed;
    Vector2 GlobalMenuTarget;

    public GameObject palla, barra, HighscoreMenu, LoseMenu, back_punti, HighscorePopup, background, introPad, gamePad, pointsCircle, rewardIcon, noInternet, blackOverlay, countdown, Hint;
    public Text punti, highscore_intro;
    public Sprite blackPad, whitePad;
    public static int points, nextColor;
    public Joystick pad_class;
    bool playing, ReachedHighscore, isConnected;
    public bool resetScene;

    // Instanze profilo giocatore
    public static PlayerClass Player;
    public static AppSettings GameSettings;

    void Awake() {
        GameSettings = AppSettings.LoadSettings();
        Player = PlayerClass.LoadProfile();
        print("Ora attuale: " + DateTime.Today);
    }

    void Start() {
        LoadProfile();                                                      // Imposto Skin / seleziono skin da utilizzare
        StartCoroutine(checkInternetConnection( () => { UpdateStats(); }));    

        resetScene = false;                                                 // Imposto bool di reset della scena
        ReachedHighscore = false;                                           // Imposto bool di raggiungimento highscore
        playing = false;                                                    // Imposto bool di controllo su partita in corso
    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.N)) {
            NewProfile();
        }
        
        back_punti.GetComponentInChildren<Text>().text = points.ToString();

        if (points > Player.highscore && !ReachedHighscore) {
            HighscorePopup.GetComponent<Animation>().Play();
            PlaySound(HighscorePopup.GetComponent<AudioSource>(), 1.4f);
            ReachedHighscore = true;
        }

        if (Vector2.Distance(palla.transform.position, palla.transform.parent.position) > 2.5f && playing) {
            barra.GetComponent<PolygonCollider2D>().enabled = false;
            playing = false;
            StartCoroutine(WaitForAd());
        }

        if (resetScene) {
            pad_class.pad.anchoredPosition = Vector2.Lerp(pad_class.pad.anchoredPosition, new Vector2(0, pad_class.bg_joy.sizeDelta.y / 3), Time.deltaTime * 4f);
            barra.transform.rotation = Quaternion.Lerp(barra.transform.rotation, new Quaternion(0, 0, 0, 0), Time.deltaTime * 4f);
        }

        ShowMenu(GlobalMenuTarget);
    }

    /*  UpdateStats: AGGIORNAMENTO STATISTICHE
    
        - Aggiorno highscore nel main menu
        - Resetto eventualmente le partite giocate
        - Aggiorno icona del reward giornaliero in base alla presenza di connessione ad Internet
        - Aggiorno skin sbloccate dal giocatore
        - Salvo profilo
    
    */

    public void UpdateStats() {

        debugPlayer(Player);

        highscore_intro.text = Player.highscore.ToString();

        Player.ResetGamesPlayed();

        if (isConnected) {
            if (Player.Rewarded() || Player.gifts == -1) {
                rewardIcon.transform.Find("ObtainedIcon").gameObject.SetActive(true);
                rewardIcon.transform.Find("Percentage").gameObject.SetActive(false);
                rewardIcon.transform.Find("LoadingCircle").gameObject.SetActive(false);
            } else {
                rewardIcon.transform.Find("Percentage").gameObject.SetActive(true);
                rewardIcon.transform.Find("LockedGift").gameObject.SetActive(false);
                rewardIcon.transform.Find("ObtainedIcon").gameObject.SetActive(false);
                rewardIcon.transform.Find("LoadingCircle").gameObject.SetActive(true);

                rewardIcon.transform.Find("Percentage").GetComponent<Text>().text = Player.gamesPlayed.ToString() + "/5";

                if (Player.gamesPlayed != 0) {
                    rewardIcon.transform.Find("LoadingCircle").GetComponent<Image>().fillAmount = (Player.gamesPlayed / 5f);    
                } else {
                    rewardIcon.transform.Find("LoadingCircle").GetComponent<Image>().fillAmount = 0;
                }
            }
        } else {
            rewardIcon.transform.Find("Percentage").gameObject.SetActive(false);
            rewardIcon.transform.Find("ObtainedIcon").gameObject.SetActive(false);
            rewardIcon.transform.Find("LoadingCircle").gameObject.SetActive(false);
            rewardIcon.transform.Find("LockedGift").gameObject.SetActive(true);

            if (!GameSettings.internetCheck) {
                noInternet.SetActive(true);

                if (Application.systemLanguage == SystemLanguage.Italian) {
                    noInternet.transform.Find("Text").GetComponent<Text>().text = "Sembra che tu non sia connesso ad internet.\nPuoi comunque continuare a giocare" +
                        ", ma i tuoi punteggi alti non verranno salvati e non potrai sbloccare alcuna skin.";
                }

                MenuShowed = noInternet;
                GlobalMenuTarget = new Vector2(MenuShowed.GetComponent<RectTransform>().anchoredPosition.x, 0f);
                GameSettings.internetCheck = true;
            }
        }

        SkinManager.setSkins();

        PlayerClass.SaveProfile(Player, true);
    }

    //  WaitForAd: Coroutine che mostra la pubblicità. Solo Una volta chiusa, continua l'esecuzione del codice.

    IEnumerator WaitForAd() {

        if (!Advertisement.IsReady()) {
            AdManager.InitAd();
        }

        if (isConnected) {
            Player.gamesPlayed++;

            if (Player.gifts < SkinManager.RandomSkin.Length) {
                Player.gifts++;
            }
        }

        int x = UnityEngine.Random.Range(0, 6);
        if (x == 3) {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(AdManager.ShowAdWhenReady());

            while (Advertisement.isShowing) {
                yield return null;
            }
        }

        SelectEndGameMenu();
    }

    /*  SelectEndGameMenu
    
        - Imposto highscore giocatore (se connesso ad Internet)
        - In base al punteggio scelgo menu da mostrare
        - Imposto GlobalMenuTarget
    
    */

    void SelectEndGameMenu () {
        if (points > Player.highscore) {
            MenuShowed = SetMenu(HighscoreMenu);
        } else {
            MenuShowed = SetMenu(LoseMenu);
        }

        GlobalMenuTarget = new Vector2(MenuShowed.GetComponent<RectTransform>().anchoredPosition.x, 100f);
    }

    // SetMenu: In base al menu selezionato da SelectEndGameMenu(), mostro risultati in tale menu.
    // Qui terminano le chiamate di fine partita.

    GameObject SetMenu(GameObject result) {

        if (isConnected && points > Player.highscore) {
            Player.highscore = points;
            PlayGamesScript.AddScoreToLeaderboard(GPGSIds.leaderboard_apong_leaderboard, Player.highscore);
        }

        result.SetActive(true);

        if (Player.gamesPlayed != 5 || (points < nextColor && !SkinManager.AllUnlocked())) {
            result.transform.Find("Custom").gameObject.SetActive(false);
        }

        // Se connesso ad internet

        if (isConnected) {
            // Sezione relativa alla skin giornaliera

            if (Player.Rewarded() || Player.gifts == -1) {
                result.transform.Find("AuxMenu").gameObject.SetActive(false);
                StartCoroutine(WaitForJob(1f, () => {

                    if (points >= nextColor && !SkinManager.AllUnlocked()) {
                        result.transform.Find("Custom").gameObject.SetActive(true);
                        result.transform.Find("Custom").GetComponent<Animation>().Play();
                        SkinManager.FindNextSkin();
                    } else {
                        result.transform.Find("Custom").gameObject.SetActive(false);
                    }
                }));
            } else {
                result.transform.Find("AuxMenu").gameObject.SetActive(true);

                for (int i=0; i < result.transform.Find("AuxMenu/CirclesBox").childCount; i++) {
                    result.transform.Find("AuxMenu/CirclesBox").GetChild(i).GetComponent<Image>().color = Color.black;
                    result.transform.Find("AuxMenu/CirclesBox").GetChild(i).GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.5f, 1f);
                    result.transform.Find("AuxMenu/CirclesBox").GetChild(i).Find("Checkmark").gameObject.SetActive(false);
                }

                for (int i = 0; i < (Player.gamesPlayed - 1); i++) {
                    result.transform.Find("AuxMenu/CirclesBox").GetChild(i).GetComponent<Image>().color = new Color32(0, 222, 94, 255);
                    result.transform.Find("AuxMenu/CirclesBox").GetChild(i).GetComponent<RectTransform>().localScale = Vector3.one;
                    result.transform.Find("AuxMenu/CirclesBox").GetChild(i).Find("Checkmark").gameObject.SetActive(true);
                }

                StartCoroutine(WaitForJob(1f, () => {

                    result.transform.Find("AuxMenu/CirclesBox").GetChild(Player.gamesPlayed - 1).GetComponent<Image>().color = new Color32(0, 222, 94, 255);
                    result.transform.Find("AuxMenu/CirclesBox").GetChild(Player.gamesPlayed - 1).Find("Checkmark").gameObject.SetActive(true);
                    result.transform.Find("AuxMenu/CirclesBox").GetChild(Player.gamesPlayed - 1).GetComponent<Animation>().Play();
                    PlaySound(result.transform.Find("AuxMenu").GetComponent<AudioSource>(), 0f);

                    print(points + "\n" + nextColor);

                    if (Player.gamesPlayed == 5 || (points >= nextColor && !SkinManager.AllUnlocked())) {

                        if (Player.gamesPlayed == 5) {
                            SkinManager.RandomSkin[Player.gifts - 1].transform.Find("AlertIcon").gameObject.SetActive(true);
                        }

                        result.transform.Find("Custom").gameObject.SetActive(true);
                        result.transform.Find("Custom").GetComponent<Animation>().Play();
                    }

                    SkinManager.FindNextSkin();

                    if (Player.gifts == SkinManager.RandomSkin.Length) {
                        Player.gifts = -1;
                    }

                }));
            }
        } else {
            result.transform.Find("AuxMenu").gameObject.SetActive(false);
        }

        if (result.name == "HighscoreMenu") {
            result.transform.Find("HighscoreCircle/InnerCircle").GetComponentInChildren<Text>().text = points.ToString();
        }

        if (result.name == "LoseMenu") {
            result.transform.Find("HighscoreCircle/InnerCircle/Text").GetComponent<Text>().text = Player.highscore.ToString();
            result.transform.Find("ScoreCircle/InnerCircle/Text").GetComponent<Text>().text = points.ToString();
        }

        return result;
    }

    /*  StartGame - FUNZIONE DI INIZIO PARTITA 
     
        - Controllo connessione ad internet
        - Resetto la scena per nuova partita
        - Eseguo coroutine WaitandPlay
         
    */

    public void StartGame() {

        StartCoroutine(checkInternetConnection(() => {  }));

        SkinManager.FindNextSkin();

        if (!Advertisement.isShowing) {
            ResetMenu();
            points = 0;
            ReachedHighscore = false;
            playing = true;
            palla.GetComponent<BallPhysics>().ResetBall();
            barra.GetComponent<PolygonCollider2D>().enabled = true;
            StartCoroutine(WaitAndPlay());
        }
    }

    // Funzione per il reset dei menu

    public void ResetMenu() {

        if (MenuShowed != null) {
            GlobalMenuTarget = new Vector2(MenuShowed.GetComponent<RectTransform>().anchoredPosition.x, 1300f);
        }

        if (!playing) {
            resetScene = true;
        }
    }

    void ShowMenu(Vector2 Direction) {
        if (MenuShowed != null) {
            MenuShowed.SetActive(true);
            float _transitionSpeed = Time.deltaTime * 4f;
            MenuShowed.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(MenuShowed.GetComponent<RectTransform>().anchoredPosition, Direction, _transitionSpeed);
        }
    }

     public void Animate() {
        back_punti.GetComponent<Animation>().Play();
        palla.GetComponent<Animation>().Play();
    }

    public void LoadProfile() {

        SkinManager.setSkins();

        if (Player.SkinType == "RandomSkin") {
            SkinManager.RandomSkin[Player.skinID].GetComponent<SkinScript>().ChangeBGColor(true);
        } else {
            SkinManager.EliteSkin[Player.skinID].GetComponent<SkinScript>().ChangeBGColor(true);
        }
    }

    /*  WaitAndPlay 
    
        - Eseguo animazione countdown
        - Attendo 3 secondi
        - Lancio pallina 
         
    */

    IEnumerator WaitAndPlay() {
        int seconds = 3;

        countdown.SetActive(true);

        while (seconds > 0) {
            countdown.GetComponentInChildren<Text>().text = seconds.ToString();

            countdown.GetComponent<Animation>().Play("Countdown");
            seconds -= 1;

            while (countdown.GetComponent<Animation>().isPlaying) {
                yield return null;
            }
        }

        countdown.GetComponent<Image>().fillAmount = 0f;
        countdown.SetActive(false);
        resetScene = false;
        palla.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 4f);
    }

    public static IEnumerator CheckInstance(Action Job) {
        while (Player == null) {
            print("Loading... ");
            yield return null;
        }

        Job();
    }

    public static IEnumerator WaitForJob(float seconds, Action Job) {
        yield return new WaitForSeconds(seconds);
        Job();
    }

    #region Utilites

    void NewProfile() {
        Player = new PlayerClass();
        UpdateStats();
        LoadProfile();
    }

    public static void PlaySound(AudioSource Clip, float delay) {
        if (GameSettings.SFX && !Clip.isPlaying) {
            Clip.PlayDelayed(delay);
        }
    }

    public void PlaySound() {
        if (GameSettings.SFX && !GetComponent<AudioSource>().isPlaying) {
            GetComponent<AudioSource>().Play();
        }
    }

    IEnumerator checkInternetConnection(Action Job) {
        float time = 0f;
        WWW www = new WWW("http://www.google.com");

        while (!www.isDone) {
            time += 0.1f;
            yield return null;
        }

        yield return www;

        if (www.error != null) {
            isConnected = false;
        } else {
            isConnected = true;
        }

        print("Time to download: " + time);

        Job();
    }

    public static void ArraySort(GameObject[] Array, int length, string mode) {
        GameObject obj;
        int i, j;

        if (mode == "Elite") {
            for (i = 1; i < length; i++) {
                obj = Array[i];
                for (j = i - 1; j >= 0 && Array[j].GetComponent<SkinScript>().pointsToUnlock > obj.GetComponent<SkinScript>().pointsToUnlock; j--) {
                    Array[j + 1] = Array[j];
                }
                Array[j + 1] = obj;
            }
        }

        if (mode == "Random") {
            for (i = 1; i < length; i++) {
                obj = Array[i];
                for (j = i - 1; j >= 0 && Array[j].GetComponent<SkinScript>().ID > obj.GetComponent<SkinScript>().ID; j--) {
                    Array[j + 1] = Array[j];
                }
                Array[j + 1] = obj;
            }
        }
    }

    public void GoToAppPage() {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.supercell.clashroyale&hl=it");
    }

    public static void debugPlayer(PlayerClass Player) {
        print("Highscore: " + Player.highscore);
        print("skinID: " + Player.skinID);
        print("SkinType: " + Player.SkinType);
        print("Partite giocate: " + Player.gamesPlayed);
        print("Gifts: " + Player.gifts);
        print(Player.saveDate + " / " + DateTime.Now);
        print(Player.Rewarded());
    }

    #endregion Utilies

}
