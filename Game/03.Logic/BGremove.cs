using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGremove : MonoBehaviour
{
    public float changSize;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x <-changSize)
        {
            transform.position=new Vector3(changSize*2,transform.position.y,transform.position.z);
        }
    }
}
