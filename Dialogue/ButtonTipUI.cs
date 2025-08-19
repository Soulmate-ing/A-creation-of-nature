using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTipUI : MonoBehaviour
{
    private Animator animator; // 存储子物体的动画组件
    private GameObject animationObject; // 存储包含动画的子物体

    private void Start()
    {
        // 获取包含动画的子物体
        animationObject = transform.Find("F").gameObject; // 替换为你的子物体名称
        animator = animationObject.GetComponent<Animator>(); // 获取动画组件

        // 默认隐藏动画对象
        animationObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 显示动画对象
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
