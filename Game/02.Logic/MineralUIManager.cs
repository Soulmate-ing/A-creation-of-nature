using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MineralUIManager : MonoBehaviour
{
    public GameObject success;
    public GameObject falsegame;
    public GameObject GameOverUI;
    public Text title;
    public Slider jindutiao;
    public TMP_Text failureCountText; // 新增失败次数文本
    public GameObject startPanel;
    public static bool KuangshigameComplete;

    private void Start()
    {
        startPanel.SetActive(true);  // 默认打开开始面板
        jindutiao.value = 0f;
        GameOverUI.SetActive(false);
        success.SetActive(false);
        falsegame.SetActive(false);
    }
    public bool IsAnyPanelActive()
    {
        return startPanel.activeSelf ||
               GameOverUI.activeSelf ||
               success.activeSelf ||
               falsegame.activeSelf;
    }
    public void StartGame()
    {
        startPanel.SetActive(false);
    }
    public void UpdateFailureCount(int current, int max)
    {
        failureCountText.text = $"失败次数：{max - current}/{max}";
    }

    public void ShowWinBoard()
    {
        GameOverUI.SetActive(true);
        title.text = "冶炼成功";
        MineralUIManager.KuangshigameComplete =true;
        Talkable.TotalcompletedNPCs++;
        TaskManager.Instance.UpdateTaskState(2);
    }

    public void ShowFalseBoard()
    {
        GameOverUI.SetActive(true);
        title.text = "冶炼失败";
    }

    public void UpdateProgress(float value)
    {
        jindutiao.value = value;
    }

    public void ShowStartPanel()
    {
        startPanel.SetActive(true);
    }

    public void ResetUI(int maxFailures)
    {
        jindutiao.value = 0f;
        GameOverUI.SetActive(false);
        success.SetActive(false);
        falsegame.SetActive(false);
        UpdateFailureCount(0, maxFailures);
    }
}