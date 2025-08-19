using MyGame.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PuzzleMG : Singleton<PuzzleMG>
{
    public List<PuzzlePiece> puzzles = new List<PuzzlePiece>();
    [Header("��Ϸ����UI")]
    public GameObject gameOverUI;
    private InventoryUI inventoryUI;
    public static bool PuzzlegameComplete;

    private void Start()
    {
        puzzles.Clear();
        puzzles.AddRange(GetComponentsInChildren<PuzzlePiece>());
        // ��ȡInventoryUI������
        inventoryUI = FindObjectOfType<InventoryUI>();
        // Ĭ����Ϸ����UI����
        gameOverUI.SetActive(false);
    }

    private void Update()
    {
        // �����Ϸ��������
        CheckGameOverCondition();
    }

    public void CheckGameOverCondition()
    {
        // �������ƴͼ�Ƿ������
        bool allPuzzlesComplete = true;
        foreach (PuzzlePiece puzzle in puzzles)
        {
            if (!puzzle.isComplete)
            {
                allPuzzlesComplete = false;
                break;
            }
        }
        // �������ƴͼ�������
        if (allPuzzlesComplete)
        {
            gameOverUI.SetActive(true);
            PuzzleMG.PuzzlegameComplete = true;
            Talkable.TotalcompletedNPCs++;
            TaskManager.Instance.UpdateTaskState(1);
            if (inventoryUI.bagOpened)
            {
                gameOverUI.SetActive(false);
            }
            else
            {
                gameOverUI.SetActive(true);
            }
        }
    }
}