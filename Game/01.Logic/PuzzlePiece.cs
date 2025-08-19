using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    public bool isComplete; //�Ƿ����ƴͼ
    private bool isDragging = false; //�Ƿ�������ק
    private Vector3 offset; //�����λ����ƴͼ�����ĵ�ƫ��
    private Camera cam;
    public Transform targetPosition;
    public float snapThreshold = 1f; //������ֵ
    
    public SoundName puzzle;
    private void Start()
    {
        cam = Camera.main;
    }
    private void OnMouseDown()
    {
        if (isComplete)
            return;
        isDragging = true;
        offset = transform.position - GetMouseWorldPosition();//����ƫ��
    }
    private void OnMouseUp()
    {
        if (isComplete)
            return;
        isDragging = false;
        CheckPosition();
    }
    private void Update()
    {
        if(isDragging)
        {
            transform.position = GetMouseWorldPosition() + offset;//����ƴͼ��λ��
        }
    }
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition; //��ȡ���λ��
        mousePoint.z = cam.WorldToScreenPoint(transform.position).z;
        return cam.ScreenToWorldPoint(mousePoint);
    }
    //���ƴͼ���Ƿ�ӽ�Ŀ��λ��
    private void CheckPosition()
    {
        if(Vector3.Distance(transform.position,targetPosition.position)<snapThreshold)
        {
            transform.position = targetPosition.position; //����ӽ� ��������Ŀ��λ��
            if (puzzle != SoundName.none)
            {
                var soundDetails = AudioManager.Instance.soundDetailsData.GetSoundDetails(puzzle);
                EventHandler.CallInitSoundEffect(soundDetails);
            }
            isComplete = true;
            PuzzleMG.Instance.CheckGameOverCondition();
        }
    }
}
