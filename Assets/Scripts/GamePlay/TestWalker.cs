using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWalker : MonoBehaviour 
{
    [SerializeField]
    Actions PlayerActions;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log("walking");
            transform.Translate(0,5,0);
            PlayerActions.Walk();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            PlayerActions.RotateLeft();

        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PlayerActions.RotateRight();

        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            PlayerActions.RotateBack();

        }
        else
        {
            PlayerActions.Stay();

        }
    }
}
