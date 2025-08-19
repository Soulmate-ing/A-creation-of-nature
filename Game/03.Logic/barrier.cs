using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class barrier : MonoBehaviour
{   public GameObject barriers1;
    public GameObject barriers2;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("CreateBarrier");
    }

    // Update is called once per frame
    void Update()
    {
        if (UI.instance.time < 0.1f )
        {
            DestroyAllWithTag("stone");
        }
    }
    IEnumerator CreateBarrier()
    {
        yield return new WaitForSeconds(1);
        for (int i = 0; i < 100; i++)
        {
            switch (Random.Range(0, 3))
            {
                case 0:
                    createbarrier1();
                    createbarrier2();
                    yield return new WaitForSeconds(2);
                    break;
                case 1:
                    createbarrier1();
                    createbarrier3();
                    yield return new WaitForSeconds(2);
                    break;
                case 2:
                    createbarrier2();
                    createbarrier3();
                    yield return new WaitForSeconds(2);
                    break;
                default:
                    break;
            }
        }
    }
    public void createbarrier1()
    {
            Vector3 birthplace = new Vector3(25, Random.Range(-7, -5), 0);
            GameObject B = Instantiate(barriers1, birthplace, Quaternion.identity);
            Destroy(B, 8);
    }
    public void createbarrier2()
    {
        Vector3 birthplace = new Vector3(30, Random.Range(-2, 1), 0);
        GameObject B = Instantiate(barriers2, birthplace, Quaternion.identity);
        Destroy(B, 8);
    }
    public void createbarrier3()
    {
        Vector3 birthplace = new Vector3(35, Random.Range(5,7), 0);
        GameObject B = Instantiate(barriers1, birthplace, Quaternion.identity);
        Destroy(B, 8);
    }
    //销毁所有石头预制体
    public static void DestroyAllWithTag(string tag)
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag(tag))
        {
            DestroyImmediate(obj);
        }
    }

}
