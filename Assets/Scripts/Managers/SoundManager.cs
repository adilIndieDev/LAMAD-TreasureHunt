using System.Collections.Generic;
using UnityEngine;
public class SoundManager : MonoBehaviour 
{
    [SerializeField]
    AudioClip[] ClickSounds;

    [SerializeField]
    AudioClip RightAnswer;

    [SerializeField]
    AudioClip WrongAnswer;

    [SerializeField]
    AudioClip RiddleSolved;

    [SerializeField]
    AudioSource BgMusic;
   // public AudioSource BgMusic;

    int ClickCounter = 0;
    //  bool firing = false;
    private static SoundManager _instance;

    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<SoundManager>();

                //Tell unity not to destroy this object when loading a new scene!
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    void Awake()
    {
       // PlayerPrefs.DeleteAll();
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

    public void TurnOffMusic()
    {
        //BgMusic.Stop();
        PlayerPrefs.SetInt(PlayerPrefKeys.MUSIC_PREF, 0);
    }

    public void TurnOnMusic()
    {
        //BgMusic.Play();
        PlayerPrefs.SetInt(PlayerPrefKeys.MUSIC_PREF, 1);
    }

    public void TurnOffSound()
    {
        PlayerPrefs.SetInt(PlayerPrefKeys.SOUND_PREF, 0);
    }

    public void TurnOnSound()
    {
        PlayerPrefs.SetInt(PlayerPrefKeys.SOUND_PREF, 1);
    }

    public void PlayClickSound()
    {
        if (PlayerPrefs.GetInt(PlayerPrefKeys.SOUND_PREF, 1) == 1)
        {
            if(ClickCounter>=(ClickSounds.Length))
            {
                ClickCounter = 0;
            }

            AudioSource.PlayClipAtPoint(ClickSounds[ClickCounter], transform.position);
            ClickCounter++;
        }
    }

    public void PlayRightAnswerdSound()
    {
        if (PlayerPrefs.GetInt(PlayerPrefKeys.SOUND_PREF, 1) == 1)
        {
            AudioSource.PlayClipAtPoint(RightAnswer, transform.position);
        }
    }

    public void PlayWrongAnswerdSound()
    {
        if (PlayerPrefs.GetInt(PlayerPrefKeys.SOUND_PREF, 1) == 1)
        {
            AudioSource.PlayClipAtPoint(WrongAnswer, transform.position);
        }
    }


    public void PlayRiddleSolvedSound()
    {
        if (PlayerPrefs.GetInt(PlayerPrefKeys.SOUND_PREF, 1) == 1)
        {
            AudioSource.PlayClipAtPoint(RiddleSolved, transform.position);
        }
    }

    public void PauseMusic()
    {
        if (PlayerPrefs.GetInt(PlayerPrefKeys.MUSIC_PREF, 1) == 1)
        {
            //BgMusic.Pause();
        }
    }

    public void PlayMusic()
    {
        if (PlayerPrefs.GetInt(PlayerPrefKeys.MUSIC_PREF, 1) == 1)
        {
            //BgMusic.Play();
        }
    }
}
