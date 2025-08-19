using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTipUI : MonoBehaviour
{
    private Animator animator; // �洢������Ķ������
    private GameObject animationObject; // �洢����������������

    private void Start()
    {
        // ��ȡ����������������
        animationObject = transform.Find("F").gameObject; // �滻Ϊ�������������
        animator = animationObject.GetComponent<Animator>(); // ��ȡ�������

        // Ĭ�����ض�������
        animationObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ��ʾ��������
            animationObject.SetActive(true);

        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            animationObject.SetActive(false);
        }
    }
}
