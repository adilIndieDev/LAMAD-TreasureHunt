using System.Collections.Generic;
using UnityEngine;
using System;
using Facebook.Unity;

public class FBLogin : MonoBehaviour 
{

    private void Awake()
    {

        if (!FB.IsInitialized)
        {
            FB.Init(() =>
            {
                if (FB.IsInitialized)
                {
                    FB.ActivateApp();
                    loginAuto();
                }
                else
                {
                    Debug.Log("Couldn't initialize");
                }
            },
                isGameShown =>
                {
                    if (!isGameShown)
                        Time.timeScale = 0;
                    else
                        Time.timeScale = 1;
                });
        }
        else
        {
            FB.ActivateApp();
        }
    }

    private void OnEnable()
    {
        EventManager.StartListening(EventNames.StartFBLoginProcess, FacebookLogin);
        EventManager.StartListening(EventNames.GetFbUserName, FacebookFetchUserData );

    }

    private void OnDisable()
    {
        EventManager.StopListening(EventNames.StartFBLoginProcess, FacebookLogin);
        EventManager.StopListening(EventNames.GetFbUserName, FacebookFetchUserData);

    }

    void loginAuto()
    {
        if (FB.IsLoggedIn)
        {
            var aToken = AccessToken.CurrentAccessToken;
            Debug.Log("Auto Logged In");
        }
        else
        {
            Debug.Log("User cancelled login");
        }
        FB.ActivateApp();
    }

    void FacebookLogin(object userdata)
    {
        var permissions = new List<string>() { "public_profile", "email", "user_friends" };
        FB.LogInWithReadPermissions(permissions, HandleResult);
    }

    public void FacebookLogout()
    {
        FB.LogOut();
    }

    protected void HandleResult(IResult result)
    {
        string LastResponse = "";
        if (result == null)
        {
            LastResponse = "Null Response\n";
            //  LogView.AddLog(this.LastResponse);
            Debug.Log(LastResponse);
            EventManager.TriggerEvent(EventNames.OnFBLoggedInFail,null);
            return;
        }


        if (!string.IsNullOrEmpty(result.Error))
        {
            LastResponse = "Error Response:\n" + result.Error;
            Debug.Log(LastResponse);
        }
        else if (result.Cancelled)
        {
            LastResponse = "Cancelled Response:\n" + result.RawResult;
            Debug.Log(LastResponse);
        }
        else if (!string.IsNullOrEmpty(result.RawResult))
        {
            LastResponse = "Success Response:\n" + result.RawResult;
            Debug.Log(LastResponse);
        }
        else
        {
            LastResponse = "Empty Response\n";
            Debug.Log(LastResponse);
        }

        if (FB.IsLoggedIn)
        {
            //Debug.Log(LastResponse);
            string aToken = AccessToken.CurrentAccessToken.TokenString;
            EventManager.TriggerEvent(EventNames.PlayFabLoginFB, (object)aToken);
        }
        else
        {
            EventManager.TriggerEvent(EventNames.OnFBLoggedInFail, null);

        }

    }

    void FacebookFetchUserData(object userdata)
    {
        Debug.Log("FacebookFetchUserData");
        FB.API("/me?fields=first_name", HttpMethod.GET, result =>
        {

            string _name = result.ResultDictionary["first_name"].ToString();
            Debug.Log("FacebookFetchUserData: " + _name);

            EventManager.TriggerEvent(EventNames.OnGotFBUserName, _name);
        });

    }

}
