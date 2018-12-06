using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MapViewUI : MonoBehaviour 
{
    [SerializeField]
    TMP_Text UserName;

    [SerializeField]
    TMP_Text Score;
    // Use this for initialization
    void Start () 
    {
        UserName.text = PlayerPrefs.GetString(PlayerPrefKeys.USERNAME, "user");
        Score.text = PlayerPrefs.GetInt(PlayerPrefKeys.SCORE, 0).ToString();
    }

    private void OnEnable()
    {
        EventManager.StartListening(EventNames.UpdatedPlayerScoreUI, updateScore);
    }

    void updateScore(object userdata)
    {
        Score.text = PlayerPrefs.GetInt(PlayerPrefKeys.SCORE, 0).ToString();

    }
}
