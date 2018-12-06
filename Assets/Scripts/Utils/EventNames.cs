
public static class EventNames 
{
	public static string OnGameStart = "OnGameStart";
    public static string OnGameEnd = "OnGameEnd";

    //Riddle
    public static string OnRiddleCreated = "OnRiddleCreated";
    public static string OnRiddleSolved = "OnRiddleSolved";
    public static string OnRiddleFailed = "OnRiddleFailed";
    public static string OpenCaptureScene = "OpenCaptureScene";
    public static string OpenMapScene = "OpenMapScene";

    //UpdateUI Events
    public static string UpdatedPlayerScoreUI = "UpdatedPlayerScoreUI";

    //Auth Events
    public static string OnFBLoggedInFail = "OnFBLoggedInFail";
    public static string OnFBLoggedIn = "OnFBLoggendIn";
    public static string OnEmailLoggedIn = "OnEmailLoggendIn";
    public static string OnEmailLogInFail = "OnEmailLogInFail";
    public static string OnCustomLoggedIn = "OnCustomLoggedIn";
    public static string OnCustomLogInFail = "OnCustomLogInFail";
    public static string StartFBLoginProcess = "StartFBLoginProcess";
    public static string StartSimpleSignUpProcess = "StartSimpleSignUp";
    public static string StartSimpleLoginProcess = "StartSimpleLogin";
    public static string PlayFabLoginCustomId = "PlayFabLoginCustomId";
    public static string PlayFabLoginFB = "PlayFabLoginFB";
    public static string OnGotFBUserName = "OnGotFBUserName";
    public static string GetFbUserName = "GetFbUserName";
    public static string LoginProcessDone = "LoginProcessDone";
    public static string LoginProcessFail = "LoginProcessFail";

    //loading
    public static string ShowLoading = "ShowLoading";
    public static string HideLoading = "HideLoading";

    //Score and leaderboard
    public static string GetUserScore = "GetUserScore";
    public static string OnUserScoreReceived = "OnUserScoreReceived";
    public static string OnUserScoreReceivedFailed = "OnUserScoreReceivedFailed";
    public static string GetLeaderboard = "GetLeaderboard";
    public static string LeaderboardFailed = "LeaderboardFailed";
    public static string LeaderboardSuccess = "LeaderboardSuccess";


    public static string MapUpdated = "MapUpdated";
}
