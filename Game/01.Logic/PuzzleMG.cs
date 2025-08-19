using MyGame.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PuzzleMG : Singleton<PuzzleMG>
{
    public List<PuzzlePiece> puzzles = new List<PuzzlePiece>();
    [Header("游戏结束UI")]
    public GameObject gameOverUI;
    private InventoryUI inventoryUI;
    public static bool PuzzlegameComplete;

    private void Start()
    {
        puzzles.Clear();
        puzzles.AddRange(GetComponentsInChildren<PuzzlePiece>());
        // 获取InventoryUI的引用
        inventoryUI = FindObjectOfType<InventoryUI>();
        // 默认游戏结束UI隐藏
        gameOverUI.SetActive(false);
    }

    private void Update()
    {
        // 检测游戏结束条件
        CheckGameOverCondition();
    }

    public void CheckGameOverCondition()
    {
        // 检查所有拼图是否都已完成
        bool allPuzzlesComplete = true;
        foreach (PuzzlePiece puzzle in puzzles)
        {
            if (!puzzle.isComplete)
            {
                allPuzzlesComplete = false;
                break;
            }
        }
        // 如果所有拼图都已完成
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