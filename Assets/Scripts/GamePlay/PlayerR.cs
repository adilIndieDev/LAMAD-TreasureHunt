using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerR : MonoBehaviour 
{
    //public Material[] Materials;
    public Transform Target;
    Actions ActionPerformer;
    public float Speed;
   // AstronautMouseController _controller;
    void Start()
    {
        // _controller = GetComponent<AstronautMouseController>();
        ActionPerformer = GetComponent<Actions>();
    }

    void Update()
    {

        //if (_controller.enabled)// Because the mouse control script interferes with this script
        //{
        //    return;
        //}

        //foreach (var item in Materials)
        //{
        //    item.SetVector("_CharacterPosition", transform.position);
        //}

        var distance = Vector3.Distance(transform.position, Target.position);
        if (distance > 0.1f)
        {
            transform.LookAt(Target.position);
            transform.Translate(Vector3.forward * Speed);
            ActionPerformer.Walk();
        }
        else
        {
            ActionPerformer.Stay();

        }


        //if (Input.GetKeyDown(KeyCode.RightArrow))
        //{
        //    PlayerActions.RotateLeft();

        //}
        //if (Input.GetKeyDown(KeyCode.LeftArrow))
        //{
        //    PlayerActions.RotateRight();

        //}

        //if (Input.GetKeyDown(KeyCode.DownArrow))
        //{
        //    PlayerActions.RotateBack();

        //}
        //else
        //{
        //    PlayerActions.Stay();

        //}
    }
}
