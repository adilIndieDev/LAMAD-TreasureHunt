using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatAndRotate : MonoBehaviour 
{

    [SerializeField] private float rotationSpeed = 50.0f;
    [SerializeField] private float floatAmplitude = 2.0f;
    [SerializeField] private float floatFrequency = 0.5f;

    private Vector3 startPosition;

    // Use this for initialization
    void Start () {
        startPosition = transform.position;
    }
    
    // Update is called once per frame
    void Update () {

        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        Vector3 position = startPosition;
        position.y += Mathf.Sin(Time.fixedTime * Mathf.PI * floatFrequency) * floatAmplitude;
        transform.position = position;
        
    }
}
