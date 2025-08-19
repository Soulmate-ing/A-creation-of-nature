using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class barriermove : MonoBehaviour
{
    Transform barrier;
    public float Speed;

    // Start is called before the first frame update
    void Start()
    {
        barrier = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        // 只有在游戏开始时才移动障碍物
        if (UI.instance.isGameStarted)
        {
            speedup1();
        }
    }

    // 15秒开始加速
    public void speedup1()
    {
        if (UI.instance.time > 15 && UI.instance.time < 17)
        {
            UI.instance.speedupword.SetActive(true);
        }
        else
        {
            UI.instance.speedupword.SetActive(false);
        }

        if (UI.instance.time < 16) // 15秒开始加速
        {
            barrier.Translate(Vector3.left * Speed * Time.deltaTime * 1.4f);
        }
        else
        {
            barrier.Translate(Vector3.left * Speed * Time.deltaTime * 1.2f);
        }
        if(UI.instance.shipweight > UI.instance.fuli)
        {
            Speed = 0;
        }
    }
}