using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGmove : MonoBehaviour
{
    public float bgspeed;//���屳�����ٶ�
    public float changSize;//����ͼ�ƶ��˶��پ���
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
                go.transform.position = new Vector3(39.7f, transform.position.y, transform.position.z);//44=���ı���ͼ��λ��-�ƶ�������40+����ͼ����ĳ���21
            }
        }

        if(UI.instance.time < 0.1f || UI.instance.shipweight > UI.instance.fuli)
        {
            bgspeed = 0;
        }
    }
}
