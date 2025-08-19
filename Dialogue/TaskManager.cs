using UnityEngine;
using UnityEngine.UI;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance;

    [Header("�������")]
    public GameObject taskPanel; // ��ק������嵽��

    [Header("����״̬�ı�")]
    public Text[] taskStatusTexts; // ��˳���״̬�ı�
    public GameObject MujiangTask;
    public GameObject TiejiangTask;
    public GameObject yuminTask;
    public GameObject CunzhangLastTask;
    public GameObject HeshenTask;
    public GameObject HeshenLastTask;


    void Awake()
    {

        PlayerPrefs.DeleteAll(); // ����浵
        FindObjectOfType<Talkable>().GetType().TypeInitializer?.Invoke(null, null); // ���þ�̬����

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
        // ȷ�����糡��
        DontDestroyOnLoad(taskPanel);
        DontDestroyOnLoad(gameObject);

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !DialogueControl.Instance.dialogueBox.activeInHierarchy)
        {
            // ֱ�ӿ����������
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
            taskStatusTexts[taskIndex].text = "�����";
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
                taskStatusTexts[i].text = "�����";
                taskStatusTexts[i].color = Color.green;
            }
        }
    }
    public void EscTaskPanel()
    {
        taskPanel.SetActive(false);
    }
}