using System.Collections.Generic;
using PlayFab;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour 
{
    [SerializeField]
    Loading loading;

    bool isLoadingScene = false;

    [SerializeField]
    InputField InEmail;

    [SerializeField]
    InputField InPassword;

    [SerializeField]
    TMP_Text UserNameTxt;

    [SerializeField]
    TMP_Text ScoreTxt;

    [SerializeField]
    TMP_Text ErrorString;

    [SerializeField]
    GameObject MainMenuButtons;

    [SerializeField]
    GameObject LoginMenu;

    bool RememberMe = true;

    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        UserNameTxt.text = PlayerPrefs.GetString(PlayerPrefKeys.DisplayName, "user");
        ScoreTxt.text = PlayerPrefs.GetInt(PlayerPrefKeys.SCORE, 0).ToString();

        if (!PlayFabLogin.Instance.ISINGame)
        {
            LoginMenu.SetActive(true);
            MainMenuButtons.SetActive(false);
            //PlayerPrefs.SetInt("USERINGAME", 1);
            //PlayerPrefs.DeleteAll();
            PlayFabLogin.Instance.ISINGame = true;
            if (RememberMe && !string.IsNullOrEmpty(PlayerPrefs.GetString(PlayerPrefKeys.PlayFabRememberMeIdKey, "")))
            {
                loading.ShowSelf(null);

                EventManager.TriggerEvent(EventNames.PlayFabLoginCustomId, null);
            }
        }
        else
        {
            LoginMenu.SetActive(false);
            MainMenuButtons.SetActive(true);
        }
    }

    private void OnEnable()
    {
        EventManager.StartListening(EventNames.LoginProcessDone, LoginProcessDone);
        EventManager.StartListening(EventNames.LoginProcessFail, LoginProcessFail);

        EventManager.StartListening(EventNames.OnFBLoggedInFail, onFBloginFail);

        EventManager.StartListening(EventNames.OnEmailLoggedIn, LoginSuccess);
        EventManager.StartListening(EventNames.OnEmailLogInFail, LoginFailed);
        EventManager.StartListening(EventNames.OnCustomLoggedIn, CustomLoginSuccess);
        EventManager.StartListening(EventNames.OnCustomLogInFail, CustomLoginFailed);
        EventManager.StartListening(EventNames.OnUserScoreReceived, SetUserData);
        EventManager.StartListening(EventNames.OnUserScoreReceivedFailed, SetUserDataFailed);
        EventManager.StartListening(EventNames.LeaderboardSuccess, ShowLeaderBoard);
        EventManager.StartListening(EventNames.LeaderboardFailed, ShowLaederboardError);



    }

    private void OnDisable()
    {
        EventManager.StopListening(EventNames.LoginProcessDone, LoginProcessDone);
        EventManager.StopListening(EventNames.LoginProcessFail, LoginProcessFail);

        EventManager.StopListening(EventNames.OnFBLoggedInFail, onFBloginFail);

        EventManager.StopListening(EventNames.OnEmailLoggedIn, LoginSuccess);
        EventManager.StopListening(EventNames.OnEmailLogInFail, LoginFailed);
        EventManager.StopListening(EventNames.OnCustomLoggedIn, CustomLoginSuccess);
        EventManager.StopListening(EventNames.OnCustomLogInFail, CustomLoginFailed);
        EventManager.StopListening(EventNames.OnUserScoreReceived, SetUserData);
        EventManager.StopListening(EventNames.OnUserScoreReceivedFailed, SetUserData);
        EventManager.StopListening(EventNames.LeaderboardSuccess, ShowLeaderBoard);
        EventManager.StopListening(EventNames.LeaderboardFailed, ShowLaederboardError);
    }

    public void onCampaignClick()
    {
        SoundManager.Instance.PlayClickSound();
        SceneManager.LoadScene(THuntConstants.Campaign);
    }

    public void OnFbLoginClick()
    {
        loading.ShowSelf(null);
        SoundManager.Instance.PlayClickSound();

        EventManager.TriggerEvent(EventNames.StartFBLoginProcess,null);

    }

    public void OnSimpleLogin()
    {
        loading.ShowSelf(null);
        SoundManager.Instance.PlayClickSound();


        Dictionary<string, string> EmailPass = new Dictionary<string, string>
        {
            { PlayerPrefKeys.EmailKey, InEmail.text },
            { PlayerPrefKeys.PasswordKey, InPassword.text }
        };

        EventManager.TriggerEvent(EventNames.StartSimpleLoginProcess, EmailPass);
    }

    public void OnSimpleSignUp()
    {
        loading.ShowSelf(null);
        SoundManager.Instance.PlayClickSound();
        Dictionary<string, string> EmailPass = new Dictionary<string, string>
        {
            { PlayerPrefKeys.EmailKey, InEmail.text },
            { PlayerPrefKeys.PasswordKey, InPassword.text }
        };

        EventManager.TriggerEvent(EventNames.StartSimpleSignUpProcess, (object)EmailPass);
    }

    void onFBloginFail(object userdata)
    {
        Debug.Log("LoginFailed");
        PlayFabError PFError = (PlayFabError)userdata;
        OnPlayFaberror(PFError);
    }

    void LoginProcessDone(object userdata)
    {
        loading.HideSelf(null);
        LoginMenu.SetActive(false);
        MainMenuButtons.SetActive(true);
        UserNameTxt.text = PlayerPrefs.GetString(PlayerPrefKeys.DisplayName,"user");
        EventManager.TriggerEvent(EventNames.GetUserScore, null);
    }

    void LoginProcessFail(object userdata)
    {
        loading.HideSelf(null);
        ShowLogiError(THuntConstants.FailedToLogin);
    }



    void LoginSuccess(object userdata)
    {
        loading.HideSelf(null);

        UserNameTxt.text = PlayerPrefs.GetString(PlayerPrefKeys.DisplayName);
        LoginMenu.SetActive(false);
        MainMenuButtons.SetActive(true);
        EventManager.TriggerEvent(EventNames.GetUserScore, null);

    }

    void LoginFailed(object userdata)
    {
        loading.HideSelf(null);

        PlayFabError PFError = (PlayFabError)userdata;
        OnPlayFaberror(PFError);
    }

    void CustomLoginSuccess(object userdata)
    {
        loading.HideSelf(null);

        UserNameTxt.text = PlayerPrefs.GetString(PlayerPrefKeys.DisplayName);
        ScoreTxt.text = PlayerPrefs.GetInt(PlayerPrefKeys.SCORE,0).ToString();
        LoginMenu.SetActive(false);
        MainMenuButtons.SetActive(true);
    }


    void CustomLoginFailed(object userdata)
    {
        loading.HideSelf(null);

        PlayFabError PFError = (PlayFabError)userdata;
        OnPlayFaberror(PFError);
    }

    private void OnPlayFaberror(PlayFabError error)
    {
        //There are more cases which can be caught, below are some
        //of the basic ones.
        switch (error.Error)
        {
            case PlayFabErrorCode.InvalidEmailAddress:
            case PlayFabErrorCode.InvalidPassword:
                ShowLogiError(THuntConstants.InvalidUsernameOrPassword);
                break;
            case PlayFabErrorCode.InvalidEmailOrPassword:
                ShowLogiError(THuntConstants.InvalidUsernameOrPassword);
                break;
            case PlayFabErrorCode.UsernameNotAvailable:
                ShowLogiError(THuntConstants.UsernameAlreadyExist);
                break;
            case PlayFabErrorCode.AccountNotFound:
                ShowLogiError(THuntConstants.InvalidUsernameOrPassword);
                break;
            default:
                ShowLogiError(THuntConstants.FailedToLogin);
                break;

        }

        //Also report to debug console, this is optional.
        Debug.Log(error.Error);
        Debug.LogError(error.GenerateErrorReport());
    }

    void OnSignUpFailure(PlayFabError pfe)
    {
        //LoadingPan.HideSelf();
        if (pfe.Error == PlayFabErrorCode.EmailAddressNotAvailable)
        {
            ShowLogiError("Email not available");
        }
        else if (pfe.Error == PlayFabErrorCode.InvalidEmailAddress)
        {
            ShowLogiError("Invalid email");
        }
        else if (pfe.Error == PlayFabErrorCode.InvalidPassword)
        {
            ShowLogiError("Invalid Password");
        }
        else if (pfe.Error == PlayFabErrorCode.InvalidTitleId)
        {
            ShowLogiError("Enter username and password.");
        }
        else if (pfe.Error == PlayFabErrorCode.InvalidUsername)
        {
            ShowLogiError("Invalid Username");
        }
        else if (pfe.Error == PlayFabErrorCode.UsernameNotAvailable)
        {
            ShowLogiError("Username not available");
        }
    }

    void ShowLogiError(string err)
    {
        ErrorString.text = err;
        ErrorString.gameObject.SetActive(true);
        Invoke("HideErrorString", 3);
    }

    void HideErrorString()
    {
        ErrorString.gameObject.SetActive(false);

    }

    void SetUserData(object userData)
    {
        UserNameTxt.text = PlayerPrefs.GetString(PlayerPrefKeys.DisplayName,"user");
        ScoreTxt.text = PlayerPrefs.GetInt(PlayerPrefKeys.SCORE, 0).ToString();

    }

    void SetUserDataFailed(object userData)
    {
        //UserNameTxt.text = PlayerPrefs.GetString(PlayerPrefKeys.DisplayName, "user");
        //ScoreTxt.text = PlayerPrefs.GetInt(PlayerPrefKeys.SCORE, 0).ToString();

        PlayFabError PFError = (PlayFabError)userData;
        OnPlayFaberror(PFError);

    }
    void ShowLeaderBoard(object userData)
    {

    }

    void ShowLaederboardError(object userData)
    {

    }
}