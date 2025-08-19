using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class DialogueControl : MonoBehaviour
{
    public static DialogueControl Instance { get; private set; }


    [Header("NPC类型按钮")]
    public GameObject ShuiCheButton;  // 水车NPC按钮
    public GameObject KuangShiButton; // 矿石NPC按钮
    public GameObject ChuanButton;    // 船NPC按钮
    public GameObject PulleyButton;
    public GameObject EscDialogueButton;
    public GameObject SuiPianButton;
    private Talkable.NPCType currentNPCType; // 记录当前NPC类型
    [Header("主角小林标识")]
    public Sprite xiaolinImage;
    [Header("UI组件")]
    [Tooltip("对话框根物体（需包含Text组件）")]
    [SerializeField] public GameObject dialogueBox;

    [Tooltip("文本显示组件")]
    [SerializeField] private Text dialogueText;
    public Text rightnameText;
    public Image rightnameImage;
    public Text leftnameText;
    public Image leftnameImage;


    private Coroutine typingCoroutine;
    private bool IsTyping => typingCoroutine != null;

    private string[] currentDialogue;

    private int currentLineIndex;

    private void Awake()
    {
        // 单例初始化
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void ShowDialogue(string[] dialogueLines)
    {
        currentDialogue = dialogueLines;
        currentLineIndex = 0;
        dialogueBox.SetActive(true);
        CheckName();

        typingCoroutine = StartCoroutine(TypewriterEffect());
    }


    private IEnumerator TypewriterEffect()
    {
        dialogueText.text = string.Empty;

        // 逐字显示当前行
        foreach (char character in currentDialogue[currentLineIndex])
        {
            dialogueText.text += character;
            yield return new WaitForSeconds(0.05f); // 每个字符间隔0.05秒

        }
        typingCoroutine = null;// 标记完成
    }


    private void Update()
    {

        if (!dialogueBox.activeInHierarchy) return;

        if (Input.GetMouseButtonDown(0))
        {
            // 正在打字时：立即完成当前行
            if (IsTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = currentDialogue[currentLineIndex];
                typingCoroutine = null;
            }
            // 未打字时：推进对话
            else
            {
                if (currentLineIndex < currentDialogue.Length - 1)
                {
                    // 推进到下一行
                    currentLineIndex++;
                    CheckName();
                    typingCoroutine = StartCoroutine(TypewriterEffect());
                }
                else
                {

                    if (Talkable.currentTalker != null)
                    {
                        currentNPCType = Talkable.currentTalker.npcType; // 记录当前NPC类型
                        ShowTypeSpecificButton(); // 根据类型显示按钮
                        EscDialogueButton.SetActive(true);
                    }
                }
            }
        }
    }

    private void ShowTypeSpecificButton()
    {
        switch (currentNPCType)
        {
            case Talkable.NPCType.VillageChief:
                TaskManager.Instance.UpdateTaskState(0);
                TaskManager.Instance.MujiangTask.SetActive(true);
                TaskManager.Instance.TiejiangTask.SetActive(true);
                TaskManager.Instance.yuminTask.SetActive(true);
                if (Talkable.TotalcompletedNPCs > 2) 
                {
                    SuiPianButton.SetActive(true);
                }
                // 当完成最终对话时更新任务状态
                break;
            case Talkable.NPCType.ShuiCheNPC:
                ShuiCheButton.SetActive(true);
                break;
            case Talkable.NPCType.KuangShiNPC:
                KuangShiButton.SetActive(true);
                break;
            case Talkable.NPCType.ChuanNPC:
                ChuanButton.SetActive(true);
                break;
            case Talkable.NPCType.PulleyNPC:
                PulleyButton.SetActive(true);
                break;
            default:
                break;
        }
    }
    private void CheckName()
    {
        if (currentDialogue == null || currentLineIndex >= currentDialogue.Length)
        {
            Debug.LogError("对话内容未初始化或索引越界！");
            return;
        }
        if (currentDialogue[currentLineIndex].StartsWith("n-"))
        {
            string speakerTag = currentDialogue[currentLineIndex].Substring(2); // 提取"n-后"的内容

            // 小林对话显示在右侧
            if (speakerTag == "云策")
            {
                rightnameText.text = "云策";
                rightnameImage.sprite = xiaolinImage;
                rightnameImage.gameObject.SetActive(true);
                rightnameText.gameObject.SetActive(true);

                leftnameImage.gameObject.SetActive(false);
                leftnameText.gameObject.SetActive(false);
            }
            // 其他NPC对话显示在左侧
            else
            {
                leftnameText.text = Talkable.currentTalker.npcName;
                leftnameImage.sprite = Talkable.currentTalker.npcImage;
                leftnameImage.gameObject.SetActive(true);
                leftnameText.gameObject.SetActive(true);

                rightnameImage.gameObject.SetActive(false);
                rightnameText.gameObject.SetActive(false);
            }
            Debug.Log($"【初始化】当前行索引: {currentLineIndex}, 首行内容: {currentDialogue[0]}");
            currentLineIndex++; // 跳过名称行

        }
    }


}