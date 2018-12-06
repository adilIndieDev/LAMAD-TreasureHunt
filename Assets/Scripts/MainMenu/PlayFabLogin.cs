using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;

public enum Authtypes
{
    None,
    Silent,
    UsernameAndPassword,
    EmailAndPassword,
    RegisterPlayFabAccount,
    Steam,
    Facebook,
    Google
}
public class PlayFabLogin : MonoBehaviour 
{
    public GetPlayerCombinedInfoRequestParams InfoRequestParams;

    public static string PlayFabId { get { return _playFabId; } }
    private static string _playFabId;

    public static string SessionTicket { get { return _sessionTicket; } }
    private static string _sessionTicket;

    string Password;

    public bool ForceLink = false;

    public bool ISINGame = false;
    public Authtypes AuthType
    {
        get
        {
            return (Authtypes)PlayerPrefs.GetInt(PlayerPrefKeys.AuthTypeKey, 0);
        }
        set
        {

            PlayerPrefs.SetInt(PlayerPrefKeys.AuthTypeKey, (int)value);
        }
    }

    public string Username
    {
        get
        {
            return PlayerPrefs.GetString(PlayerPrefKeys.USERNAME, "");
        }
        set
        {

            PlayerPrefs.SetString(PlayerPrefKeys.USERNAME, value);
        }
    }

    private string RememberMeId
    {
        get
        {
            return PlayerPrefs.GetString(PlayerPrefKeys.PlayFabRememberMeIdKey, "");
        }
        set
        {
            ;
            PlayerPrefs.SetString(PlayerPrefKeys.PlayFabRememberMeIdKey, value);
        }
    }

    private static PlayFabLogin _instance;

    public static PlayFabLogin Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<PlayFabLogin>();

                //Tell unity not to destroy this object when loading a new scene!
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    void Awake()
    {
        Debug.Log("Awake Called");
        if (_instance == null)
        {
            //If I am the first instance, make me the Singleton
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            //If a Singleton already exists and you find
            //another reference in scene, destroy it!
            if (this != _instance)
                Destroy(gameObject);
        }

    }

    private void OnEnable()
    {
        EventManager.StartListening(EventNames.StartSimpleLoginProcess, SignIn);
        EventManager.StartListening(EventNames.StartSimpleSignUpProcess, SignUp);
        EventManager.StartListening(EventNames.PlayFabLoginCustomId, SignInCustomID);
        EventManager.StartListening(EventNames.PlayFabLoginFB, FBLogin);
        EventManager.StartListening(EventNames.GetUserScore, getAllProfileData);
        EventManager.StartListening(EventNames.GetLeaderboard, DoReadLeaderboard);

        EventManager.StartListening(EventNames.OnGotFBUserName, UpdatePlayFabDisplayName);

        EventManager.StartListening(EventNames.UpdatedPlayerScoreUI, UpdatePlayerStatistics);


    }

    private void OnDisable()
    {
        EventManager.StopListening(EventNames.StartSimpleLoginProcess, SignIn);
        EventManager.StopListening(EventNames.StartSimpleSignUpProcess, SignUp);
        EventManager.StopListening(EventNames.PlayFabLoginCustomId, SignInCustomID);

        EventManager.StopListening(EventNames.PlayFabLoginFB, FBLogin);

        EventManager.StopListening(EventNames.GetUserScore, getAllProfileData);
        EventManager.StopListening(EventNames.GetLeaderboard, DoReadLeaderboard);

        EventManager.StopListening(EventNames.OnGotFBUserName, UpdatePlayFabDisplayName);
        EventManager.StopListening(EventNames.UpdatedPlayerScoreUI, UpdatePlayerStatistics);


    }


    void SignIn(object userdata) 
    {
        Dictionary<string, string> EmailPass = (Dictionary<string, string>)userdata;
        string user = "";
        EmailPass.TryGetValue(PlayerPrefKeys.EmailKey,out user);
        EmailPass.TryGetValue(PlayerPrefKeys.PasswordKey, out Password);

        Username = user;

        LoginWithUserName();
    }

    void SignUp(object userdata)
    {
        //Any time we attempt to register a player, first silently authenticate the player.
        //This will retain the players True Origination (Android, iOS, Desktop)
        Dictionary<string, string> EmailPass = (Dictionary<string, string>)userdata;
        string user = "";
        EmailPass.TryGetValue(PlayerPrefKeys.EmailKey, out user);
        EmailPass.TryGetValue(PlayerPrefKeys.PasswordKey, out Password);

        Username = user;

            
            PlayFabClientAPI.RegisterPlayFabUser(new RegisterPlayFabUserRequest()
            {
                RequireBothUsernameAndEmail = false,
                DisplayName = Username,
                Username = Username, //Because it is required & Unique and not supplied by User.
                //Email = Username,
                Password = Password,
            }, (addResult) => {

                    _playFabId = addResult.PlayFabId;
                    _sessionTicket = addResult.SessionTicket;
                    
                    //Generate a new Guid 
                    RememberMeId = Guid.NewGuid().ToString();
                    //Fire and forget, but link the custom ID to this PlayFab Account.
                    PlayFabClientAPI.LinkCustomID(new LinkCustomIDRequest()
                    {
                        CustomId = RememberMeId,
                        ForceLink = ForceLink
                    }, null, null);

                PlayerPrefs.SetString(PlayerPrefKeys.DisplayName, Username);

                //Override the auth type to ensure next login is using this auth type.
                AuthType = Authtypes.EmailAndPassword;

                //Report login result back to subscriber.
            EventManager.TriggerEvent(EventNames.OnEmailLoggedIn, null);
            }, (error) => {
                //if (OnPlayFabError != null)
                {
                    //Report error result back to subscriber
                EventManager.TriggerEvent(EventNames.OnEmailLogInFail, error);

                }
            });
    }

    void SignInCustomID(object userdata)
    {
        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
        {
            TitleId = PlayFabSettings.TitleId,
            CustomId = RememberMeId,
            CreateAccount = true,
            InfoRequestParameters = InfoRequestParams
        }, 
        (result) =>
        {
            //Store identity and session
            _playFabId = result.PlayFabId;
            _sessionTicket = result.SessionTicket;
            //int score = result.CustomData


            EventManager.TriggerEvent(EventNames.OnCustomLoggedIn,null);
        }, (error) =>
        {
            EventManager.TriggerEvent(EventNames.OnCustomLogInFail, (object)error);

        });
    }


    private void LoginWithUserName()
    {

        //a good catch: If username & password is empty, then do not continue, and Call back to Authentication UI Display 
        if (string.IsNullOrEmpty(Username) && string.IsNullOrEmpty(Password))
        {
            return;
        }


        //We have not opted for remember me in a previous session, so now we have to login the user with email & password.
        PlayFabClientAPI.LoginWithPlayFab(new LoginWithPlayFabRequest()
        {
            TitleId = PlayFabSettings.TitleId,
            Username = Username,
            Password = Password,
            InfoRequestParameters = InfoRequestParams
        }, (result) =>
        {
            //store identity and session
            _playFabId = result.PlayFabId;
            _sessionTicket = result.SessionTicket;

            RememberMeId = Guid.NewGuid().ToString();
            //Fire and forget, but link the custom ID to this PlayFab Account.
            PlayFabClientAPI.LinkCustomID(new LinkCustomIDRequest()
            {
                CustomId = RememberMeId,
                ForceLink = ForceLink
            }, null, null);


            EventManager.TriggerEvent(EventNames.OnEmailLoggedIn, null);

        }, (error) =>
        {
            EventManager.TriggerEvent(EventNames.OnEmailLogInFail, error);
        });
    }


    void FBLogin(object acessToken)
    {
        Debug.Log("playfab fblogin 1");
        //if (FB.IsLoggedIn)
        {
            PlayFabClientAPI.LoginWithFacebook(new LoginWithFacebookRequest 
            { CreateAccount = true, AccessToken = (string)acessToken , TitleId = PlayFabSettings.TitleId },
                (result) =>
                {
                    Debug.Log("playfab fblogin 2");

                    //store identity and session
                    _playFabId = result.PlayFabId;
                    _sessionTicket = result.SessionTicket;

                    RememberMeId = Guid.NewGuid().ToString();
                    //Fire and forget, but link the custom ID to this PlayFab Account.
                    PlayFabClientAPI.LinkCustomID(new LinkCustomIDRequest()
                    {
                        CustomId = RememberMeId,
                        ForceLink = ForceLink
                    }, null, null);


                EventManager.TriggerEvent(EventNames.GetFbUserName, null);

                }, (error) =>
                {
                EventManager.TriggerEvent(EventNames.OnFBLoggedInFail, error);
                    Debug.Log("playfab fblogin 3");

                });
            //_userName = FB.
        }

    }


    void UpdatePlayFabDisplayName(object nm)
    {
        string name = (string)nm;
        UpdateUserTitleDisplayNameRequest upTit = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = name
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(
            upTit,
            result =>
            {
                Debug.Log(result.ToString());
                //displayName = nm;
                PlayerPrefs.SetString(PlayerPrefKeys.DisplayName, name);
                EventManager.TriggerEvent(EventNames.LoginProcessDone, name);

            },
            (error) =>
            {
            EventManager.TriggerEvent(EventNames.LoginProcessFail, error);
            });
    }

    void getAllProfileData(object userdata)
    {
        string playFabId = _playFabId;

        PlayFabClientAPI.GetPlayerCombinedInfo
        (
            new GetPlayerCombinedInfoRequest()
            {

                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
                {
                    GetPlayerProfile = true,
                    GetPlayerStatistics = true,
                    ProfileConstraints = new PlayerProfileViewConstraints()
                    {
                        ShowDisplayName = true
                },
                PlayerStatisticNames = new List<string> { THuntConstants.PLAYFAB_STAT_SCORE },

                },
                PlayFabId = playFabId
            },
            onPlayerProfileDataRecieved,
            (error) =>
            {
            EventManager.TriggerEvent(EventNames.OnUserScoreReceivedFailed, (object)error);
            });
    }

    void onPlayerProfileDataRecieved(GetPlayerCombinedInfoResult gpr)
    {
        string playFabId = _playFabId;

        string name = gpr.InfoResultPayload.PlayerProfile.DisplayName;
        //CountryCode? country = gpr.InfoResultPayload.PlayerProfile.Locations[0].CountryCode;
        int score = 0;
        if (gpr.InfoResultPayload.PlayerStatistics.Count > 0)
        {
            score = gpr.InfoResultPayload.PlayerStatistics[0].Value;
        }
        //  int win = gpr.InfoResultPayload.;


        PlayerPrefs.SetInt(PlayerPrefKeys.SCORE, score);
        PlayerPrefs.SetString(PlayerPrefKeys.DisplayName, name);

        EventManager.TriggerEvent(EventNames.OnUserScoreReceived, (object)score);
    }

    void DoReadLeaderboard(object userdata)
    {

        PlayFabClientAPI.GetLeaderboard
        (
            new GetLeaderboardRequest()
            {
                MaxResultsCount = 90,
                ProfileConstraints = new PlayerProfileViewConstraints()
                {
                    ShowDisplayName = true
                },
            StatisticName = THuntConstants.PLAYFAB_STAT_SCORE,
                Version = 0

            },
            onDoneReadingLeaderBoard,
            (error) =>
            {
            EventManager.TriggerEvent(EventNames.LeaderboardFailed, (object)error);
            });
    }

    void onDoneReadingLeaderBoard(GetLeaderboardResult res)
    {
        EventManager.TriggerEvent(EventNames.LeaderboardSuccess, (object)res);

    }

    public void UpdatePlayerStatistics(object val)
    {
        PlayFabClientAPI.UpdatePlayerStatistics
        (
            new UpdatePlayerStatisticsRequest()
            {
                Statistics = new List<StatisticUpdate>()
                {
                    new StatisticUpdate()
                    {
                    StatisticName = THuntConstants.PLAYFAB_STAT_SCORE,
                        Version = 0,
                        Value = PlayerPrefs.GetInt(PlayerPrefKeys.SCORE, 0)
    }
                }
            },
            result => Debug.Log("Complete"),
            error => Debug.Log(error.GenerateErrorReport())
        );
    }

    //    private void SilentlyAuthenticate(System.Action<LoginResult> callback = null)
    //    {
    //#if UNITY_ANDROID  && !UNITY_EDITOR

    //        //Get the device id from native android
    //        AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    //        AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity");
    //        AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject>("getContentResolver");
    //        AndroidJavaClass secure = new AndroidJavaClass("android.provider.Settings$Secure");
    //        string deviceId = secure.CallStatic<string>("getString", contentResolver, "android_id");

    //        //Login with the android device ID
    //        PlayFabClientAPI.LoginWithAndroidDeviceID(new LoginWithAndroidDeviceIDRequest() {
    //            TitleId = PlayFabSettings.TitleId,
    //            AndroidDevice = SystemInfo.deviceModel,
    //            OS = SystemInfo.operatingSystem,
    //            AndroidDeviceId = deviceId,
    //            CreateAccount = true,
    //            InfoRequestParameters = InfoRequestParams
    //        }, (result) => {

    //            //Store Identity and session
    //            _playFabId = result.PlayFabId;
    //            _sessionTicket = result.SessionTicket;

    //            //check if we want to get this callback directly or send to event subscribers.
    //            if (callback == null && OnLoginSuccess != null)
    //            {
    //                //report login result back to the subscriber
    //                OnLoginSuccess.Invoke(result);
    //            }else if (callback != null)
    //            {
    //                //report login result back to the caller
    //                callback.Invoke(result);
    //            }
    //        }, (error) => {

    //            //report errro back to the subscriber
    //            if(callback == null && OnPlayFabError != null){
    //                OnPlayFabError.Invoke(error);
    //            }else{
    //                //make sure the loop completes, callback with null
    //                callback.Invoke(null);
    //                //Output what went wrong to the console.
    //                Debug.LogError(error.GenerateErrorReport());
    //            }
    //        });

    //#elif  UNITY_IPHONE || UNITY_IOS && !UNITY_EDITOR
    //        PlayFabClientAPI.LoginWithIOSDeviceID(new LoginWithIOSDeviceIDRequest()
    //        {
    //            TitleId = PlayFabSettings.TitleId,
    //            DeviceModel = SystemInfo.deviceModel,
    //            OS = SystemInfo.operatingSystem,
    //            DeviceId = SystemInfo.deviceUniqueIdentifier,
    //            CreateAccount = true,
    //            InfoRequestParameters = InfoRequestParams
    //        }, (result) => {
    //            //Store Identity and session
    //            _playFabId = result.PlayFabId;
    //            _sessionTicket = result.SessionTicket;

    //            //check if we want to get this callback directly or send to event subscribers.
    //            if (callback == null)
    //            {
    //                //report login result back to the subscriber
    //                //OnLoginSuccess.Invoke(result);
    //            }
    //            else if (callback != null)
    //            {
    //                //report login result back to the caller
    //                callback.Invoke(result);
    //            }
    //        }, (error) => {
    //            //report errro back to the subscriber
    //            if (callback == null)
    //            {
    //                //OnPlayFabError.Invoke(error);
    //            }
    //            else
    //            {
    //                //make sure the loop completes, callback with null
    //                callback.Invoke(null);
    //                //Output what went wrong to the console.
    //                Debug.LogError(error.GenerateErrorReport());
    //            }
    //        });
    //#else
    //        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
    //        {
    //            TitleId = PlayFabSettings.TitleId,
    //            CustomId = SystemInfo.deviceUniqueIdentifier,
    //            CreateAccount = true,
    //            InfoRequestParameters = InfoRequestParams
    //        }, (result) => {
    //            //Store Identity and session
    //            _playFabId = result.PlayFabId;
    //            _sessionTicket = result.SessionTicket;

    //            //check if we want to get this callback directly or send to event subscribers.
    //            if (callback == null && OnLoginSuccess != null)
    //            {
    //                //report login result back to the subscriber
    //                OnLoginSuccess.Invoke(result);
    //            }
    //            else if (callback != null)
    //            {
    //                //report login result back to the caller
    //                callback.Invoke(result);
    //            }
    //        }, (error) => {
    //            //report errro back to the subscriber
    //            if (callback == null && OnPlayFabError != null)
    //            {
    //                OnPlayFabError.Invoke(error);
    //            }
    //            else
    //            {
    //                //make sure the loop completes, callback with null
    //                callback.Invoke(null);
    //                //Output what went wrong to the console.
    //                Debug.LogError(error.GenerateErrorReport());
    //            }

    //        });
    //#endif
    //}
}
