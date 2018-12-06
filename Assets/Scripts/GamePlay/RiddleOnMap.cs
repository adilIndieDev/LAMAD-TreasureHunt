using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiddleOnMap : MonoBehaviour 
{

    //private void Start()
    //{
    //    EventManager.TriggerEvent(EventNames.OnRiddleCreated,(object)this);
    //}

    private void OnMouseDown()
    {
        SoundManager.Instance.PlayClickSound();
        EventManager.TriggerEvent(EventNames.OnRiddleCreated, (object)this);

        EventManager.TriggerEvent(EventNames.OpenCaptureScene,(object)this);
    }
}
