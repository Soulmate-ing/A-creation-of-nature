using UnityEngine;
using UnityEngine.UI;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance;

    [Header("任务面板")]
    public GameObject taskPanel; // 拖拽任务面板到此

    [Header("任务状态文本")]
    public Text[] taskStatusTexts; // 按顺序绑定状态文本
    public GameObject MujiangTask;
    public GameObject TiejiangTask;
    public GameObject yuminTask;
    public GameObject CunzhangLastTask;
    public GameObject HeshenTask;
    public GameObject HeshenLastTask;


    void Awake()
    {

        PlayerPrefs.DeleteAll(); // 清除存档
        FindObjectOfType<Talkable>().GetType().TypeInitializer?.Invoke(null, null); // 重置静态变量

        if (Instance == null)
        {
            Instance = this;
            LoadTaskStates();

        }
        else
        {
            Destroy(gameObject);
            Destroy(taskPanel);
        }
        MujiangTask.SetActive(false);
        TiejiangTask.SetActive(false);
        yuminTask.SetActive(false);
        CunzhangLastTask.SetActive(false);
        HeshenTask.SetActive(false);
        HeshenLastTask.SetActive(false);
        // 确保面板跨场景
        DontDestroyOnLoad(taskPanel);
        DontDestroyOnLoad(gameObject);

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !DialogueControl.Instance.dialogueBox.activeInHierarchy)
        {
            // 直接控制面板显隐
            taskPanel.SetActive(!taskPanel.activeSelf);
        }
        if (PuzzleMG.PuzzlegameComplete && MineralUIManager.KuangshigameComplete && UI.ChuangameComplete)
        {
            CunzhangLastTask.SetActive(true);
        }
        if (BucketRopeController.PulleygameComplete == true)
            HeshenLastTask.SetActive(true);
    }

    public void UpdateTaskState(int taskIndex)
    {
        if (taskIndex >= 0 && taskIndex < taskStatusTexts.Length && taskStatusTexts[taskIndex] != null)
        {
            taskStatusTexts[taskIndex].text = "已完成";
            taskStatusTexts[taskIndex].color = Color.green;
            PlayerPrefs.SetInt($"Task{taskIndex}", 1);
        }
    }

    void LoadTaskStates()
    {
        for (int i = 0; i < taskStatusTexts.Length; i++)
        {
            if (PlayerPrefs.GetInt($"Task{i}", 0) == 1)
            {
                taskStatusTexts[i].text = "已完成";
                taskStatusTexts[i].color = Color.green;
            }
        }
    }
    public void EscTaskPanel()
    {
        taskPanel.SetActive(false);
    }
}