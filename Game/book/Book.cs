using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Book : MonoBehaviour
{
    [SerializeField] float pageSpeed = 0.5f;//��ҳ�ٶ�
    [SerializeField] List<Transform> pages;
    int index = -1;
    bool rotate=false;//�ж��Ƿ���ҳ
    [SerializeField] GameObject forwardbutton;
    [SerializeField] GameObject backbutton;
    [SerializeField] GameObject fristpageback;
    [SerializeField] GameObject Lastpageback;
    //��ǰ��

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
        float angle = 180;//��Y����ת����ֵ��180��
        ForwardButtonAction();
        pages[index].SetAsLastSibling();//�ѵ�ǰҳ�ƶ�������ת���б�����
        StartCoroutine(Rotate(angle, true));
        if (book != SoundName.none)
        {
            var soundDetails = AudioManager.Instance.soundDetailsData.GetSoundDetails(book);
            EventHandler.CallInitSoundEffect(soundDetails);
        }
    }
    //����ط���ť������ǰ����ť
    public void ForwardButtonAction()
    {
        //����ط���ť���ڷǻ״̬
        if (backbutton.activeInHierarchy == false)//activeInHierarchy:������ڻ״̬����Ϊ True��������ڷǻ״̬����Ϊ false��
        {
            backbutton.SetActive(true);//ÿ��������ǰ��ҳ�����˰�ťӦ�ñ�����
        }
        if (index == pages.Count - 1)//count���б�Ԫ�صĸ���
        {
            forwardbutton.SetActive(false);
            Lastpageback.SetActive(true);
        }
    }
    //����
    public void RotateBack()
    {
        if (rotate == true) { return; }
        float angle = 0;//��Y����ת����ֵ��0��
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
            Quaternion targetRotation=Quaternion.Euler(0,angle,0);//��ȡ��ת�ĽǶȣ������ط�or���·���
            value += Time.deltaTime * pageSpeed;
            pages[index].rotation = Quaternion.Slerp(pages[index].rotation, targetRotation, value);//ƽ����ҳ
            float angle1=Quaternion.Angle(pages[index].rotation, targetRotation);//���㵱ǰ��ת�ǶȺ�Ŀ��Ƕ�֮��ļн�
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
