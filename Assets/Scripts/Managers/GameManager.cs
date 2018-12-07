using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour 
{
    RiddleOnMap rMAp;
    
    [SerializeField]
    GameObject MapObject;

    [SerializeField]
    GameObject RiddleObject;


    [SerializeField]
    GameObject UIRiddleObject;

    PlayerR player;

    [SerializeField]
    TMP_Text UserName;

    [SerializeField]
    TMP_Text Score;

    [SerializeField]
    AbstractMap _map;

    [SerializeField]
    Loading _loading;


    [SerializeField]
    Camera MapCamera;

    [SerializeField]
    Camera RiddleCamera;

    private void Start()
    {
        Debug.Log("Start GameManager");
        RiddleObject.SetActive(false);
        UserName.text = PlayerPrefs.GetString(PlayerPrefKeys.DisplayName, "user");
        Score.text = PlayerPrefs.GetInt(PlayerPrefKeys.SCORE, 0).ToString();
        _loading.ShowSelf(null);
        _map.OnInitialized += DisableLoading;

    }

    void DisableLoading()
    {
        _loading.HideSelf(null);

    }

    private void OnEnable()
    {
        EventManager.StartListening(EventNames.OpenCaptureScene, OnRiddleClick);
        EventManager.StartListening(EventNames.OnRiddleSolved, RiddleSolved);
        EventManager.StartListening(EventNames.OnRiddleFailed, RiddleFailed);
        EventManager.StartListening(EventNames.OnRiddleCreated, AddRiddle);
//        EventManager.StartListening(EventNames.MapUpdated, RemoveRiddles);

    }

    private void OnDisable()
    {
        EventManager.StopListening(EventNames.OpenCaptureScene, OnRiddleClick);
        EventManager.StopListening(EventNames.OnRiddleSolved, RiddleSolved);
        EventManager.StopListening(EventNames.OnRiddleFailed, RiddleFailed);
        EventManager.StopListening(EventNames.OnRiddleCreated, AddRiddle);
    //    EventManager.StopListening(EventNames.MapUpdated, RemoveRiddles);

    }

    void OnRiddleClick(object riddle)
    {
        //RidlleToDelete = (RiddleOnMap)riddle;

        MapObject.SetActive(false);
        RiddleObject.SetActive(true);
        UIRiddleObject.SetActive(true);
        MapCamera.GetComponent<AudioListener>().enabled = false;
        RiddleCamera.GetComponent<AudioListener>().enabled = true;

    }

    void RiddleSolved(object o)
    {


        RiddleObject.SetActive(false);
        UIRiddleObject.SetActive(false);

        MapObject.SetActive(true);
       // RidllesOnMap.Remove(RidlleToDelete);
        //Destroy(RidlleToDelete.gameObject);
        int score = PlayerPrefs.GetInt(PlayerPrefKeys.SCORE, 0) + 50;

        PlayerPrefs.SetInt(PlayerPrefKeys.SCORE, score);
        Score.text = PlayerPrefs.GetInt(PlayerPrefKeys.SCORE, 0).ToString();
        Destroy(rMAp.gameObject);
        EventManager.TriggerEvent(EventNames.UpdatedPlayerScoreUI,null);

        MapCamera.GetComponent<AudioListener>().enabled = true;
        RiddleCamera.GetComponent<AudioListener>().enabled = false;
    }

    void RiddleFailed(object o)
    {
        RiddleObject.SetActive(false);
        UIRiddleObject.SetActive(false);

        MapObject.SetActive(true);
       
        MapCamera.GetComponent<AudioListener>().enabled = true;
        RiddleCamera.GetComponent<AudioListener>().enabled = false;
        //RidlleToDelete = null;
    }

    void AddRiddle(object o)
    {
        // RidllesOnMap.Add((RiddleOnMap)o);
        rMAp = (RiddleOnMap)o;
    }

    //void RemoveRiddles(object userdata)
    //{
    //    for (int i = 0; i < RidllesOnMap.Count; i++)
    //    {
    //        RidllesOnMap[i].gameObject.SetActive(false);
    //    }

    //   // RidllesOnMap.Clear();
    //}

    public void onMainMenu()
    {
        SceneManager.LoadScene(THuntConstants.MainMenu);
    }
}
