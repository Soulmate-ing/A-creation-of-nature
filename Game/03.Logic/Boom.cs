using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boom : MonoBehaviour
{
    // Start is called before the first frame update
    public void closeboom()
    {
        gameObject.SetActive(false);
    }
}
