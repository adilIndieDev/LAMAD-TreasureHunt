using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loading : MonoBehaviour 
{
    [SerializeField]
    Transform loadImg;


    private void OnEnable()
    {
        EventManager.StartListening(EventNames.ShowLoading, ShowSelf);
        EventManager.StartListening(EventNames.HideLoading, HideSelf);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventNames.ShowLoading, ShowSelf);
        EventManager.StopListening(EventNames.HideLoading, HideSelf);
    }

    public void ShowSelf(object userdata)
    {
        iTween.RotateBy(loadImg.gameObject, iTween.Hash("z", 20.0f, "looptype", iTween.LoopType.loop));
        gameObject.SetActive(true);
    }

    public void HideSelf(object userdata)
    {
        gameObject.SetActive(false);
        iTween.Stop(loadImg.gameObject);
    }
}
