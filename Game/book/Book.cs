using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Book : MonoBehaviour
{
    [SerializeField] float pageSpeed = 0.5f;//翻页速度
    [SerializeField] List<Transform> pages;
    int index = -1;
    bool rotate=false;//判断是否翻完页
    [SerializeField] GameObject forwardbutton;
    [SerializeField] GameObject backbutton;
    [SerializeField] GameObject fristpageback;
    [SerializeField] GameObject Lastpageback;
    //往前翻

    public SoundName book;
    private void Start()
    {
        backbutton.SetActive(false);
        fristpageback.SetActive(false);
        Lastpageback.SetActive(false) ;
        
        
    }
    public void RotateForward()
    {
        if (rotate == true) { return; }
        index++;
        float angle = 180;//在Y轴旋转的数值：180度
        ForwardButtonAction();
        pages[index].SetAsLastSibling();//把当前页移动到本地转换列表的最后。
        StartCoroutine(Rotate(angle, true));
        if (book != SoundName.none)
        {
            var soundDetails = AudioManager.Instance.soundDetailsData.GetSoundDetails(book);
            EventHandler.CallInitSoundEffect(soundDetails);
        }
    }
    //激活回翻按钮，隐藏前翻按钮
    public void ForwardButtonAction()
    {
        //如果回翻按钮处于非活动状态
        if (backbutton.activeInHierarchy == false)//activeInHierarchy:如果处于活动状态，则为 True，如果处于非活动状态，则为 false。
        {
            backbutton.SetActive(true);//每当我们向前翻页，后退按钮应该被激活
        }
        if (index == pages.Count - 1)//count是列表元素的个数
        {
            forwardbutton.SetActive(false);
            Lastpageback.SetActive(true);
        }
    }
    //往后翻
    public void RotateBack()
    {
        if (rotate == true) { return; }
        float angle = 0;//在Y轴旋转的数值：0度
        BackbuttonAction();
        pages[index].SetAsLastSibling();
        StartCoroutine(Rotate(angle,false));
        if (book != SoundName.none)
        {
            var soundDetails = AudioManager.Instance.soundDetailsData.GetSoundDetails(book);
            EventHandler.CallInitSoundEffect(soundDetails);
        }
    }
    public void BackbuttonAction()
    {
        
        if (forwardbutton.activeInHierarchy == false)
        {
            forwardbutton.SetActive(true);
        }
        if (index == 0)
        {
            backbutton.SetActive(false);
        }
    }
    IEnumerator Rotate(float angle,bool forward )
    {
        float value = 0f;
        while (true)
        {   rotate=true;
            Quaternion targetRotation=Quaternion.Euler(0,angle,0);//获取旋转的角度（是往回翻or往下翻）
            value += Time.deltaTime * pageSpeed;
            pages[index].rotation = Quaternion.Slerp(pages[index].rotation, targetRotation, value);//平滑翻页
            float angle1=Quaternion.Angle(pages[index].rotation, targetRotation);//计算当前旋转角度和目标角度之间的夹角
            if (angle == 0)
            {
                if (angle1 < 95)
                {
                    pages[index].Find("TEXT").gameObject.SetActive(true);
                    pages[index].Find("TEXT2").gameObject.SetActive(false);
                }
            }
            else if (angle ==180)
            {
                if (angle1 < 95)
                {
                    pages[index].Find("TEXT").gameObject.SetActive(false);
                    pages[index].Find("TEXT2").gameObject.SetActive(true);
                }
            }
            if (angle1 < 0.1f)
            {
                if (forward == false)
                {
                    index--;
                }
                rotate=false;
                break;
            }
            yield return null;
        }
    }
}
