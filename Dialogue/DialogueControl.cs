using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class DialogueControl : MonoBehaviour
{
    public static DialogueControl Instance { get; private set; }


    [Header("NPC���Ͱ�ť")]
    public GameObject ShuiCheButton;  // ˮ��NPC��ť
    public GameObject KuangShiButton; // ��ʯNPC��ť
    public GameObject ChuanButton;    // ��NPC��ť
    public GameObject PulleyButton;
    public GameObject EscDialogueButton;
    public GameObject SuiPianButton;
    private Talkable.NPCType currentNPCType; // ��¼��ǰNPC����
    [Header("����С�ֱ�ʶ")]
    public Sprite xiaolinImage;
    [Header("UI���")]
    [Tooltip("�Ի�������壨�����Text�����")]
    [SerializeField] public GameObject dialogueBox;

    [Tooltip("�ı���ʾ���")]
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
        // ������ʼ��
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

        // ������ʾ��ǰ��
        foreach (char character in currentDialogue[currentLineIndex])
        {
            dialogueText.text += character;
            yield return new WaitForSeconds(0.05f); // ÿ���ַ����0.05��

        }
        typingCoroutine = null;// ������
    }


    private void Update()
    {

        if (!dialogueBox.activeInHierarchy) return;

        if (Input.GetMouseButtonDown(0))
        {
            // ���ڴ���ʱ��������ɵ�ǰ��
            if (IsTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = currentDialogue[currentLineIndex];
                typingCoroutine = null;
            }
            // δ����ʱ���ƽ��Ի�
            else
            {
                if (currentLineIndex < currentDialogue.Length - 1)
                {
                    // �ƽ�����һ��
                    currentLineIndex++;
                    CheckName();
                    typingCoroutine = StartCoroutine(TypewriterEffect());
                }
                else
                {

                    if (Talkable.currentTalker != null)
                    {
                        currentNPCType = Talkable.currentTalker.npcType; // ��¼��ǰNPC����
                        ShowTypeSpecificButton(); // ����������ʾ��ť
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
                // ��������նԻ�ʱ��������״̬
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
            Debug.LogError("�Ի�����δ��ʼ��������Խ�磡");
            return;
        }
        if (currentDialogue[currentLineIndex].StartsWith("n-"))
        {
            string speakerTag = currentDialogue[currentLineIndex].Substring(2); // ��ȡ"n-��"������

            // С�ֶԻ���ʾ���Ҳ�
            if (speakerTag == "�Ʋ�")
            {
                rightnameText.text = "�Ʋ�";
                rightnameImage.sprite = xiaolinImage;
                rightnameImage.gameObject.SetActive(true);
                rightnameText.gameObject.SetActive(true);

                leftnameImage.gameObject.SetActive(false);
                leftnameText.gameObject.SetActive(false);
            }
            // ����NPC�Ի���ʾ�����
            else
            {
                leftnameText.text = Talkable.currentTalker.npcName;
                leftnameImage.sprite = Talkable.currentTalker.npcImage;
                leftnameImage.gameObject.SetActive(true);
                leftnameText.gameObject.SetActive(true);

                rightnameImage.gameObject.SetActive(false);
                rightnameText.gameObject.SetActive(false);
            }
            Debug.Log($"����ʼ������ǰ������: {currentLineIndex}, ��������: {currentDialogue[0]}");
            currentLineIndex++; // ����������

        }
    }


}