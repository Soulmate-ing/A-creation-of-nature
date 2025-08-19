using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarFollow : MonoBehaviour
{
    public Transform followtarget;//跟随的目标
    public Vector3 offset;//偏移量
    public Camera observationCamera;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   if(followtarget == null)
        {
            return;
        }
        if(observationCamera == null)
        {
            observationCamera=Camera.main;
        }
        transform.position = observationCamera.WorldToScreenPoint(followtarget.position+offset);
    }
}
