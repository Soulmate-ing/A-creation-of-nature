// Talkable.cs
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Talkable : MonoBehaviour

{
    [Header("NPC�����Ϣ")]
    public string npcName = "Ĭ��NPC"; // NPC��ʾ����
    public Sprite npcImage;           // NPCͷ��
    [Header("����λ��")]
    [Tooltip("������С��Ϸ��ķ���λ��")]

    public static Vector3 GlobalReturnPosition;   // ��¼��Ҵ����Ի�ʱ��λ��z


    public enum NPCType { VillageChief, ShuiCheNPC, KuangShiNPC, ChuanNPC, PulleyNPC }
    public static Talkable currentTalker; // ��ǰ�Ի���NPC

    [Header("��������")]
    [Tooltip("����NPC����\n�峤��Ҫ����Ի��߼�")]
    public NPCType npcType = NPCType.VillageChief;

    [Header("�Ի���������")]
    [TextArea(1, 3), Tooltip("��ʼ�Ի����ݣ��״ν���ʱ��ʾ��")]
    public string[] initialDialogue = { "Ĭ�ϳ�ʼ�Ի�" };

    [TextArea(1, 3), Tooltip("С��Ϸ��ɺ�ĶԻ����ݣ�����ͨNPC��Ч��")]
    public string[] postGameDialogue = { "Ĭ����ɶԻ�" };

    [TextArea(1, 3), Tooltip("�峤�����նԻ������������ͨNPC����ʾ��")]

    public string[] finalChiefDialogue = { "����ף�ضԻ�" };

    private bool playerInRange;

    public static int TotalcompletedNPCs;


    private void Update()
    {

        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {

            currentTalker = this;

            if (DialogueControl.Instance.dialogueBox.activeInHierarchy) return;
            switch (npcType)
            {
                case NPCType.VillageChief:
                    if (TotalcompletedNPCs < 3)
                    {
                        DialogueControl.Instance.ShowDialogue(initialDialogue);
                    }
                    else
                    {
                        DialogueControl.Instance.ShowDialogue(finalChiefDialogue);
                        TaskManager.Instance.HeshenTask.SetActive(true);
                        TaskManager.Instance.UpdateTaskState(4);
                    }

                    break;
                case NPCType.ShuiCheNPC:
                    if (PuzzleMG.PuzzlegameComplete == false)
                    {
                        DialogueControl.Instance.ShowDialogue(initialDialogue);

                    }
                    else
                    {
                        DialogueControl.Instance.ShowDialogue(postGameDialogue);
                    }
                    break;
                case NPCType.KuangShiNPC:
                    if (MineralUIManager.KuangshigameComplete == false)
                    {
                        DialogueControl.Instance.ShowDialogue(initialDialogue);

                    }
                    else
                    {
                        DialogueControl.Instance.ShowDialogue(postGameDialogue);
                    }
                    break;
                case NPCType.ChuanNPC:
                    if (UI.ChuangameComplete == false)
                    {
                        DialogueControl.Instance.ShowDialogue(initialDialogue);

                    }
                    else
                    {
                        DialogueControl.Instance.ShowDialogue(postGameDialogue);
                    }
                    break;
                case NPCType.PulleyNPC:
                    if (BucketRopeController.PulleygameComplete == false)
                    {
                        DialogueControl.Instance.ShowDialogue(initialDialogue);
                    }
                    else
                    {
                        DialogueControl.Instance.ShowDialogue(postGameDialogue);
                        TaskManager.Instance.UpdateTaskState(6);
                    }
                    break;
                default:
                    break;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            GlobalReturnPosition = other.transform.position;

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

        }
    }

}