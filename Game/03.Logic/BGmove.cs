using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGmove : MonoBehaviour
{
    public float bgspeed;//定义背景的速度
    public float changSize;//背景图移动了多少距离
    public List<GameObject> gos;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left * bgspeed * Time.deltaTime);
        foreach (GameObject go in gos)
        {
            if (go.transform.position.x < changSize)
            {
                go.transform.position = new Vector3(39.7f, transform.position.y, transform.position.z);//44=最后的背景图的位置-移动的数量40+背景图本身的长度21
            }
        }

        if(UI.instance.time < 0.1f || UI.instance.shipweight > UI.instance.fuli)
        {
            bgspeed = 0;
        }
    }
}
