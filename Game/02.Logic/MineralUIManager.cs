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
    public TMP_Text failureCountText; // ����ʧ�ܴ����ı�
    public GameObject startPanel;
    public static bool KuangshigameComplete;

    private void Start()
    {
        startPanel.SetActive(true);  // Ĭ�ϴ򿪿�ʼ���
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
        failureCountText.text = $"ʧ�ܴ�����{max - current}/{max}";
    }

    public void ShowWinBoard()
    {
        GameOverUI.SetActive(true);
        title.text = "ұ���ɹ�";
        MineralUIManager.KuangshigameComplete =true;
        Talkable.TotalcompletedNPCs++;
        TaskManager.Instance.UpdateTaskState(2);
    }

    public void ShowFalseBoard()
    {
        GameOverUI.SetActive(true);
        title.text = "ұ��ʧ��";
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