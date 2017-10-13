using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

public class PlayGamesScript : MonoBehaviour {

    void Start() {

        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        SignIn();
    }

    void SignIn() {
        Social.localUser.Authenticate(success => { });
    }

    public static void AddScoreToLeaderboard(string leaderboardID, long score) {
        Social.ReportScore(score, leaderboardID, success => { });
    }

    public static void ShowLeaderBoard() {
        PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_apong_leaderboard);
    }
}
