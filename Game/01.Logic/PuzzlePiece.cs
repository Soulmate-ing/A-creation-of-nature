using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    public bool isComplete; //是否完成拼图
    private bool isDragging = false; //是否正在拖拽
    private Vector3 offset; //鼠标点击位置与拼图块中心的偏移
    private Camera cam;
    public Transform targetPosition;
    public float snapThreshold = 1f; //吸附阈值
    
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
        offset = transform.position - GetMouseWorldPosition();//计算偏移
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
            transform.position = GetMouseWorldPosition() + offset;//更新拼图块位置
        }
    }
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition; //获取鼠标位置
        mousePoint.z = cam.WorldToScreenPoint(transform.position).z;
        return cam.ScreenToWorldPoint(mousePoint);
    }
    //检查拼图块是否接近目标位置
    private void CheckPosition()
    {
        if(Vector3.Distance(transform.position,targetPosition.position)<snapThreshold)
        {
            transform.position = targetPosition.position; //如果接近 则吸附到目标位置
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
